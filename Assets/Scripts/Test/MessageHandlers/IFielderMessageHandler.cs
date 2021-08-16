using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public interface IFielderMessageHandler : IEventSystemHandler
  {
    // ボールが投げられた時のメッセージハンドラ
    void SetFielderBall(GameObject g);

    // ボールが消えた時のメッセージハンドラ
    void ResetFielderBall();

    // ボールが打たれた時のメッセージハンドラ
    void EnableFielderMove();

    // 追わなくなる時のメッセージハンドラ
    void DisableFielderMove();

    // カバーに入るかを決める角度を伝えるメッセージハンドラ
    void NotifyBallAngle(float angle);

    // 元の位置に戻る
    void ReturnToOriginalPosition();

    // ボールまでの距離を教え合うメッセージハンドラ
    void TellDistanceToBall(float f);
  }
}