using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class Ground : MonoBehaviour
  {
    void OnCollisionEnter(Collision other)
    {
      BattingManager.Instance.IsBallBounded = true;
      if (other.gameObject.CompareTag("Ball"))
      {
        var label = this.gameObject.tag;
        if (label.Equals("Fair"))
        {
          BattingManager.Instance.SetJudgeText("ヒット");
        }
        if (label.Equals("Score"))
        {
          BattingManager.Instance.SetJudgeText("はいった");
          BattingManager.Instance.CountScore();
        }
        if (label.Equals("Foul"))
        {
          BattingManager.Instance.SetJudgeText("ファール");
        }
        if (label.Equals("Strike"))
        {
          BattingManager.Instance.SetJudgeText("ストライク");
        }
        if (label.Equals("Far"))
        {
          if (BattingManager.Instance.IsBatSwung) BattingManager.Instance.SetJudgeText("ストライク");
          else BattingManager.Instance.SetJudgeText("ボール");
        }
        Destroy(other.gameObject);
      }

    }
  }
}