using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Fielder : MonoBehaviour, IFielderMessageHandler
  {
    private InputActions inputActions;
    private Vector2 stickVector;
    private Rigidbody fielderRigidbody;
    public float moveSpeed = 1f;
    public bool isPlayer;
    public bool isMoving;
    private GameObject targetBall;

    // 守備位置番号(1-9)
    public int fielderPositionNumber;

    // 守備位置ベクトル(1-9に対応)
    public List<Vector3> fielderPositionVector;

    void Awake()
    {
      isPlayer = false;
      isMoving = false;
      inputActions = new InputActions();
      fielderRigidbody = GetComponent<Rigidbody>();
      fielderPositionVector = new List<Vector3>{
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 1f, 9f),
        new Vector3(-6f, 1f, -3f),
        new Vector3(-3f, 1f, -6f),
        new Vector3(6f, 1f, -3f),
        new Vector3(3f, 1f, -6f),
        new Vector3(10f, 1f, -12f),
        new Vector3(0f, 1f, -16f),
        new Vector3(-10f, 1f, -12f),
      };
    }

    void Update()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
      if (isMoving)
      {
        if (isPlayer) fielderRigidbody.velocity = new Vector3(-stickVector.x, 0f, -stickVector.y) * moveSpeed;
        else
        {
          if (targetBall)
          {
            var direction = (targetBall.transform.position - transform.position).normalized;
            fielderRigidbody.velocity = new Vector3(direction.x, 0f, direction.z) * moveSpeed;
          }
        }
      }
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        if (!BattingManager.Instance.IsBallBounded)
        {
          // バウンド判定つける
          BattingManager.Instance.IsBallBounded = true;

          // フライアウト
          BattingManager.Instance.CountOut();
          BattingManager.Instance.SetJudgeText("アウト");
          Destroy(other.gameObject);
        }
      }
    }

    void OnEnable()
    {
      inputActions.Player.Enable();
    }

    void OnDisable()
    {
      inputActions.Player.Disable();
    }

    // ボールが投げられた時のメッセージハンドラ
    public void SetFielderBall(GameObject g)
    {
      targetBall = g;
    }

    // ボールが消えた時のメッセージハンドラ
    public void ResetFielderBall()
    {
      targetBall = null;
    }

    // ボールが打たれた時のメッセージハンドラ
    public void EnableFielderMove()
    {
      isMoving = true;
    }

    // 追わなくなる時のメッセージハンドラ
    public void DisableFielderMove()
    {
      isMoving = false;
    }

    // 元の位置に戻る
    public void ReturnToOriginalPosition()
    {
      // 攻撃している側に応じてisPlayerを切り替える
      isPlayer = !BattingManager.Instance.IsTop;

      // velocityも0にしておく
      fielderRigidbody.velocity = Vector3.zero;
      transform.position = fielderPositionVector[fielderPositionNumber - 1];
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float f) { }
  }
}