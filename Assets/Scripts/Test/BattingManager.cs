using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Test
{
  public class BattingManager : MonoBehaviour
  {
    public static BattingManager Instance;

    private int remain;
    public int Remain { get; set; }
    public Text RemainText;

    private int score;
    public int Score { get { return score; } set { score = value; } }
    public Text ScoreText;

    public Text JudgeText;

    private bool isBallHit;
    public bool IsBallHit { get; set; }

    private GameObject pitcher;

    // Start is called before the first frame update
    void Start()
    {
      if (Instance == null) Instance = this;
      remain = 10;
      score = 0;
      JudgeText.enabled = false;
      pitcher = FindObjectOfType<Pitcher>().gameObject;
    }

    void Update()
    {
      RemainText.text = "のこり：" + remain.ToString();
      ScoreText.text = "スコア：" + score.ToString();
    }

    public void SetJudgeText(string judge)
    {
      JudgeText.text = judge;
      JudgeText.enabled = true;

      Invoke(nameof(ResumeToStart), 3f);
    }

    // ボールが接地して判定を表示した後に呼ばれて初期状態に戻す
    void ResumeToStart()
    {
      remain--;
      if (remain <= 0)
      {
        JudgeText.text = "終了";
      }
      else
      {
        JudgeText.enabled = false;

        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<IPitcherMessageHandler>(
          target: pitcher,
          eventData: null,
          functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // カメラをMainCameraに切り替え
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(true, null)
        );
      }
    }
  }
}