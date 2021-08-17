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

    // 判定UI
    public Text JudgeText;

    // 残り球数
    private int remain;
    public int Remain { get { return remain; } set { remain = value; } }
    public Text RemainText;

    // 1Pスコア
    private int score0;
    public int Score0 { get { return score0; } set { score0 = value; } }
    public Text ScoreText0;

    // 2Pスコア
    private int score1;
    public int Score1 { get { return score1; } set { score1 = value; } }
    public Text ScoreText1;

    // カウント
    private int ballCount;
    public int BallCount { get { return ballCount; } set { ballCount = value; } }
    public Text BallCountText;
    private int strikeCount;
    public int StrikeCount { get { return strikeCount; } set { strikeCount = value; } }
    public Text StrikeCountText;
    private int outCount;
    public int OutCount { get { return outCount; } set { outCount = value; } }
    public Text OutCountText;

    // Ballが地面にバウンドしたフラグ
    private bool isBallBounded;
    public bool IsBallBounded { get { return isBallBounded; } set { isBallBounded = value; } }

    // Batterがバットを振ったフラグ
    private bool isBatSwung;
    public bool IsBatSwung { get { return isBatSwung; } set { isBatSwung = value; } }

    // Pitcherが投げたフラグ
    private bool isPitched;
    public bool IsPitched { get { return isPitched; } set { isPitched = value; } }

    private GameObject pitcher;
    private GameObject batter;
    private List<Fielder> fielders;

    // 1Pの手番か2Pの手番か
    private bool isTop;
    public bool IsTop { get { return isTop; } set { isTop = value; } }

    // 塁
    public Transform[] bases = new Transform[4];

    void Start()
    {
      if (Instance == null) Instance = this;

      isTop = true;
      isPitched = false;
      isBatSwung = false;
      isBallBounded = false;

      remain = 10;
      score0 = 0;
      score1 = 0;
      outCount = 0;
      JudgeText.enabled = false;

      pitcher = FindObjectOfType<Pitcher>().gameObject;
      batter = FindObjectOfType<Batter>().gameObject;
      fielders = new List<Fielder>(FindObjectsOfType<Fielder>());

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

      if (outCount >= 3) ProceedInning();
      if (!IsInvoking(nameof(ResumeToStart))) Invoke(nameof(ResumeToStart), 3f);
    }

    public void CountScore()
    {
      if (isTop) score0++;
      else score1++;
    }

    // 回を進める処理
    void ProceedInning()
    {
      // アウトカウントリセット
      outCount = 0;

      // 裏なら残りを減らす
      if (!isTop) remain--;

      // 残り回数0になったら終了
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
      }
    }

    // ボールが接地して判定を表示した後に呼ばれて初期状態に戻す
    void ResumeToStart()
    {
      // 判定をけす
      JudgeText.enabled = false;

      // カウント表示更新
      BallCountText.text = "ボール：" + ballCount.ToString();
      StrikeCountText.text = "ストラ：" + strikeCount.ToString();
      OutCountText.text = "アウト：" + outCount.ToString();

      // ランナーの状態を更新
      RunnerManager.Instance.UpdateRunners();

      // ボールを片づける
      Destroy(GameObject.FindGameObjectWithTag("Ball"));

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

      // ボールのバウンド判定をリセット
      isBallBounded = false;

      // バットのスイング判定をリセット
      isBatSwung = false;

      // ピッチャーの投げた判定をリセット
      isPitched = false;

      // 野手の位置を元に戻して動かなくする
      foreach (var f in fielders)
      {
        ExecuteEvents.Execute<IFielderMessageHandler>(
          target: f.gameObject,
          eventData: null,
          functor: (receiver, eventData) =>
          {
            receiver.DisableFielderMove();
            receiver.ReturnToOriginalPosition();
          }
        );
      }

    }

    public void CountStrike(bool isFoul)
    {
      strikeCount++;
      if (strikeCount >= 3)
      {
        if (isFoul)
        {
          strikeCount = 2;
        }
        else
        {
          CountOut();
          SetJudgeText("アウト");
        }
      }
    }

    public void CountBall()
    {
      ballCount++;
      if (ballCount >= 4)
      {
        // CountBase();
        RunnerManager.Instance.InstantiateRunner();
        RunnerManager.Instance.ProceedAllRunners();
        ResetCount();
        SetJudgeText("フォアボール");
      }
    }

    public void ResetCount()
    {
      strikeCount = 0;
      ballCount = 0;
    }

    public void CountOut()
    {
      ResetCount();
      outCount++;
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