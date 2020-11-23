using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
  private Player _p;

  private InputActions _ia;
  private Vector2 _stickVector;

  [SerializeField] private bool _goAhead;
  [SerializeField] private bool _goBack;

  public float rate = 0f;
  public float speed = 10f;
  [SerializeField] private float _direction = 1f; // 前進(反時計回り)=1, 後退(時計回り)=-1

  private void Awake()
  {
    _direction = 0f;
    _p = GetComponent<Player>();
    _ia = new InputActions();

    _goAhead = false;
    _goBack = false;

    _ia.Player.A.performed += (context) => { _goBack = true; };
    _ia.Player.A.canceled += (context) => { _goBack = false; };
    _ia.Player.B.performed += (context) => { _goAhead = true; };
    _ia.Player.B.canceled += (context) => { _goAhead = false; };
  }

  // Update is called once per frame
  void Update()
  {
    if (_goAhead && _goBack) _direction = 0f; // 両方入力が一番条件が厳しいので一番先に
    else if (_goAhead) _direction = 1f;
    else if (_goBack) _direction = -1f;

    rate += speed * _direction * Time.deltaTime;
    if (rate < 0) rate += 100f;
    rate %= 100;
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
