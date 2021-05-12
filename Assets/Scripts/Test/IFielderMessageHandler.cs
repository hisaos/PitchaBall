using UnityEngine;
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

    // 元の位置に戻る
    void ReturnToOriginalPosition();

    // ボールまでの距離を教え合うメッセージハンドラ
    void TellDistanceToBall(float f);
  }
}