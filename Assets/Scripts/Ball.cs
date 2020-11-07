using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
  private Rigidbody _rb;

  private void Awake()
  {
    _rb = this.GetComponent<Rigidbody>();
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Wall"))
    {
      _rb.useGravity = true;
      Destroy(this.gameObject, 1f);
    }
    else if (collision.gameObject.CompareTag("Body"))
    {
      Destroy(this.gameObject);
    }
  }
}
