using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public interface IBatterMessageHandler : IEventSystemHandler
  {
    // バッターをリフレッシュする
    void EnableBatter();

    // ボールが投げられたことを通知
    void NotifyBallThrown(GameObject ball);
  }
}