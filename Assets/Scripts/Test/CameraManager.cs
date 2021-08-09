using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
  public class CameraManager : MonoBehaviour, ICameraManagerMessageHander
  {
    public Camera MainCamera;
    public Camera FieldCamera;
    private FieldCamera fieldCamera;

    public static GameObject Instance;

    void Start()
    {
      if (Instance == null) Instance = this.gameObject;
      MainCamera.enabled = true;
      FieldCamera.enabled = false;
      fieldCamera = FieldCamera.GetComponent<FieldCamera>();
    }

    // カメラを切り替えるメッセージハンドラ    
    public void SwitchCamera(bool isMain, Transform target)
    {
      MainCamera.enabled = isMain;
      FieldCamera.enabled = !isMain;
      if (target) fieldCamera.ChaseTarget = target;
    }
  }
}