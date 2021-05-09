﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public interface IFielderMessageHandler : IEventSystemHandler
  {
    // ボールが投げられた時のメッセージハンドラ
    void SetFielderBall(GameObject g);

    // ボールが打たれた時のメッセージハンドラ
    void EnableFielderMove();

    // ボールが消えた時のメッセージハンドラ
    void ResetFielderBall();

    // 追わなくなる時のメッセージハンドラ
    void DisableFielderMove();
  }
}