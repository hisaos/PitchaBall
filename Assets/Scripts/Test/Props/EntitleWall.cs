using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test

{
  public class EntitleWall : MonoBehaviour
  {
    void OnTriggerEnter(Collider other)
    {
      // ボールがバウンドしていた時だけ仕事する
      if (!BattingManager.Instance.IsBallBounded) return;

      // ボールがフェアの時だけ仕事する
      if (!BattingManager.Instance.IsBallPlaying) return;

      if (other.CompareTag("Ball"))
      {
        BattingManager.Instance.SetJudgeText("エンタイトル");
        // エンタイトルは戻す
        BattingManager.Instance.TriggerReturn();
        BattingManager.Instance.ResetCount();
        RunnerManager.Instance.DisnotifyRunnersFair();
        BattingManager.Instance.ProceedRunnerEntitled(2);
      }
    }
  }
}