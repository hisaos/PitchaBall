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
        var label = this.gameObject.tag;
        if (label.Equals("Fair"))
        {
          BattingManager.Instance.SetJudgeText("ヒット");
        }
        if (label.Equals("Score"))
        {
          BattingManager.Instance.SetJudgeText("はいった");
          BattingManager.Instance.Score++;
        }
        if (label.Equals("Foul"))
        {
          BattingManager.Instance.SetJudgeText("ファール");
        }
        if (label.Equals("Strike"))
        {
          BattingManager.Instance.SetJudgeText("ストライク");
        }
        Destroy(other.gameObject);
      }

    }
  }
}