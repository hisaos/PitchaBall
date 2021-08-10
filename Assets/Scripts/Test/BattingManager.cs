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

    // 残り球数
    private int remain;
    public int Remain { get; set; }
    public Text RemainText;

    // 1Pスコア
    private int score0;
    public int Score0 { get { return score0; } set { score0 = value; } }
    public Text ScoreText0;

    // 2Pスコア
    private int score1;
    public int Score1 { get { return score1; } set { score1 = value; } }
    public Text ScoreText1;

    // 判定UI
    public Text JudgeText;

    private bool isBallHit;
    public bool IsBallHit { get; set; }

    private GameObject pitcher;
    private GameObject batter;

    // 1Pの手番か2Pの手番か
    private bool isTop;

    // Start is called before the first frame update
    void Start()
    {
      if (Instance == null) Instance = this;
      isTop = true;
      remain = 10;
      score0 = 0;
      score1 = 0;
      JudgeText.enabled = false;
      pitcher = FindObjectOfType<Pitcher>().gameObject;
      batter = FindObjectOfType<Batter>().gameObject;

      ScoreText0.color = Color.red;
      ScoreText1.color = Color.white;
      StartCoroutine(ShowWhoIsAtBat("1Pのこうげき"));
    }

    void Update()
    {
      RemainText.text = "のこり：" + remain.ToString();
      ScoreText0.text = "1P：" + score0.ToString();
      ScoreText1.text = "2P：" + score1.ToString();
    }

    public void SetJudgeText(string judge)
    {
      JudgeText.text = judge;
      JudgeText.enabled = true;

      Invoke(nameof(ResumeToStart), 3f);
    }

    public void CountScore()
    {
      if (isTop) score0++;
      else score1++;
    }
    // ボールが接地して判定を表示した後に呼ばれて初期状態に戻す
    void ResumeToStart()
    {
      if (!isTop) remain--;

      if (remain <= 0)
      {
        JudgeText.text = "終了";
      }
      else
      {
        // 次の攻撃をするプレイヤーの表示
        if (isTop)
        {
          ScoreText0.color = Color.white;
          ScoreText1.color = Color.red;
          StartCoroutine(ShowWhoIsAtBat("2Pのこうげき"));
        }
        else
        {
          ScoreText0.color = Color.red;
          ScoreText1.color = Color.white;
          StartCoroutine(ShowWhoIsAtBat("1Pのこうげき"));
        }
        isTop = !isTop;

        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<IPitcherMessageHandler>(
          target: pitcher,
          eventData: null,
          functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // バッターをリフレッシュ
        ExecuteEvents.Execute<IBatterMessageHandler>(
          target: batter,
          eventData: null,
          functor: (receiver, eventData) => receiver.EnableBatter()
        );

        // カメラをMainCameraに切り替え
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(true, null)
        );
      }
    }

    IEnumerator ShowWhoIsAtBat(string text)
    {
      JudgeText.enabled = true;
      JudgeText.text = text;
      yield return new WaitForSeconds(1f);
      JudgeText.enabled = false;
    }
  }
}