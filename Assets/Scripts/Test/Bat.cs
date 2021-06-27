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
    private List<Fielder> _fielders;

    void Start()
    {
      _col = GetComponentInChildren<Collider>();
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを飛ばす
        var r = other.gameObject.GetComponent<Rigidbody>();
        r.velocity = Vector3.zero;
        r.useGravity = true;
        // otherとのコンタクト点よりボールの方が前にあるなら、ボール - コンタクト点の位置の向きに飛ばすと前に飛ぶ
        r.AddForce(((r.transform.position - other.GetContact(0).point).normalized + Vector3.up / 2f) * batterPower, ForceMode.Impulse);

        // バットを一時的に無効化（1.5秒後復活）
        _col.enabled = false;
        Invoke("EnableBat", 1.5f);

        foreach (var f in _fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnableFielderMove()
          );
        }

      }
    }

    void EnableBat()
    {
      _col.enabled = true;
    }
  }
}