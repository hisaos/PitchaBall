using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Pitcher : MonoBehaviour
  {
    public GameObject ball;

    // Update is called once per frame
    void Update()
    {
        var b = Instantiate(ball);
        b.transform.localPosition = this.transform.position + Vector3.forward;        
    }
  }
}