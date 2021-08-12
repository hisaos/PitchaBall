using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test2
{
  public class Fielder : MonoBehaviour
  {
    public GameObject ball;
    public float pitchUpForce;
    public float pitchForwardForce;

    private GameObject pitcher;
    private GameObject catcher;
    private GameObject thrownBall;
    private bool isChasingBall;
    private Rigidbody fielderRigidbody;

    public float moveSpeed = 3f;
    private List<Fielder> fielders;
    public Vector3 originalPosition;

    private float distToBall;
    private bool isMinDistToBall;

    private bool pickBall;
    private bool isCatcher = false;

    // Start is called before the first frame update
    void Start()
    {
      pitcher = FindObjectOfType<Pitcher>().gameObject;
      catcher = FindObjectOfType<Catcher>().gameObject;
      if (catcher == this.gameObject) isCatcher = true;
      fielderRigidbody = this.gameObject.GetComponent<Rigidbody>();
      fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
      originalPosition = this.transform.position;
    }

    void FixedUpdate()
    {
      // ボールを追っかけさせる
      if (!(thrownBall && isChasingBall)) return;

      // ボールまでのx-z平面上の距離の計算
      var ballPos = thrownBall.transform.position;
      var pos = this.transform.position;
      var distToBall = new Vector3(ballPos.x - pos.x, 0f, ballPos.z - pos.z);
      this.distToBall = distToBall.magnitude;

      // 互いに距離を教え合う
      // foreach (var f in fielders)
      // {
      //   if (f.gameObject == this.gameObject) continue;
      //   ExecuteEvents.Execute<IFielderMessageHandler>(
      //     target: f.gameObject,
      //     eventData: null,
      //     functor: (receiver, eventData) =>
      //     {
      //       receiver.TellDistanceToBall(this.distToBall);
      //     }
      //   );
      // }

      // もし一番ボールに近くなかったら止まる
      if (!isMinDistToBall)
      {
        fielderRigidbody.velocity = Vector3.zero;
        fielderRigidbody.angularVelocity = Vector3.zero;
        return;
      }

      // 動く方向ベクトルの計算、実際に動く
      var moveDir = distToBall.normalized;
      // velocityを直接変えて一定スピードで動くようにする
      fielderRigidbody.velocity = moveDir * moveSpeed;
      //      _rb.AddForce(moveDir * moveSpeed, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを拾った
        pickBall = true;

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
        if (!isCatcher)
        {
          var b = Instantiate(ball);
          b.transform.localPosition = this.transform.position + Vector3.forward;
          var rb = b.GetComponent<Rigidbody>();
          var _directionToCatcher = (catcher.transform.position - this.transform.position).normalized;
          rb.AddForce(_directionToCatcher * pitchForwardForce + Vector3.up * pitchUpForce, ForceMode.Impulse);
          pickBall = false;
        }
      } 
      else if (other.gameObject.CompareTag("Wall"))
      {
        fielderRigidbody.velocity = Vector3.zero;
      }
    }

    // ボールが投げられた時のメッセージハンドラ
    public void SetFielderBall(GameObject ball)
    {
      Debug.Log("SetFielderBall");
      thrownBall = ball;
    }

    // ボールが打たれた時のメッセージハンドラ
    public void EnableFielderMove()
    {
      Debug.Log("EnableFielderMove");
      isChasingBall = true;
    }

    // ボールが消えた時のメッセージハンドラ
    public void ResetFielderBall()
    {
      Debug.Log("ResetFielderBall");
      thrownBall = null;
    }

    // 追わなくなる時のメッセージハンドラ
    public void DisableFielderMove()
    {
      Debug.Log("DisableFielderMove");
      fielderRigidbody.velocity = Vector3.zero;
      isChasingBall = false;
      isMinDistToBall = false;
    }

    // 元の位置に戻る時のメッセージハンドラ
    public void ReturnToOriginalPosition()
    {
      Debug.Log("Return");
      fielderRigidbody.velocity = Vector3.zero;
      this.transform.position = originalPosition;
      pickBall = false;
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float dist)
    {
      if (dist > distToBall) isMinDistToBall = true;
      else isMinDistToBall = false;
    }
  }
}