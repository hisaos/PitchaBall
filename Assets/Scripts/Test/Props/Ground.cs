using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Ground : MonoBehaviour
  {
    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // 判定済みなら何もしない
        if (BattingManager.Instance.IsBallBounded) return;

        // グラウンドにボールが触れた時点でバウンド判定
        BattingManager.Instance.IsBallBounded = true;

        var label = this.gameObject.tag;
        if (label.Equals("Fair"))
        {
          BattingManager.Instance.SetJudgeText("ヒット");
          BattingManager.Instance.ResetCount();
          RunnerManager.Instance.NotifyRunnersFair();
        }
        if (label.Equals("Score"))
        {
          BattingManager.Instance.SetJudgeText("はいった");
          // ホームランは戻す
          BattingManager.Instance.TriggerReturn();
          BattingManager.Instance.ResetCount();
          RunnerManager.Instance.NotifyRunnersFair();
        }
        if (label.Equals("Foul"))
        {
          BattingManager.Instance.SetJudgeText("ファール");
          // ファールは戻す
          BattingManager.Instance.TriggerReturn();
          BattingManager.Instance.CountStrike(true);
        }
        if (label.Equals("Strike"))
        {
          if (BattingManager.Instance.IsBallHit)
          {
            // ボールにかすってたらファール
            BattingManager.Instance.SetJudgeText("ファール");
            // ファールは戻す
            BattingManager.Instance.TriggerReturn();
            BattingManager.Instance.CountStrike(true);
          }
          else
          {
            BattingManager.Instance.SetJudgeText("ストライク");
            // ストライクは戻す
            BattingManager.Instance.TriggerReturn();
            BattingManager.Instance.CountStrike(false);
            Destroy(other.gameObject);
          }
        }
        if (label.Equals("Far"))
        {
          if (BattingManager.Instance.IsBallHit)
          {
            // ボールにかすってたらファール
            BattingManager.Instance.SetJudgeText("ファール");
            // ファールは戻す
            BattingManager.Instance.TriggerReturn();
            BattingManager.Instance.CountStrike(true);
          }
          else
          {
            if (BattingManager.Instance.IsBatSwung)
            {
              BattingManager.Instance.SetJudgeText("ストライク");
              // ストライクは戻す
              BattingManager.Instance.TriggerReturn();
              BattingManager.Instance.CountStrike(false);
            }
            else
            {
              BattingManager.Instance.SetJudgeText("ボール");
              // ボールは戻す
              BattingManager.Instance.TriggerReturn();
              BattingManager.Instance.CountBall();
            }
          }

          Destroy(other.gameObject);
        }
      }

    }
  }
}