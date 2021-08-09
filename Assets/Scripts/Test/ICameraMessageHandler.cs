using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public interface ICameraManagerMessageHander : IEventSystemHandler
  {
      // カメラを切り替えるメッセージハンドラ
    void SwitchCamera(bool isMain, Transform target);
  }
}