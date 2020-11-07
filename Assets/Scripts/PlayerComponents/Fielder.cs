using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour
{
  private Player _p;

  private InputActions _ia;

  public GameObject ballPrefab;       // 投げるボール
  public float throwSpeed = 10f;      // 投げるボールのスピード
  public float moveSpeed = 5f;        // 歩く時のスピード
  public float distance_limit = 0.1f; // 塁カバー時の最小距離。これを下回ったらベースに張り付いて止まる

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
    First,
    Second,
    Third,
    Home,
    None
  }
  public Covering position;

  // Start is called before the first frame update
  void Awake()
  {
    _p = GetComponent<Player>();
    _ia = new InputActions();

    _freezeTimer = 0f;
    _hasBall = false;

    _ia.Player.A.performed += (context) =>
    {
      if (_hasBall && ballPrefab)
      {
        Vector2 stickVector = _ia.Player.Move.ReadValue<Vector2>();

        Vector3 v = (bases[0].transform.position - this.transform.position).normalized;
        if (stickVector.y < 0f)
          v = (bases[3].transform.position - this.transform.position).normalized;
        else if (stickVector.x < 0f)
          v = (bases[2].transform.position - this.transform.position).normalized;
        else if (stickVector.y > 0f)
          v = (bases[1].transform.position - this.transform.position).normalized;

        var b = Instantiate(ballPrefab, this.transform.position + Vector3.up * 3f, Quaternion.identity, null);
        var rb = b.GetComponent<Rigidbody>();
        rb.AddForce(v * throwSpeed, ForceMode.Impulse);

        _freezeTimer = _freezeTime;
        _hasBall = false;
      }
    };
    _ia.Player.B.performed += (context) =>
    {
      if (_hasBall)
      {
        Vector2 stickVector = _ia.Player.Move.ReadValue<Vector2>();

        position = Covering.First;
        if (stickVector.y < 0f)
          position = Covering.Home;
        else if (stickVector.x < 0f)
          position = Covering.Third;
        else if (stickVector.y > 0f)
          position = Covering.Second;
      }
    };
  }

  // Update is called once per frame
  void Update()
  {
    if (GameManager.Instance.mode == GameManager.Mode.Main)
      return;

    if (_freezeTimer > 0f)
    {
      _freezeTimer -= Time.deltaTime;
      return;
    }

    if (position != Covering.None)
    {
      if (Vector3.Distance(bases[(int)position].transform.position, this.transform.position) > distance_limit)
      {
        var v = (bases[(int)position].transform.position - this.transform.position).normalized;
        this.transform.position += v * Time.deltaTime * moveSpeed;
      }
      else
      {
        this.transform.position = bases[(int)position].transform.position;
      }
    }
    else if (!_hasBall)
    {
      Vector2 stickVector = _ia.Player.Move.ReadValue<Vector2>();
      this.transform.position += new Vector3(stickVector.x, 0f, stickVector.y) * Time.deltaTime * moveSpeed;
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

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Ball"))
    {
      Destroy(collision.gameObject);
      _hasBall = true;
    }
  }

}