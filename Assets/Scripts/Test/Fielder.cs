using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Fielder : MonoBehaviour, IFielderMessageHandler
  {
    public GameObject ball;
    public float pitchUpForce;
    public float pitchForwardForce;

    private GameObject _pitcher;
    private GameObject _catcher;
    private GameObject _pitchedBall;
    private bool _chaseBall;
    private Rigidbody _rb;

    public float moveSpeed = 3f;
    private List<Fielder> _fielders;
    public Vector3 originalPosition;

    private float _distToBall;
    private bool _isMinDistToBall;

    private bool _pickBall;
    private bool _isCatcher = false;

    // Start is called before the first frame update
    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _catcher = FindObjectOfType<Catcher>().gameObject;
      if (_catcher == this.gameObject) _isCatcher = true;
      _rb = this.gameObject.GetComponent<Rigidbody>();
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
      originalPosition = this.transform.position;
    }

    void FixedUpdate()
    {
      // ボールを追っかけさせる
      if (!(_pitchedBall && _chaseBall)) return;

      // ボールまでのx-z平面上の距離の計算
      var ballPos = _pitchedBall.transform.position;
      var pos = this.transform.position;
      var distToBall = new Vector3(ballPos.x - pos.x, 0f, ballPos.z - pos.z);
      _distToBall = distToBall.magnitude;

      // 互いに距離を教え合う
      foreach (var f in _fielders)
      {
        if (f.gameObject == this.gameObject) continue;
        ExecuteEvents.Execute<IFielderMessageHandler>(
          target: f.gameObject,
          eventData: null,
          functor: (receiver, eventData) =>
          {
            receiver.TellDistanceToBall(_distToBall);
          }
        );
      }

      // もし一番ボールに近くなかったら止まる
      if (!_isMinDistToBall)
      {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        return;
      }

      // 動く方向ベクトルの計算、実際に動く
      var moveDir = distToBall.normalized;
      // velocityを直接変えて一定スピードで動くようにする
      _rb.velocity = moveDir * moveSpeed;
      //      _rb.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを拾った
        _pickBall = true;

        // // ピッチャーを投げれる状態にする
        // ExecuteEvents.Execute<IPitcherMessageHandler>(
        //   target: _pitcher,
        //   eventData: null,
        //   functor: (receiver, eventData) => receiver.EnablePitch()
        // );

        // // ボールを取ったら追わずに元の位置へ
        // foreach (var f in _fielders)
        // {
        //   ExecuteEvents.Execute<IFielderMessageHandler>(
        //     target: f.gameObject,
        //     eventData: null,
        //     functor: (receiver, eventData) =>
        //     {
        //       receiver.ResetFielderBall();
        //       receiver.DisableFielderMove();
        //       receiver.ReturnToOriginalPosition();
        //     }
        //   );
        // }

        // ボールを消す（即時）->捕球
        Destroy(other.gameObject);

        // ボールを返球する（雑）
        if (!_isCatcher)
        {
          var b = Instantiate(ball);
          b.transform.localPosition = this.transform.position + Vector3.forward;
          var rb = b.GetComponent<Rigidbody>();
          var _directionToCatcher = (_catcher.transform.position - this.transform.position).normalized;
          rb.AddForce(_directionToCatcher * pitchForwardForce + Vector3.up * pitchUpForce, ForceMode.Impulse);
          _pickBall = false;
        }
      } 
      else if (other.gameObject.CompareTag("Wall"))
      {
        _rb.velocity = Vector3.zero;
      }
    }

    // ボールが投げられた時のメッセージハンドラ
    public void SetFielderBall(GameObject ball)
    {
      Debug.Log("SetFielderBall");
      _pitchedBall = ball;
    }

    // ボールが打たれた時のメッセージハンドラ
    public void EnableFielderMove()
    {
      Debug.Log("EnableFielderMove");
      _chaseBall = true;
    }

    // ボールが消えた時のメッセージハンドラ
    public void ResetFielderBall()
    {
      Debug.Log("ResetFielderBall");
      _pitchedBall = null;
    }

    // 追わなくなる時のメッセージハンドラ
    public void DisableFielderMove()
    {
      Debug.Log("DisableFielderMove");
      _rb.velocity = Vector3.zero;
      _chaseBall = false;
      _isMinDistToBall = false;
    }

    // 元の位置に戻る時のメッセージハンドラ
    public void ReturnToOriginalPosition()
    {
      Debug.Log("Return");
      _rb.velocity = Vector3.zero;
      this.transform.position = originalPosition;
      _pickBall = false;
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float dist)
    {
      if (dist > _distToBall) _isMinDistToBall = true;
      else _isMinDistToBall = false;
    }
  }
}