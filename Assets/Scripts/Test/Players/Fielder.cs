using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    // 塁カバー関連
    public bool isCoveringBase;
    private int coveringBaseNumber;
    private float minCoverDistance = 0.05f;

    // 打球オブジェクト
    private GameObject targetBall;

    // 投げるボールオブジェクト
    public GameObject ball;

    // 捕球しているか
    public bool hasBall;

    // 送球方向選択時のあそび
    public float throwDirectionFlux = 0.2f;

    // 送球スピード
    public float throwForce = 0.8f;

    // 送球方向（ボールを持って歩く時も共用）
    private int throwDirection;

    // 守備位置、塁位置関連
    // 守備位置番号(1-9)
    public int fielderPositionNumber;

    // 守備位置ベクトル(1-9に対応)
    public List<Vector3> fielderPositionVector;

    // 塁位置ベクトル(0-3に対応)
    public List<Vector3> baseCoverPositionVector;

    // 野手全員のリスト
    private List<Fielder> fielders;

    void Awake()
    {
      inputActions = new InputActions();

      inputActions.Player.A.performed += (context) =>
      {
        if (isPlayer && hasBall) ExecuteThrow();
      };

      inputActions.Player.B.performed += (context) =>
      {
        if (isPlayer && hasBall)
        {
          isCoveringBase = true;
          coveringBaseNumber = throwDirection;
        }
      };

      isPlayer = false;
      isMoving = false;
      isCoveringBase = false;

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
        new Vector3(-10f, 1f, -12f)
      };
      baseCoverPositionVector = new List<Vector3>{
        new Vector3(-8f, 1f, 0f),
        new Vector3(0f, 1f, -8f),
        new Vector3(8f, 1f, 0f),
        new Vector3(0f, 1f, 8f)
      };
    }

    void Start()
    {
      fielders = new List<Fielder>(GameObject.FindObjectsOfType<Fielder>());
    }

    void Update()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();

      // 送球方向をアップデート
      UpdateThrowDirection();
    }

    void FixedUpdate()
    {
      if (isMoving)
      {
        if (isCoveringBase)
        {
          // 塁カバー時
          // Debug.Log(this.name + " covering: " + coveringBaseNumber); 
          var direction = baseCoverPositionVector[coveringBaseNumber] - transform.position;
          if (direction.sqrMagnitude > minCoverDistance)
          {
            var d = direction.normalized;
            fielderRigidbody.velocity = new Vector3(d.x, 0f, d.z) * moveSpeed;
          }
          else
          {
            // ある程度カバーする塁に近づいていたら塁にスナップして止める
            transform.position = baseCoverPositionVector[coveringBaseNumber];
            fielderRigidbody.velocity = Vector3.zero;
          }
        }
        else
        {
          // 通常移動時
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
    }

    // 送球・塁カバーに歩いていく方向を決める
    private void UpdateThrowDirection()
    {
      var x = stickVector.x;
      var y = stickVector.y;
      throwDirection = 0;

      if (x >= throwDirectionFlux)
      {
        throwDirection = 0;
      }
      else if (x >= -throwDirectionFlux)
      {
        if (y >= throwDirectionFlux) throwDirection = 1;
        if (y < -throwDirectionFlux) throwDirection = 3;
      }
      else
      {
        throwDirection = 2;
      }
    }

    private void ExecuteThrow()
    {
      var b = Instantiate(ball);
      hasBall = false;

      if (isPlayer)
      {
        // ボールを飛ばす方向を決める
        var directionVector = (baseCoverPositionVector[throwDirection] - transform.position).normalized;
        b.transform.localPosition = this.transform.position + directionVector;
        b.GetComponent<Rigidbody>().AddForce(directionVector * throwForce, ForceMode.Impulse);

        // FieldCameraの追跡対象を投げたボールにする
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(false, b.transform)
        );

        // ボールインプレイのフラグを立てる
        BattingManager.Instance.IsBallPlaying = true;
      }
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールインプレイのフラグを降ろす
        BattingManager.Instance.IsBallPlaying = false;

        // ボールを持っているフラグを建てる
        hasBall = true;

        // 打球を消す
        foreach (var f in fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.ResetFielderBall()
          );
        }

        if (!BattingManager.Instance.IsBallBounded)
        {
          // フライアウト
          // バウンド判定つける
          BattingManager.Instance.IsBallBounded = true;

          // アウトの処理
          RunnerManager.Instance.NotifyRunnerOut(1, -1);
        }

        // FieldCameraの追跡対象をこのFielderにする
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(false, this.transform)
        );

        Destroy(other.gameObject);
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
      isCoveringBase = false;

      // velocityも0にしておく
      fielderRigidbody.velocity = Vector3.zero;
    }

    public void NotifyBallAngle(float angle)
    {
      if (fielderPositionNumber < 3 || fielderPositionNumber > 6) return;
      else
      {
        if (angle >= 315f)
        {
          // Debug.Log(this.name + " got: 1");
          if (fielderPositionNumber == 3) { coveringBaseNumber = 0; isCoveringBase = true; }
          if (fielderPositionNumber == 4) { coveringBaseNumber = 1; isCoveringBase = true; }
          if (fielderPositionNumber == 5) { coveringBaseNumber = 2; isCoveringBase = true; }
        }
        else if (angle >= 270f)
        {
          // Debug.Log(this.name + " got: 0");
          if (fielderPositionNumber == 3) { coveringBaseNumber = 0; isCoveringBase = true; }
          if (fielderPositionNumber == 4) { coveringBaseNumber = 1; isCoveringBase = true; }
          if (fielderPositionNumber == 6) { coveringBaseNumber = 2; isCoveringBase = true; }
        }
        else if (angle >= 45f)
        {
          // Debug.Log(this.name + " got: 3");
          if (fielderPositionNumber == 4) { coveringBaseNumber = 0; isCoveringBase = true; }
          if (fielderPositionNumber == 6) { coveringBaseNumber = 1; isCoveringBase = true; }
          if (fielderPositionNumber == 5) { coveringBaseNumber = 2; isCoveringBase = true; }
        }
        else
        {
          // Debug.Log(this.name + " got: 2");
          if (fielderPositionNumber == 3) { coveringBaseNumber = 0; isCoveringBase = true; }
          if (fielderPositionNumber == 6) { coveringBaseNumber = 1; isCoveringBase = true; }
          if (fielderPositionNumber == 5) { coveringBaseNumber = 2; isCoveringBase = true; }
        }
      }
    }

    // 元の位置に戻る
    public void ReturnToOriginalPosition()
    {
      // 攻撃している側に応じてisPlayerを切り替える
      isPlayer = !BattingManager.Instance.IsTop;

      // ボールもここで離す
      hasBall = false;

      transform.position = fielderPositionVector[fielderPositionNumber - 1];
    }

    // ボールまでの距離を教え合うメッセージハンドラ
    public void TellDistanceToBall(float f) { }
  }
}