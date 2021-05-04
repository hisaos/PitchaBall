using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        var r = other.gameObject.GetComponent<Rigidbody>();
        r.AddForce((r.transform.position - other.GetContact(0).point).normalized * 2f, ForceMode.Impulse);
      }
    }
  }
}