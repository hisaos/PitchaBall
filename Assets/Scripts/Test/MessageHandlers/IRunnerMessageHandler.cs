using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public interface IRunnerMessageHandler : IEventSystemHandler
  {
    // 方向と進塁入力によって進塁する
    void ProceedBase(int commandNum);

    // 方向と帰塁入力によって帰塁する
    void ReturnBase(int commandNum);

    // ランナーの状態をリセット
    void ResetAtBat();

    // アウトのメッセージ
    void NotifyOut();

    // フライアウトのメッセージ
    void NotifyFlyOut();

    // フォースアウトのメッセージ
    void NotifyForceOut(int baseNumber);

    // フォースアウトになる塁を指定
    void NotifyForceOutBaseNumber();

    // タッチアップアウトのメッセージ
    void NotifyTouchupOut(int baseNumber);

    // タッチアップアウトの塁を指定
    void NotifyTouchupOutBaseNumber();

    // フェアのメッセージ
    void NotifyFair();

  }
}

