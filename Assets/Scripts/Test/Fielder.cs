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

    // 守備位置番号(1-9)
    public int fielderPositionNumber;

    // 守備位置ベクトル(1-9に対応)
    public List<Vector3> fielderPositionVector;

    void Awake()
    {
      isPlayer = true;
      isMoving = false;
      inputActions = new InputActions();
      fielderRigidbody = GetComponent<Rigidbody>();
      fielderPositionVector = new List<Vector3>{
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 1f, 8f),
        new Vector3(-6f, 1f, 0f),
        new Vector3(0f, 1f, -1f),
        new Vector3(6f, 1f, 0f),
        new Vector3(3f, 1f, -0.5f),
        new Vector3(6f, 1f, -2f),
        new Vector3(0f, 1f, -4f),
        new Vector3(-6f, 1f, -2f),
      };
    }

    void Update()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
      if (isMoving && isPlayer) fielderRigidbody.velocity = new Vector3(-stickVector.x, 0f, -stickVector.y) * moveSpeed;
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
    public void SetFielderBall(GameObject g) {}

    // ボールが打たれた時のメッセージハンドラ
    public void EnableFielderMove()
    {
      isMoving = true;
    }

    // ボールが消えた時のメッセージハンドラ
    public void ResetFielderBall() {}

    // 追わなくなる時のメッセージハンドラ
    public void DisableFielderMove()
    {
      isMoving = false;
    }

    // 元の位置に戻る
    public void ReturnToOriginalPosition()
    {
      transform.position = fielderPositionVector[fielderPositionNumber - 1];
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float f) {}
  }
}