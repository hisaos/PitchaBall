using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Runner : MonoBehaviour, IRunnerMessageHandler
  {
    // 進塁先
    private int distinationBaseNumber;
    public int DistinationBaseNumber { get { return distinationBaseNumber; } set { distinationBaseNumber = value; } }

    // 今いる塁
    private int parkingBaseNumber;
    public int ParkingBaseNumber { get { return parkingBaseNumber; } set { parkingBaseNumber = value; } }

    // プレイ開始時にいた塁
    private int startingBaseNumber;
    public int StartingBaseNumber { get { return startingBaseNumber; } set { startingBaseNumber = value; } }

    // そこにボールを持った野手が入ったらフォースアウトになる塁
    private int forceOutBaseNumber;
    public int ForceOutBaseNumber { get { return forceOutBaseNumber; } set { forceOutBaseNumber = value; } }

    // そこにボールを持った野手が入ったらタッチアップのアウトになる塁
    private int touchupOutBaseNumber;
    public int TouchupOutBaseNumber { get { return touchupOutBaseNumber; } set { touchupOutBaseNumber = value; } }

    // フェア判定がされたかフラグ
    private bool isFair;

    // 走っているかフラグ
    private bool isRunning;
    public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }

    private bool isBatter;
    public bool IsBatter { get { return isBatter; } set { isBatter = value; } }

    private float minRunningDistance = 0.05f;

    private Rigidbody runnerRigidbody;
    public float runningSpeed = 10f;

    void Awake()
    {
      // 生成時は1塁（0）に向かい、1塁がフォースアウト
      distinationBaseNumber = 0;
      forceOutBaseNumber = 0;

      startingBaseNumber = -1;
      parkingBaseNumber = -1;
      touchupOutBaseNumber = -1;

      // Instantiateの時は必ずバッター
      isBatter = true;

      // フェアフラグ降ろして生成
      isFair = false;

      // 生成時は走っている
      isRunning = true;

      runnerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
      var dist = BattingManager.Instance.bases[distinationBaseNumber].position - transform.position;
      var dist2d = new Vector3(dist.x, 0f, dist.z);
      if (dist2d.sqrMagnitude >= minRunningDistance)
      {
        isRunning = true;
        runnerRigidbody.velocity = (dist2d.normalized * runningSpeed);
      }
      else
      {
        // Debug.Log("Reach: " + distinationBase);
        transform.position = BattingManager.Instance.bases[distinationBaseNumber].position + Vector3.up;
        runnerRigidbody.velocity = Vector3.zero;
        parkingBaseNumber = distinationBaseNumber;
        // Debug.Log("Now Parking: " + parkingBase);

        // タッチアップの対象に着いたらタッチアップ状態をリセット
        if (parkingBaseNumber == touchupOutBaseNumber) touchupOutBaseNumber = -1;

        // フォースアウトの対象に着いたらフォースアウト状態をリセット
        if (parkingBaseNumber == forceOutBaseNumber) forceOutBaseNumber = -1;

        isRunning = false;

        // フェア判定が出てる時にホームに付いたら得点
        if (isFair && parkingBaseNumber >= 3) HomeIn();
      }
    }

    private void HomeIn()
    {
      BattingManager.Instance.CountScore();
      Destroy(this.gameObject);
    }

    // 方向と進塁入力によって進塁する
    public void ProceedBase(int commandNum)
    {
      // 無効なコマンドをはじく
      if (commandNum < 0 || commandNum > 3) return;

      // ホームについてたらもう進まない
      if (parkingBaseNumber >= 0 && parkingBaseNumber < 3)
      {
        // 全員進塁
        if (commandNum >= 3) distinationBaseNumber = parkingBaseNumber + 1;
        // 選択進塁
        else if (commandNum == parkingBaseNumber) distinationBaseNumber = commandNum + 1;
      }
    }

    // 方向と帰塁入力によって帰塁する
    public void ReturnBase(int commandNum)
    {
      // ホームにいてもまだホームインしてないなら戻れる
      if (parkingBaseNumber > 0 && parkingBaseNumber <= 3)
      {
        // 全員帰塁
        if (commandNum >= 3) distinationBaseNumber = parkingBaseNumber;
        // 選択帰塁
        else if (commandNum == parkingBaseNumber) distinationBaseNumber = commandNum;
      }
    }

    // ランナーの状態をリセット
    public void ResetAtBat()
    {
      // エンタイトル・ホームランの後でホームを越えてたら得点にする
      if (startingBaseNumber > 2)
      {
        BattingManager.Instance.CountScore();
        Destroy(this.gameObject);
        return;
      }

      // フェア判定が付いていなくてバッターだったら消す
      if (isBatter && !isFair)
      {
        Destroy(this.gameObject);
        return;
      }

      // 塁の上にいてリセットを受けるならバッターで無くなる
      isBatter = false;
      // フェア判定が付いていたら到達していた塁に戻す
      if (isFair) startingBaseNumber = parkingBaseNumber;
      else parkingBaseNumber = distinationBaseNumber = startingBaseNumber;

      // タッチアップ状態を解除
      touchupOutBaseNumber = -1;

      // 使い終わったらフェアのフラグは降ろす
      isFair = false;

      // Debug.Log("Starting: " + startingBase + ", Parking: " + parkingBase);

      // 決まった塁に戻す
      transform.position = BattingManager.Instance.bases[startingBaseNumber].position + Vector3.up;
    }

    // アウトのメッセージ
    public void NotifyOut()
    {
      BattingManager.Instance.CountOut();
      BattingManager.Instance.SetJudgeText("アウト");
      Destroy(this.gameObject);
    }

    // タッチアウトのメッセージ
    public void NotifyTouchOut()
    {
      if (isRunning) NotifyOut();
      else BattingManager.Instance.SetJudgeText("セーフ");
    }

    // フライアウトのメッセージ
    public void NotifyFlyOut()
    {
      // フライでアウトになるのはバッターだけ
      if (isBatter) NotifyOut();
    }

    // フォースアウトのメッセージ
    public void NotifyForceOut(int baseNumber)
    {
      if (baseNumber == forceOutBaseNumber) NotifyOut();
    }

    // フォースアウトになる塁を通知される
    public void NotifyForceOutBaseNumber()
    {
      forceOutBaseNumber = RunnerManager.Instance.QueryChasingRunners(startingBaseNumber);
    }

    // タッチアップアウトのメッセージ
    public void NotifyTouchupOut(int baseNumber)
    {
      if (baseNumber == touchupOutBaseNumber) NotifyOut();
    }

    // タッチアップアウトになる塁を通知される
    public void NotifyTouchupOutBaseNumber()
    {
      touchupOutBaseNumber = startingBaseNumber;
    }

    // フェア判定のメッセージ
    public void NotifyFair()
    {
      isFair = true;
    }

    // フェア判定を取り消すメッセージ（エンタイトル処理用）
    public void DisnotifyFair()
    {
      isFair = false;
    }

  }
}
