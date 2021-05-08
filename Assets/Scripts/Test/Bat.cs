using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    public float batterPower { get; set; }

    private Collider _col;

    private GameObject _pitcher;

    void Start()
    {
      _col = GetComponentInChildren<Collider>();
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを飛ばす
        var r = other.gameObject.GetComponent<Rigidbody>();
        r.velocity = Vector3.zero;
        // otherとのコンタクト点よりボールの方が前にあるなら、ボール - コンタクト点の位置の向きに飛ばすと前に飛ぶ
        r.AddForce(((r.transform.position - other.GetContact(0).point).normalized + Vector3.up / 2f) * batterPower, ForceMode.Impulse);

        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<ICustomMessageTarget>(
            target: _pitcher,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // ボールを消す（1.5秒後）
        Destroy(other.gameObject, 1.5f);
        _col.enabled = false;
        Invoke("EnableBat", 1.5f);
      }
    }

    void EnableBat()
    {
      _col.enabled = true;
    }
  }
}