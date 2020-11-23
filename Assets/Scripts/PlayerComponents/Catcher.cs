using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catcher : MonoBehaviour
{
  private GameObject _ball;
  private Vector3 _home;
  private Rigidbody _rb;
  public float moveSpeed = 10f;
  public float distance_limit = 0.1f;

  private void Start()
  {
    _rb = this.gameObject.GetComponent<Rigidbody>();
    _home = new Vector3(0f, 0f, -8.5f);
  }

  // Update is called once per frame
  void Update()
  {
    if (_ball)
      this.transform.position = new Vector3(_ball.transform.position.x, this.transform.position.y, this.transform.position.z);
    else
      // ボールが投げられた瞬間を追いかけるために必要
      StartCoroutine(MonitorBall());
  }

  public void ReturnToHome()
  {
    this.transform.position = _home;
  }

  IEnumerator MonitorBall()
  {
    while (!_ball)
    {
      _ball = GameObject.FindGameObjectWithTag("Ball");

      // ボールが消えた時に元の場所に引っ張ってくるための処理
      if (Vector3.Distance(_home, this.transform.position) > distance_limit)
      {
        this.transform.position += new Vector3(_home.x - this.transform.position.x, 0f, 0f).normalized * Time.deltaTime * moveSpeed;
      }
      else
      {
        this.transform.position = _home;
      }

      yield return new WaitForEndOfFrame();
    }
  }

  public void StopMonitorBall()
  {
    StopAllCoroutines();
  }
}