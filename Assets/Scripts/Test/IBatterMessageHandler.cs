using UnityEngine;
using UnityEngine.EventSystems;

public interface IBatterMessageHandler : IEventSystemHandler
{
  // バッターをリフレッシュする
  void EnableBatter();

  // ボールが投げられたことを通知
  void NotifyBallThrown();
}
