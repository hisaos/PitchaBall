using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

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
      var ps = GameObject.FindGameObjectsWithTag("Player");
      foreach (var p in ps)
      {
        var _p = p.GetComponent<Player>();
        _p.ChangeMode(Player.Mode.Fielder);
      }
      var c = GameObject.FindGameObjectWithTag("Catcher");
      var _c = c.GetComponent<Player>();
      _c.ChangeMode(Player.Mode.Fielder);
    }
    else if (m == Mode.Main)
    {
      var c = GameObject.FindGameObjectWithTag("Catcher");
      var _p = c.GetComponent<Player>();
      var _c = c.GetComponent<Catcher>();
      _p.ChangeMode(Player.Mode.Catcher);
      _c.ReturnToHome();
    }
    Instance.mode = m;

  }
}
