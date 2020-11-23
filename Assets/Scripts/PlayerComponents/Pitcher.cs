using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
  private InputActions _ia;
  private Player _p;

  private Vector3 _home;

  public float pitchSpeed = 10f;    // ボールの速度
  public float controlSpeed = 10f;  // ボールの変化速度
  public float moveSpeed = 2f;      // プレート上の移動速度
  public float moveLeftBound = -1f;
  public float moveRightBound = 1f;

  public GameObject ballPrefab;     // ボールのプレハブ
  private bool _isBallGoing;        // 投げたボールがCatcherに向かっているか
  private Rigidbody _ballRigidBody; // 投げたボールのRigidbody

  private void Awake()
  {
    _p = GetComponent<Player>();

    _home = Vector3.zero;

    _ia = new InputActions();

    _ia.Player.A.performed += (context) =>
    {
      if (!_isBallGoing)
      {
        var b = Instantiate(ballPrefab);
        b.transform.position = this.transform.position + Vector3.up + Vector3.back * 1.1f;

        _ballRigidBody = b.GetComponent<Rigidbody>();
        _ballRigidBody.useGravity = false;
        _ballRigidBody.AddForce(Vector3.back * pitchSpeed, ForceMode.VelocityChange);

        _isBallGoing = true;
        StartCoroutine(MonitorBall());
      }
    };
    _ia.Player.B.performed += (context) =>
    {
      if (GameManager.Instance.mode == GameManager.Mode.Main)
      {
        GameManager.Instance.SwitchModeTo(GameManager.Mode.Field, 0f);
        //        GameManager.Instance.SwitchCameraTo(CameraManager.CameraMode.Main, 3f);
        _p.SetFielderHasBall(true);
        _p.ChangeMode(Player.Mode.Fielder);
      }
    };
  }

  private void Update()
  {
    if (!_isBallGoing)
    {
      Vector2 horizontalVector = _ia.Player.Move.ReadValue<Vector2>();
      this.transform.position += new Vector3(horizontalVector.x, 0f, 0f) * Time.deltaTime * moveSpeed;
      if (this.transform.position.x > moveRightBound) this.transform.position = new Vector3(moveRightBound, 0f, 0f);
      else if (this.transform.position.x < moveLeftBound) this.transform.position = new Vector3(moveLeftBound, 0f, 0f);
    }
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    if (_ballRigidBody)
    {
      Vector2 horizontalVector = _ia.Player.Move.ReadValue<Vector2>();
      _ballRigidBody.AddForce(Vector3.right * controlSpeed * horizontalVector.x, ForceMode.Acceleration);
    }
  }

  private void OnEnable()
  {
    _ia.Player.Enable();
  }

  private void OnDisable()
  {
    _ia.Player.Disable();
  }

  public void ReturnToHome()
  {
    this.transform.position = _home;
  }

  IEnumerator MonitorBall()
  {
    while (_ballRigidBody)
    {
      yield return new WaitForEndOfFrame();
    }
    _isBallGoing = false;
  }

}
