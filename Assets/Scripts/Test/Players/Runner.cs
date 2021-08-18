using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Runner : MonoBehaviour, IRunnerMessageHandler
  {
    // 進塁先
    private int distinationBase;
    public int DistinationBase { get { return distinationBase; } set { distinationBase = value; } }

    // 今いる塁
    private int parkingBase;
    public int ParkingBase { get { return parkingBase; } set { parkingBase = value; } }

    // プレイ開始時にいた塁
    private int startingBase;
    public int StartingBase { get { return startingBase; } set { startingBase = value; } }

    // そこにボールを持った野手が入ったらフォースアウトになる塁
    private int forceOutBaseNumber;
    public int ForceOutBase { get { return forceOutBaseNumber; } set { forceOutBaseNumber = value; } }

    // そこにボールを持った野手が入ったらタッチアップのアウトになる塁
    private int touchupOutBaseNumber;
    public int TouchupOutBase { get { return touchupOutBaseNumber; } set { touchupOutBaseNumber = value; } }

    // フェア判定がされたかフラグ
    private bool isFair;

    private bool isBatter;
    private float minRunningDistance = 0.05f;

    private Rigidbody runnerRigidbody;
    public float runningSpeed = 10f;

    void Awake()
    {
      distinationBase = 0;
      startingBase = -1;
      parkingBase = -1;
      forceOutBaseNumber = -1;
      touchupOutBaseNumber = -1;

      // Instantiateの時は必ずバッター
      isBatter = true;

      // フェアフラグ降ろして生成
      isFair = false;

      runnerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
      var dist = BattingManager.Instance.bases[distinationBase].position - transform.position;
      var dist2d = new Vector3(dist.x, 0f, dist.z);
      if (dist2d.sqrMagnitude >= minRunningDistance)
      {
        runnerRigidbody.velocity = (dist2d.normalized * runningSpeed);
      }
      else
      {
        Debug.Log("Reach: " + distinationBase);
        transform.position = BattingManager.Instance.bases[distinationBase].position + Vector3.up;
        runnerRigidbody.velocity = Vector3.zero;
        parkingBase = distinationBase;
        Debug.Log("Now Parking: " + parkingBase);

        // フェア判定が出てる時にホームに付いたら得点
        if (isFair && parkingBase >= 3) HomeIn();
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
      // ホームについてたらもう進まない
      if (parkingBase >= 0 && parkingBase < 3)
      {
        // 全員進塁
        if (commandNum >= 3) distinationBase = parkingBase + 1;
        // 選択進塁
        else if (commandNum == parkingBase) distinationBase = commandNum + 1;
      }
    }

    // 方向と帰塁入力によって帰塁する
    public void ReturnBase(int commandNum)
    {
      // ホームにいてもまだホームインしてないなら戻れる
      if (parkingBase > 0 && parkingBase <= 3)
      {
        // 全員帰塁
        if (commandNum >= 3) distinationBase = parkingBase - 1;
        // 選択帰塁
        else if (commandNum == parkingBase) distinationBase = commandNum - 1;
      }
    }

    // ランナーの状態をリセット
    public void ResetAtBat()
    {
      // フェア判定が付いていなくてバッターだったら消す
      if (isBatter && !isFair)
      {
        Destroy(this.gameObject);
        return;
      }

      // 塁の上にいてリセットを受けるならバッターで無くなる
      isBatter = false;

      // フェア判定が付いていたら到達していた塁に戻す
      if (isFair) startingBase = parkingBase;
      else parkingBase = distinationBase = startingBase;

      // 使い終わったらフェアのフラグは降ろす
      isFair = false;

      // Debug.Log("Starting: " + startingBase + ", Parking: " + parkingBase);

      // 決まった塁に戻す
      transform.position = BattingManager.Instance.bases[startingBase].position + Vector3.up;
    }

    // アウトのメッセージ
    public void NotifyOut()
    {
      BattingManager.Instance.CountOut();
      BattingManager.Instance.SetJudgeText("アウト");
      Destroy(this.gameObject);
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

    // タッチアップアウトのメッセージ
    public void NotifyTouchupOut(int baseNumber)
    {
      if (baseNumber == touchupOutBaseNumber) NotifyOut();
    }

    // フェア判定のメッセージ
    public void NotifyFair()
    {
      isFair = true;
    }

  }
}
