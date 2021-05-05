using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    public float batterPower { get; set; }
    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        var r = other.gameObject.GetComponent<Rigidbody>();
        r.velocity = Vector3.zero;
        r.AddForce((r.transform.position - other.GetContact(0).point).normalized * batterPower, ForceMode.Impulse);
      }
    }
  }
}