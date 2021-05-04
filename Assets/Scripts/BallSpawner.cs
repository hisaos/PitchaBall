using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
  public GameObject ball;
  public Vector3 spawnPosition; // ボールが飛び出す始点
  public Vector3 spawnVector;    // ボールを飛ばす方向
  public float spawnPower;

  // Start is called before the first frame update
  void OnEnable()
  {
    var b = Instantiate(ball, spawnPosition, Quaternion.identity, null);
    var rb = b.GetComponent<Rigidbody>();
    rb.AddForce(spawnVector * spawnPower, ForceMode.Impulse);
  }
}
