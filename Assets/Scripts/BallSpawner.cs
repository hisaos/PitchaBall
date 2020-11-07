using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
  public GameObject ball;
  public Vector3 spawnPosition;

  // Start is called before the first frame update
  void Start()
  {
    Instantiate(ball, spawnPosition, Quaternion.identity, null);
  }
}
