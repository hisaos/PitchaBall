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

    // 今いる塁（塁を離れている時は-1）
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

      runnerRigidbody = GetComponent<Rigidbody>();

      // Instantiateの時は必ずバッター
      isBatter = true;
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

        if (parkingBase >= 3) HomeIn();
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
      if (parkingBase >= 0)
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
      if (parkingBase > 0)
      {
        // 全員帰塁
        if (commandNum >= 3) distinationBase = parkingBase - 1;
        // 選択帰塁
        else if (commandNum == parkingBase) distinationBase = commandNum - 1;
      }
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

    // ランナーの状態をリセット
    public void ResetAtBat()
    {
      Debug.Log("Starting: " + startingBase + ", Parking: " + parkingBase);
      // 塁の上にいてリセットを受けるならバッターで無くなる
      isBatter = false;
      startingBase = parkingBase;
      transform.position = BattingManager.Instance.bases[startingBase].position + Vector3.up;
    }
  }
}
