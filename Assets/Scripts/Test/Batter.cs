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
    private bool isSwingingBat = false;
    private float timeToSwingBat;
    private float timeToKeepSwinging = -1f;

    public float power = 2f;
    private Bat batComponent;

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
        // _batAngleを増やしてバットを振る
        Debug.Log(this.name + ": A Press");
        if (isPlayer) batSwingVector = 1f;
      };
      inputActions.Player.A.canceled += (context) =>
      {
        Debug.Log(this.name + ": A Release");
        if (isPlayer) batSwingVector = -1f;
      };
      inputActions.Player.B.performed += (context) =>
      {
        Debug.Log(this.name + ": B");
      };
    }

    private void FixedUpdate()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();
      //Debug.Log("x:" + _stickVector.x + " y:" + _stickVector.y);

      if (isPlayer) playerRigidbody.velocity = new Vector3(-stickVector.x, 0f, -stickVector.y) * moveSpeed;
      // _rb.AddForce(new Vector3(-_stickVector.x, 0f, -_stickVector.y) * moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
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
          isSwingingBat = true;
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
      bat.transform.rotation = Quaternion.Euler(0f, batAngle, 0f);
    }

    public void EnableBatter()
    {
      Debug.Log(this.gameObject.name + ": EnableBatter");
      this.transform.position = initialPosition;
      isPlayer = !isPlayer;
      timeToSwingBat = 0.7f + Random.Range(-0.1f, 0.1f);
      timeToKeepSwinging = -1f;
      isCountingDownToSwing = false;
      isSwingingBat = false;
      batAngle = minBatAngle;
      batSwingVector = -1f;
    }

    public void NotifyBallThrown()
    {
      isCountingDownToSwing = true;
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