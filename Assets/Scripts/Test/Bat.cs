using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    public float batterPower { get; set; }

    private GameObject _pitcher;

    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを飛ばす
        var r = other.gameObject.GetComponent<Rigidbody>();
        r.velocity = Vector3.zero;
        r.AddForce((r.transform.position - other.GetContact(0).point).normalized * batterPower, ForceMode.Impulse);
      }
    }
  }
}