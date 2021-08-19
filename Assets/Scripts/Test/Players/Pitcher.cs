using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test

{
  public class Pitcher : MonoBehaviour, IPitcherMessageHandler
  {
    // 入力管理
    private InputActions inputActions;
    private Vector2 stickVector;

    // 手動・自動フラグ
    private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } set { isPlayer = value; } }

    // 投球関連
    public GameObject ball;
    private float pitchInterval;
    public float PitchInterval = 1.5f;
    public float pitchForwardForce = 0.8f;
    public float pitchUpForce = 0.4f;

    // マネージャークラスが同じようなフラグ持ってる。まとめたい
    private bool isPitched = false;

    // 左右変化させるためだけに持ってるらしい。要らんかも
    private Rigidbody pitchedBallRigidbody;
    // CPU用の左右変化量
    private float ballWindingVector;

    // キャラクターの管理
    private List<Fielder> fielders;
    private Batter batter;

    void Awake()
    {
      inputActions = new InputActions();

      inputActions.Player.A.performed += (context) =>
      {
        // Debug.Log(this.name + ": A Press");
        if (isPlayer && !isPitched) ExecutePitch();
      };
    }

    void Start()
    {
      isPlayer = false;
      pitchInterval = PitchInterval;
      batter = FindObjectOfType<Batter>();
      fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    // Update is called once per frame
    void Update()
    {
      stickVector = inputActions.Player.Move.ReadValue<Vector2>();

      if (isPitched) return;

      // CPU側
      // ボールを投げる（CPU側のトリガー）
      if (!isPlayer)
      {
        pitchInterval -= Time.deltaTime;
        if (pitchInterval < 0f)
        {
          ExecutePitch();
        }
      }
    }

    void FixedUpdate()
    {
      // ボールの左右変化を付ける
      if (pitchedBallRigidbody) pitchedBallRigidbody.AddForce(Vector3.right * -ballWindingVector, ForceMode.Force);
    }

    private void ExecutePitch()
    {
      var b = Instantiate(ball);
      b.transform.localPosition = this.transform.position + Vector3.forward;
      pitchInterval = PitchInterval;
      pitchedBallRigidbody = b.GetComponent<Rigidbody>();
      pitchedBallRigidbody.useGravity = false;

      // 速さ調整
      var pf = pitchForwardForce;
      if (isPlayer)
      {
        // プレイヤー用
        // 上下キーで速さ調整
        if (stickVector.y > 0f) pf *= 0.7f;
        else if (stickVector.y < 0f) pf *= 1.3f;

        // 左右変化を右か左で投げるとき決める。途中で変化させない。
        if (stickVector.x > 0f) ballWindingVector = 0.3f;
        else if (stickVector.x < 0f) ballWindingVector = -0.3f;
        else ballWindingVector = 0f;
      }
      else
      {
        // CPU用
        // サイコロを振って速さ調整
        var d0 = Random.Range(-1, 2);
        if (d0 == 1) pf *= 0.7f;
        else if (d0 == -1) pf *= 1.3f;

        // 左右変化もサイコロを振って調整
        var d1 = Random.Range(-1, 2);
        if (d1 == 1) ballWindingVector = 0.3f;
        else if (d1 == -1) ballWindingVector = -0.3f;
        else ballWindingVector = 0f;

        // Debug.Log("Ball pattern: " + d0.ToString() + "," + d1.ToString());
      }

      pitchedBallRigidbody.AddForce(Vector3.forward * pf, ForceMode.Impulse);
      isPitched = true;
      BattingManager.Instance.IsPitched = true;

      ExecuteEvents.Execute<IBatterMessageHandler>(
        target: batter.gameObject,
        eventData: null,
        functor: (receiver, eventData) => receiver.NotifyBallThrown(b.gameObject)
      );
    }

    public void EnablePitch()
    {
      // Debug.Log(this.gameObject.name + ": EnablePitch");
      isPlayer = !BattingManager.Instance.IsTop;
      isPitched = false;
    }

    void OnEnable()
    {
      inputActions.Player.Enable();
    }

    void OnDisable()
    {
      inputActions.Player.Disable();
    }
  }
}