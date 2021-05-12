using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Fielder : MonoBehaviour, IFielderMessageHandler
  {
    private GameObject _pitcher;
    private GameObject _ball;
    private bool _chaseBall;
    private Rigidbody _rb;

    public float moveSpeed = 1f;
    private List<Fielder> _fielders;
    public Vector3 originalPosition;

    private float _distToBall;
    private bool _minDistToBall;

    // Start is called before the first frame update
    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _rb = this.gameObject.GetComponent<Rigidbody>();
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
      originalPosition = this.transform.position;
    }

    void FixedUpdate()
    {
      // ボールを追っかけさせる
      if (!(_ball && _chaseBall)) return;

      var ballPos = _ball.transform.position;
      var pos = this.transform.position;
      var diffToBall = new Vector3(ballPos.x - pos.x, 0f, ballPos.z - pos.z);
      _distToBall = diffToBall.magnitude;

      // ボールを取ったら追わずに元の位置へ
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

      var moveDir = diffToBall.normalized;

      _rb.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<IPitcherMessageHandler>(
          target: _pitcher,
          eventData: null,
          functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // ボールを取ったら追わずに元の位置へ
        foreach (var f in _fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) =>
            {
              receiver.ResetFielderBall();
              receiver.DisableFielderMove();
              receiver.ReturnToOriginalPosition();
            }
          );
        }

        // ボールを消す（即時）->捕球
        Destroy(other.gameObject);
      }
    }

    // ボールが投げられた時のメッセージハンドラ
    public void SetFielderBall(GameObject ball)
    {
      Debug.Log("SetFielderBall");
      _ball = ball;
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
      _ball = null;
    }

    // 追わなくなる時のメッセージハンドラ
    public void DisableFielderMove()
    {
      Debug.Log("DisableFielderMove");
      _rb.velocity = Vector3.zero;
      _chaseBall = false;
      _minDistToBall = false;
    }

    // 元の位置に戻る時のメッセージハンドラ
    public void ReturnToOriginalPosition()
    {
      Debug.Log("Return");
      _rb.velocity = Vector3.zero;
      this.transform.position = originalPosition;
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float dist)
    {
      if (dist > _distToBall) _minDistToBall = true;
      else _minDistToBall = false;
    }
  }
}