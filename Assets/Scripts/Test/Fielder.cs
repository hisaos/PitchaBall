using UnityEngine;

namespace Test
{
  public class Fielder : MonoBehaviour
  {
    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        if (!BattingManager.Instance.IsBallBounded)
        {
          // フライアウト
          BattingManager.Instance.SetJudgeText("アウト");
          Destroy(other.gameObject);
        }
      }
    }
  }
}