using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  // FieldからMainに戻るときの制御
  private const float TIME_TO_RETURN_MAIN = 3f;
  private float _timeToReturnMain;

  private bool _inFieldRunnerRunning;
  private bool _inFieldBallMoving;

  public enum Mode
  {
    Main,
    Field,
    None
  }
  public Mode mode { get; private set; }
  [SerializeField] private Mode _mode;

  public CameraManager cameraManager;

  private void Awake()
  {
    if (!Instance) Instance = this;
    Instance.mode = Mode.Main;
    Instance._timeToReturnMain = TIME_TO_RETURN_MAIN;
    Instance._inFieldBallMoving = false;
    Instance._inFieldRunnerRunning = false;
  }

  private void Update()
  {
    if (Instance.mode == Mode.Main) return;

    if (!Instance._inFieldBallMoving && !Instance._inFieldRunnerRunning)
    {
      Instance._timeToReturnMain -= Time.deltaTime;
      if (Instance._timeToReturnMain <= 0f)
        SwitchModeTo(Mode.Main, 0f);
    }
    else
      Instance._timeToReturnMain = TIME_TO_RETURN_MAIN;
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
}
