using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  private InputActions _ia;

  // FieldからMainに戻るときの制御
  private const float TIME_TO_RETURN_MAIN = 3f;
  private float _timeToReturnMain;

  private bool _inFieldRunnerRunning;
  private bool _inFieldBallMoving;

  private bool _isGameOver;

  public enum Mode
  {
    Title,
    Settings,
    Main,
    Field,
    Pause,
    Result,
    Gameover,
    None
  }
  public Mode mode { get; private set; }

  public GameObject[] huds;
  public GameObject ballSpawner;
  private GameObject _ball;

  private void Awake()
  {
    if (!Instance) Instance = this;
    mode = Mode.Title;
    _timeToReturnMain = TIME_TO_RETURN_MAIN;
    _inFieldBallMoving = false;
    _inFieldRunnerRunning = false;
    _isGameOver = false;

    foreach (var h in huds)
    {
      h.SetActive(false);
    }

    _ia = new InputActions();

    // とりあえずAボタンでモードを切り替えていく
    // メニューが出てる時以外はGameManagerで動かすものはない
    _ia.Player.A.performed += (context) =>
    {
      huds[(int)Instance.mode].SetActive(false);
      ballSpawner.SetActive(false);

      // モードの遷移図通りに実装
      if (Instance.mode == Mode.Title)
        Instance.mode = Mode.Settings;
      else if (Instance.mode == Mode.Settings)
        Instance.mode = Mode.Main;
      else if (Instance.mode == Mode.Main)
      {
        Instance.mode = Mode.Result;
        _isGameOver = true;
      }
      else if (Instance.mode == Mode.Result)
      {
        if (Instance._isGameOver)
          Instance.mode = Mode.Gameover;
        else
          Instance.mode = Mode.Main;
      }
      else if (Instance.mode == Mode.Gameover)
        Instance.mode = Mode.Title;
    };
    // Mainにいる時にPauseに入る
    _ia.Player.B.performed += (context) =>
    {
      huds[(int)Instance.mode].SetActive(false);

      if (Instance.mode == Mode.Main)
        Instance.mode = Mode.Pause;
      else if (Instance.mode == Mode.Pause)
        Instance.mode = Mode.Main;
    };
    _ia.Player.Y.performed += (context) =>
    {
      huds[(int)Instance.mode].SetActive(false);
      ballSpawner.SetActive(false);

      if (Instance.mode == Mode.Main)
      {
        Instance.mode = Mode.Field;
        ballSpawner.SetActive(true);
        _ball = GameObject.FindGameObjectWithTag("Ball");
      }
      else if (Instance.mode == Mode.Field)
      {
        if (_ball) Destroy(_ball);
        var ps = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in ps)
          p.GetComponent<Player>().ReturnToInitialPosition();
        Instance.mode = Mode.Main;
      }
    };
  }

  private void Update()
  {
    huds[(int)Instance.mode].SetActive(true);

    switch (Instance.mode)
    {
      case Mode.Title:
        break;
      case Mode.Settings:
        break;
      case Mode.Main:
        return;
      case Mode.Field:
        /*
        if (!Instance._inFieldBallMoving && !Instance._inFieldRunnerRunning)
        {
          Instance._timeToReturnMain -= Time.deltaTime;
          if (Instance._timeToReturnMain <= 0f)
            SwitchModeTo(Mode.Main, 0f);
        }
        else
          Instance._timeToReturnMain = TIME_TO_RETURN_MAIN;
        */
        break;
      case Mode.Pause:
        break;
      case Mode.Result:
        break;
      case Mode.Gameover:
        break;
    }
  }

  public void SwitchModeTo(Mode m, float t)
  {
    StartCoroutine(_SwitchModeTo(m, t));
  }

  private IEnumerator _SwitchModeTo(Mode m, float t)
  {
    yield return new WaitForSeconds(t);
    if (m == Mode.Field)
    {
      Instance._timeToReturnMain = TIME_TO_RETURN_MAIN;
      var ps = GameObject.FindGameObjectsWithTag("Player");
      foreach (var p in ps)
      {
        var _p = p.GetComponent<Player>();
        _p.ChangeMode(Player.Mode.Fielder);
      }
      var c = GameObject.FindGameObjectWithTag("Catcher");
      var catcherCatcher = c.GetComponent<Catcher>();
      catcherCatcher.StopMonitorBall();
      var catcherPlayer = c.GetComponent<Player>();
      catcherPlayer.ChangeMode(Player.Mode.Fielder);
    }
    else if (m == Mode.Main)
    {
      var ps = GameObject.FindGameObjectsWithTag("Player");
      foreach (var p in ps)
      {
        var _p = p.GetComponent<Player>();
        _p.SetFielderHasBall(false);
      }
      // PitcherとCatcherをそれぞれのモードに復帰させて位置を戻す
      var catcher = GameObject.FindGameObjectWithTag("Catcher");
      var catcherPlayer = catcher.GetComponent<Player>();
      catcherPlayer.SetFielderHasBall(false);
      catcherPlayer.ChangeMode(Player.Mode.Catcher);
      var catcherCatcher = catcher.GetComponent<Catcher>();
      catcherCatcher.ReturnToHome();

      var pitcher = GameObject.FindGameObjectWithTag("Pitcher");
      var pitcherPlayer = pitcher.GetComponent<Player>();
      pitcherPlayer.SetFielderHasBall(false);
      pitcherPlayer.ChangeMode(Player.Mode.Pitcher);
      var pitcherPitcher = pitcher.GetComponent<Pitcher>();
      pitcherPitcher.ReturnToHome();
    }

    Instance.mode = m;
  }

  private void OnEnable()
  {
    _ia.Player.Enable();
  }

  private void OnDisable()
  {
    _ia.Player.Disable();
  }

}
