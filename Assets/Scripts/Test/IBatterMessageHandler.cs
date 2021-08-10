using UnityEngine;
using UnityEngine.EventSystems;

public interface IBatterMessageHandler : IEventSystemHandler
{
  // バッターをリフレッシュする
  void EnableBatter();
}
