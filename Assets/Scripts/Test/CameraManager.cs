using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class CameraManager : MonoBehaviour, ICameraManagerMessageHander
  {
    public Camera mainCamera;
    public Camera fieldCamera;
    public static GameObject Instance;

    void Start()
    {
      if (Instance == null) Instance = this.gameObject;
      mainCamera.enabled = true;
      fieldCamera.enabled = false;
    }

    // カメラを切り替えるメッセージハンドラ    
    public void SwitchCamera(bool isMain)
    {
      mainCamera.enabled = isMain;
      fieldCamera.enabled = !isMain;
    }
  }
}