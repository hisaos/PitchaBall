using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public enum Mode
  {
    Batter,
    Catcher,
    Fielder,
    Pitcher,
    Runner,
    None
  };
  [SerializeField] private Mode _mode;

  private Batter _b;
  private Catcher _c;
  private Fielder _f;
  private Pitcher _p;
  private Runner _r;

  // Start is called before the first frame update
  private void Awake()
  {
    _b = this.GetComponent<Batter>();
    _c = this.GetComponent<Catcher>();
    _f = this.GetComponent<Fielder>();
    _p = this.GetComponent<Pitcher>();
    _r = this.GetComponent<Runner>();
  }

  private void Start()
  {
    ChangeMode(_mode);
  }

  public void ChangeMode(Mode m)
  {
    _mode = m;

    _b.enabled = false;
    _c.enabled = false;
    _f.enabled = false;
    _p.enabled = false;
    _r.enabled = false;

    switch (_mode)
    {
      case Mode.Batter:
        _b.enabled = true;
        break;
      case Mode.Catcher:
        _c.enabled = true;
        this.transform.tag = "Catcher";
        break;
      case Mode.Fielder:
        _f.enabled = true;
        break;
      case Mode.Pitcher:
        _p.enabled = true;
        this.transform.tag = "Pitcher";
        break;
      case Mode.Runner:
        _r.enabled = true;
        break;
    }
  }

  public void SetFielderHasBall(bool hasBall)
  {
    _f.hasBall = hasBall;
  }
}
