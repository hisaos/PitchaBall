using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour
{
  private Player _p;

  private InputActions _ia;
  private Vector2 _stickVector;

  private Transform _body;
  private Rigidbody _rb;

  private Vector3 _zv = Vector3.zero;
  //private Vector3 _bodyHome = Vector3.up;

  public GameObject ballPrefab;       // 投げるボール
  public float throwSpeed = 10f;      // 投げるボールのスピード
  public float moveSpeed = 50f;       // 歩く時のスピード
  public float distance_limit = 0.1f; // 塁カバー時の最小距離。これを下回ったらベースに張り付いて止まる

  // ボール持ってるかフラグ
  [SerializeField] private bool _hasBall;
  public bool hasBall { get { return _hasBall; } set { _hasBall = value; } }

  // 送球後の硬直時間
  private const float _freezeTime = 0.3f;
  private float _freezeTimer;

  // 野手が認識しているベース
  public GameObject[] bases;

  // カバーするポジション
  // Bボタンの時はこれを変えるだけ
  public enum Covering
  {
    None,
    First,
    Second,
    Third,
    Home,
  }
  public Covering cover;
  private Covering _origCover;     // 元々のカバー。ボール持って歩いてボール投げたあとの戻り先。

  // Start is called before the first frame update
  void Awake()
  {
    _p = GetComponent<Player>();
    _ia = new InputActions();
    _body = transform.GetChild(0);
    _rb = GetComponent<Rigidbody>();

    _origCover = cover;

    _freezeTimer = 0f;
    _hasBall = false;

    _ia.Player.A.performed += (context) =>
    {
      if (_hasBall && ballPrefab)
      {
        //_stickVector = _ia.Player.Move.ReadValue<Vector2>();

        Vector3 v = (bases[0].transform.position - this.transform.position).normalized;
        if (_stickVector.y < 0f)
          v = (bases[3].transform.position - this.transform.position).normalized;
        else if (_stickVector.x < 0f)
          v = (bases[2].transform.position - this.transform.position).normalized;
        else if (_stickVector.y > 0f)
          v = (bases[1].transform.position - this.transform.position).normalized;

        var b = Instantiate(ballPrefab, this.transform.position + Vector3.up * 3f, Quaternion.identity, null);
        var rb = b.GetComponent<Rigidbody>();
        rb.AddForce(v * throwSpeed, ForceMode.Impulse);

        _freezeTimer = _freezeTime;
        _hasBall = false;
        cover = _origCover;
      }
    };
    _ia.Player.B.performed += (context) =>
    {
      if (_hasBall)
      {
        //_stickVector = _ia.Player.Move.ReadValue<Vector2>();

        cover = Covering.First;
        if (_stickVector.y < 0f)
          cover = Covering.Home;
        else if (_stickVector.x < 0f)
          cover = Covering.Third;
        else if (_stickVector.y > 0f)
          cover = Covering.Second;
      }
    };
  }

  private void Update()
  {
    _stickVector = _ia.Player.Move.ReadValue<Vector2>();
  }

  private void FixedUpdate()
  {
    if (GameManager.Instance.mode != GameManager.Mode.Field)
      return;

    if (_freezeTimer > 0f)
    {
      _freezeTimer -= Time.deltaTime;
      return;
    }
    if (cover != Covering.None)
    {
      if (Vector3.Distance(bases[(int)cover].transform.position, this.transform.position) > distance_limit)
      {
        var v = (bases[(int)cover].transform.position - this.transform.position).normalized;
        //this.transform.position += new Vector3(v.x, 0f, v.z) * Time.deltaTime * moveSpeed;
        _rb.AddForce(new Vector3(v.x, 0f, v.z) * moveSpeed, ForceMode.Acceleration);
      }
      else
      {
        this.transform.position = bases[(int)cover].transform.position;
        _rb.velocity = Vector3.zero;
      }
    }
    else if (!_hasBall)
    {
      _stickVector = _ia.Player.Move.ReadValue<Vector2>();

      //this.transform.position += new Vector3(_stickVector.x, 0f, _stickVector.y) * Time.deltaTime * moveSpeed;
      _rb.AddForce(new Vector3(_stickVector.x, 0f, _stickVector.y) * moveSpeed, ForceMode.Acceleration);
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
}