using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class RunnerManager : MonoBehaviour
  {
    public static RunnerManager Instance;

    private InputActions inputActions;

    private Vector2 stickVector;
    private float runningDirectionFlux;
    private int runningCommandNum;

    private List<Runner> runners;
    public GameObject runnerPrefab;
    private bool isRunning;
    public bool IsRunning { get { return isRunning; } private set { isRunning = value; } }
    private bool isPlayer;

    void Awake()
    {
      if (Instance == null) Instance = this;

      inputActions = new InputActions();

      isPlayer = true;

      inputActions.Player.A.performed += (context) =>
      {
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.ReturnBase(runningCommandNum)
          );
        }
      };

      inputActions.Player.B.performed += (context) =>
      {
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.ProceedBase(runningCommandNum)
          );
        }
      };
    }

    void Update()
    {
      // ランナーの動いている状態を更新（どこか別の場所に置く）
      var runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      isRunning = false;
      foreach (var r in runners) isRunning |= r.IsRunning;
      Debug.Log("IsRunning: " + isRunning);

      stickVector = inputActions.Player.Move.ReadValue<Vector2>();

      if (isPlayer)
      {
        // 送球方向を決める
        var x = stickVector.x;
        var y = stickVector.y;

        if (x >= runningDirectionFlux)
        {
          runningCommandNum = 0;
        }
        else if (x >= -runningDirectionFlux)
        {
          if (y >= runningDirectionFlux) runningCommandNum = 1;
          if (y < -runningDirectionFlux) runningCommandNum = 3;
        }
        else
        {
          runningCommandNum = 2;
        }
      }

    }

    // 打者の位置にランナーを出す
    public void InstantiateRunner()
    {
      Instantiate(runnerPrefab, BattingManager.Instance.bases[3].position + Vector3.up, Quaternion.identity);
    }

    public void NotifyRunnerOut(int type, int baseNum)
    {
      var runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      if (type == 0)
      {
        // アウト
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => r.NotifyOut()
          );
        }
      }
      else if (type == 1)
      {
        // フライアウト
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => r.NotifyFlyOut()
          );
        }
      }
      else if (type == 2)
      {
        // フォースアウト
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => r.NotifyForceOut(baseNum)
          );
        }
      }
      else if (type == 3)
      {
        // タッチアップのアウト
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => r.NotifyTouchupOut(baseNum)
          );
        }
      }

    }

    public void ProceedAllRunners()
    {
      var runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => r.ProceedBase(3)
        );
      }
    }

    public void UpdateRunners()
    {
      runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => r.ResetAtBat()
        );
      }
    }

    public void NotifyRunnersFair()
    {
      runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => r.NotifyFair()
        );
      }
    }

  }
}