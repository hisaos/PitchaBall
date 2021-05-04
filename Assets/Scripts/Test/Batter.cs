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

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
      _ia = new InputActions();
      _rb = GetComponent<Rigidbody>();
      // バットの配置
      _pivot = this.transform.position;
      _bat = Instantiate(bat, _pivot + Vector3.right * 2, Quaternion.identity);
      _bat.transform.SetParent(this.transform);
      
      _ia.Player.A.performed += (context) =>
      {
        Debug.Log(this.name + ": A");
      };
      _ia.Player.B.performed += (context) =>
      {
        Debug.Log(this.name + ": B");
      };
    }

    private void FixedUpdate()
    {
      _stickVector = _ia.Player.Move.ReadValue<Vector2>();

      _rb.AddForce(new Vector3(-_stickVector.x, 0f, -_stickVector.y) * moveSpeed, ForceMode.VelocityChange);
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