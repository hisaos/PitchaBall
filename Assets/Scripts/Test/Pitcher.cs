using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Pitcher : MonoBehaviour
  {
    public GameObject ball;
    private float _pitchInterval;
    public float pitchInterval = 0.5f;

    void Start()
    {
      _pitchInterval = pitchInterval;
    }

    // Update is called once per frame
    void Update()
    {
      _pitchInterval -= Time.deltaTime;
      if (_pitchInterval < 0f)
      {
        var b = Instantiate(ball);
        b.transform.localPosition = this.transform.position + Vector3.forward;
        _pitchInterval = pitchInterval;
        var rb = b.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * 10f + Vector3.up * 3f, ForceMode.Impulse);
      }
    }
  }
}