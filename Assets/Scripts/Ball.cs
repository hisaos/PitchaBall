using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
  private Rigidbody _rb;
  private bool _collisionSemaphor;

  private void Awake()
  {
    _collisionSemaphor = true;
    _rb = this.GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    _collisionSemaphor = true;
  }

  void OnCollisionEnter(Collision collision)
  {
    if (!_collisionSemaphor)
      return;

    if (collision.gameObject.CompareTag("Wall"))
    {
      _rb.useGravity = true;
      _collisionSemaphor = false;
      //Destroy(this.gameObject, 1f);
    }
    else if (collision.gameObject.CompareTag("Player"))
    {
      // ボールがPlayerのボディにぶつかったとき
      var p = collision.gameObject.GetComponent<Player>();
      if (p.mode == Player.Mode.Fielder) p.SetFielderHasBall(true);
      _collisionSemaphor = false;
      Destroy(this.gameObject);
    }
    else if (collision.gameObject.CompareTag("Catcher"))
    {
      // Catcherがボールを取ったとき
      var p = collision.gameObject.GetComponent<Player>();
      if (p.mode == Player.Mode.Fielder) p.SetFielderHasBall(true);
      _collisionSemaphor = false;
      Destroy(this.gameObject);
    }
    else if (collision.gameObject.CompareTag("Pitcher"))
    {
      // Catcherがボールを取ったとき
      var p = collision.gameObject.GetComponent<Player>();
      if (p.mode == Player.Mode.Fielder) p.SetFielderHasBall(true);
      _collisionSemaphor = false;
      Destroy(this.gameObject);
    }
  }
}
