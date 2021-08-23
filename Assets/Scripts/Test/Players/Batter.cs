using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Batter : MonoBehaviour, IBatterMessageHandler
  {
    private InputActions inputActions;
    private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } set { isPlayer = value; } }

    private Vector3 initialPosition = new Vector3(-1f, 1f, 8f);

    private Vector2 stickVector;

    private Rigidbody playerRigidbody;

    public GameObject Bat;
    private GameObject bat;
    private Vector3 pivot;
    private float batAngle;
    private float batSwingVector;
    public float maxBatAngle = 90f;
    public float minBatAngle = -90f;

    private float batSwingTime;
    public float batSwingSpeed = 5f;

    // CPU制御用
    private bool isCountingDownToSwing = false;
    private float timeToSwingBat;
    private float timeToKeepSwinging = -1f;

    public float power = 2f;
    private Bat batComponent;

    // ボールの位置追尾用
    private GameObject ball;
    private float prev_bx;
    private float bx;
    public Vector3 ball_offset;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
      isPlayer = true;

      inputActions = new InputActions();
      playerRigidbody = GetComponent<Rigidbody>();

      // バットの配置
      pivot = this.transform.position;
      bat = Instantiate(Bat, pivot, Quaternion.identity);
      bat.transform.SetParent(this.transform);
      batSwingVector = -1f;
      batAngle = minBatAngle;
      batComponent = bat.GetComponentInChildren<Bat>();
      timeToSwingBat = 0.7f + Random.Range(-0.1f, 0.1f);

      inputActions.Player.A.performed += (context) =>
      {
        // batAngleを増やしてバットを振る
        if (isPlayer) batSwingVector = 1f;
      };
      inputActions.Player.A.canceled += (context) =>
      {
        // batAngleを減らしてバットを戻す
        if (isPlayer) batSwingVector = -1f;
      };
      inputActions.Player.B.performed += (context) =>
      {
      };
    }

    private void FixedUpdate()
    {
      if (ball)
      {
        prev_bx = bx;
        bx = ball.transform.position.x;
      }

      if (isPlayer) playerRigidbody.velocity = new Vector3(-stickVector.x, 0f, -stickVector.y) * moveSpeed;
      else
      {
        // var d = (bx - prev_bx);
        // if (ball) playerRigidbody.velocity = new Vector3(d / Mathf.Abs(d), 0f, 0f) * moveSpeed;
        // else playerRigidbody.velocity = Vector3.zero;
      }
    }

    private void Update()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();

      batComponent.batterPower = power;

      // CPU制御用
      if (!isPlayer)
      {
        batSwingTime = 180f / batSwingSpeed;

        if (isCountingDownToSwing) timeToSwingBat -= Time.deltaTime;
        // Debug.Log("TimeToSwing: " + timeToSwingBat);
        if (timeToKeepSwinging >= 0f) timeToKeepSwinging -= Time.deltaTime;

        if (timeToSwingBat <= 0f)
        {
          ball = null;
          timeToKeepSwinging = batSwingTime;
        }
        batSwingVector = timeToKeepSwinging >= 0f ? 1f : -1f;
      }

      SwingBat();
    }

    private void SwingBat()
    {
      batAngle += batSwingVector * batSwingSpeed;
      if (batAngle > maxBatAngle) batAngle = maxBatAngle;
      else if (batAngle < minBatAngle) batAngle = minBatAngle;

      // Pitcherが投げた後にバットがスイングしてるか判定
      if (BattingManager.Instance.IsPitched && batAngle > 0f) BattingManager.Instance.IsBatSwung = true;

      bat.transform.rotation = Quaternion.Euler(0f, batAngle, 0f);
    }

    public void EnableBatter()
    {
      // Debug.Log(this.gameObject.name + ": EnableBatter");
      this.transform.position = initialPosition;
      isPlayer = BattingManager.Instance.IsTop;
      timeToSwingBat = 0.7f + Random.Range(-0.1f, 0.1f);
      timeToKeepSwinging = -1f;
      isCountingDownToSwing = false;
      batAngle = minBatAngle;
      batSwingVector = -1f;
      prev_bx = 0f;
      bx = 0f;
    }

    public void NotifyBallThrown(GameObject ball)
    {
      isCountingDownToSwing = true;
      this.ball = ball;
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // 判定済みなら何もしない
        if (BattingManager.Instance.IsBallBounded) return;

        // バウンド判定つける
        BattingManager.Instance.IsBallBounded = true;

        // デッドボールはランナーを出す
        BattingManager.Instance.SetJudgeText("デッドボール");
        BattingManager.Instance.ResetCount();
        RunnerManager.Instance.InstantiateRunner();
        RunnerManager.Instance.ProceedAllRunners();
        RunnerManager.Instance.NotifyRunnersFair();
        BattingManager.Instance.TriggerReturn();

        Destroy(other.gameObject);
      }
    }

    private void OnEnable()
    {
      inputActions.Player.Enable();
    }

    private void OnDisable()
    {
      inputActions.Player.Disable();
    }
  }
}