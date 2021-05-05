﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Pitcher : MonoBehaviour
  {
    public GameObject ball;
    private float _pitchInterval;
    public float pitchInterval = 1.5f;
    public float pitchForwardForce = 0.8f;
    public float pitchUpForce = 0.4f;

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
        rb.AddForce(Vector3.forward * pitchForwardForce + Vector3.up * pitchUpForce, ForceMode.Impulse);
      }
    }
  }
}