using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class Batter : MonoBehaviour
  {
    private InputActions _ia;
    private Vector2 _stickVector;

    private Rigidbody _rb;

    public GameObject bat;
    private GameObject _bat;
    private Vector3 _pivot;
    private float _batAngle;
    private float _batSwingVector;
    public float maxBatAngle = 90f;
    public float minBatAngle = -90f;

    public float power = 2f;
    private Bat _batComponent;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
      _ia = new InputActions();
      _rb = GetComponent<Rigidbody>();
      // バットの配置
      _pivot = this.transform.position;
      _bat = Instantiate(bat, _pivot, Quaternion.identity);
      _bat.transform.SetParent(this.transform);
      _batSwingVector = -5f;
      _batAngle = minBatAngle;
      _batComponent = _bat.GetComponentInChildren<Bat>();

      _ia.Player.A.performed += (context) =>
      {
        // _batAngleを増やしてバットを振る
        Debug.Log(this.name + ": A Press");
        _batSwingVector = 5f;
      };
      _ia.Player.A.canceled += (context) =>
      {
        Debug.Log(this.name + ": A Release");
        _batSwingVector = -5f;
      };
      _ia.Player.B.performed += (context) =>
      {
        Debug.Log(this.name + ": B");
      };
    }

    private void FixedUpdate()
    {
      _stickVector = _ia.Player.Move.ReadValue<Vector2>();
      //Debug.Log("x:" + _stickVector.x + " y:" + _stickVector.y);

      _rb.velocity = new Vector3(-_stickVector.x, 0f, -_stickVector.y) * moveSpeed;
      // _rb.AddForce(new Vector3(-_stickVector.x, 0f, -_stickVector.y) * moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
      _batComponent.batterPower = power;
      _batAngle += _batSwingVector;
      if (_batAngle > maxBatAngle) _batAngle = maxBatAngle;
      else if (_batAngle < minBatAngle) _batAngle = minBatAngle;
      _bat.transform.rotation = Quaternion.Euler(0f, _batAngle, 0f);
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