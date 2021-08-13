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
          BattingManager.Instance.CountBase();
        }
        if (label.Equals("Score"))
        {
          BattingManager.Instance.SetJudgeText("はいった");
          BattingManager.Instance.CountBase();
          for (int i = 0; i < BattingManager.Instance.BaseCount; i ++) BattingManager.Instance.CountScore();
          BattingManager.Instance.BaseCount = 0;
        }
        if (label.Equals("Foul"))
        {
          BattingManager.Instance.SetJudgeText("ファール");
        }
        if (label.Equals("Strike"))
        {
          BattingManager.Instance.SetJudgeText("ストライク");
          Destroy(other.gameObject);
        }
        if (label.Equals("Far"))
        {
          if (BattingManager.Instance.IsBatSwung) BattingManager.Instance.SetJudgeText("ストライク");
          else BattingManager.Instance.SetJudgeText("ボール");
          Destroy(other.gameObject);
        }
      }

    }
  }
}