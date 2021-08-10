using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Batter : MonoBehaviour, IBatterMessageHandler
  {
    private InputActions _ia;
    private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } set { isPlayer = value; } }

    private Vector3 initialPosition = new Vector3(-1f, 1f, 8f);

    private Vector2 stickVector;

    private Rigidbody rb;

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
    private bool isSwingingBat = false;
    private float timeToSwingBat = 2.7f;
    private float timeToKeepSwinging = -1f;

    public float power = 2f;
    private Bat _batComponent;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
      isPlayer = true;

      _ia = new InputActions();
      rb = GetComponent<Rigidbody>();
      // バットの配置
      pivot = this.transform.position;
      bat = Instantiate(Bat, pivot, Quaternion.identity);
      bat.transform.SetParent(this.transform);
      batSwingVector = -1f;
      batAngle = minBatAngle;
      _batComponent = bat.GetComponentInChildren<Bat>();

      _ia.Player.A.performed += (context) =>
      {
        // _batAngleを増やしてバットを振る
        Debug.Log(this.name + ": A Press");
        if (isPlayer) batSwingVector = 1f;
      };
      _ia.Player.A.canceled += (context) =>
      {
        Debug.Log(this.name + ": A Release");
        if (isPlayer) batSwingVector = -1f;
      };
      _ia.Player.B.performed += (context) =>
      {
        Debug.Log(this.name + ": B");
      };
    }

    private void FixedUpdate()
    {
      stickVector = _ia.Player.Move.ReadValue<Vector2>();
      //Debug.Log("x:" + _stickVector.x + " y:" + _stickVector.y);

      if (isPlayer) rb.velocity = new Vector3(-stickVector.x, 0f, -stickVector.y) * moveSpeed;
      // _rb.AddForce(new Vector3(-_stickVector.x, 0f, -_stickVector.y) * moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
      _batComponent.batterPower = power;

      // CPU制御用
      if (!isPlayer)
      {
        batSwingTime = 180f / batSwingSpeed;

        timeToSwingBat -= Time.deltaTime;
        timeToKeepSwinging -= Time.deltaTime;

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
      Debug.Log(this.gameObject.name + "EnableBatter");
      this.transform.position = initialPosition;
      isPlayer = !isPlayer;
      timeToSwingBat = 2.7f + Random.Range(-0.4f, 0.4f);
      timeToKeepSwinging = -1f;
      isSwingingBat = false;
      batAngle = minBatAngle;
      batSwingVector = -1f;
    }

    private void OnEnable()
    {
      _ia.Player.Enable();
    }

    private void OnDisable()
    {
      _ia.Player.Disable();
    }
  }
}