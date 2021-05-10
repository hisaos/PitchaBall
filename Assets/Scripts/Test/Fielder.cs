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

    // Start is called before the first frame update
    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _rb = this.gameObject.GetComponent<Rigidbody>();
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    void FixedUpdate()
    {
      // ボールを追っかけさせる
      if (!_chaseBall) return;
      if (!_ball) return;

      var ballPos = _ball.transform.position;
      var pos = this.transform.position;
      var moveDir = new Vector3(ballPos.x - pos.x, 0f, ballPos.z - pos.z).normalized;

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

        foreach (var f in _fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: _pitcher,
            eventData: null,
            functor: (receiver, eventData) => {
              receiver.ResetFielderBall();
              receiver.DisableFielderMove();
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
    }
  }
}