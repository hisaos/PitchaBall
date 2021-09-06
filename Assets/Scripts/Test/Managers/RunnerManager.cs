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
    private const float runningDirectionFlux = 0.2f;
    private int runningCommandNum;

    private List<Runner> runners;

    // エンタイトル処理用のキャッシュ
    // 投球時のランナーのキャッシュ
    private List<Runner> runnersAtPitch;
    // 打者ランナーのキャッシュ
    private Runner runnerAtBat;

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
      // ランナーの動いている状態を更新
      // Todo: runnersはランナーの状態が変わった時だけ取得してソートして持っておく
      runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      runners.Sort((a, b) => b.StartingBaseNumber - a.StartingBaseNumber);

      isRunning = false;
      foreach (var r in runners) isRunning |= r.IsRunning;

      stickVector = inputActions.Player.Move.ReadValue<Vector2>();

      if (isPlayer)
      {
        // 方向を決める
        var x = stickVector.x;
        var y = stickVector.y;

        if (x >= runningDirectionFlux) runningCommandNum = 0;
        else if (x <= -runningDirectionFlux) runningCommandNum = 2;
        else if (y >= runningDirectionFlux) runningCommandNum = 1;
        else if (y <= -runningDirectionFlux) runningCommandNum = 3;
        else runningCommandNum = -1;

        // Debug.Log("x: " + x + " y: " + y + " rc: " + runningCommandNum);
      }

    }

    // 打者の位置にランナーを出す
    public void InstantiateRunner()
    {
      var r = Instantiate(runnerPrefab, BattingManager.Instance.bases[3].position + Vector3.up, Quaternion.identity);
      runnerAtBat = r.GetComponent<Runner>();
    }

    public void ClearRunner()
    {
      foreach (var r in runners)
      {
        Destroy(r.gameObject);
      }
    }

    // ランナーにアウトを通知（多分後で消す）
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

    // デッドボール・四球で進塁させる用
    public void ProceedRunnersAtBat()
    {
      var runners = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
      runners.Sort((a, b) => a.ParkingBaseNumber - b.ParkingBaseNumber);
      var index = -1;
      foreach (var r in runners)
      {
        // Debug.Log("runner parking: " + r.ParkingBaseNumber);
        if (r.ParkingBaseNumber == index)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => r.ProceedBase(3)
          );
          index++;
        }
        else break;
      }
    }

    // エンタイトルツーベース・ホームランで進塁させる用
    public void ProceedRunnersEntitled(int numBases)
    {
      runners = runnersAtPitch;
      if (runnerAtBat)
      {
        Debug.Log("Added runner at bat");
        runners.Add(runnerAtBat);
      }

      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => 
          {
            r.IsBatter = false;
            r.StartingBaseNumber = r.StartingBaseNumber + numBases;
          }
        );
      }

    }

    // バッターランナーのキャッシュを取る
    public void CacheRunnerAtBat(Runner r)
    {
      runnerAtBat = r;
    }

    // 塁にいるランナーのキャッシュを取る
    public void CacheRunnersAtPitch()
    {
      runnersAtPitch = new List<Runner>(GameObject.FindObjectsOfType<Runner>());
    }

    public void ResetRunnersAtBat()
    // 打撃にカメラを戻す時にランナー全員にリセットを掛ける
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

    // 後続のランナーで塁が埋まってるかのクエリ
    public int QueryChasingRunners(int startingBaseNumber)
    {
      var index = startingBaseNumber;
      foreach (var r in runners)
      {
        // 自分は必ずrunnersにいるのでそこまでリストを繰る
        if (r.StartingBaseNumber > index) continue;
        else
        {
          // 途中で番号が飛んでたらフォースアウトではない
          if (r.StartingBaseNumber != index) return -1;
          else index--;
        }
      }
      // 最後まで繰れたらフォースアウトを今いる次の塁に設定する
      return startingBaseNumber + 1;
    }

    public void NotifyRunnersFair()
    {
      var runners = FindObjectsOfType<Runner>();
      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => r.NotifyFair()
        );
      }
    }

    public void DisnotifyRunnersFair()
    {
      var runners = FindObjectsOfType<Runner>();
      foreach (var r in runners)
      {
        ExecuteEvents.Execute<IRunnerMessageHandler>(
          target: r.gameObject,
          eventData: null,
          functor: (receiver, eventData) => r.DisnotifyFair()
        );
      }
    }

    private void OnEnable()
    {
      inputActions.Player.Enable();
    }

    private void OnDisable()
    {
      inputActions.Player.Disable();
    }

  }
}