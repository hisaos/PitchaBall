using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  public static CameraManager Instance;

  public Camera mainCamera;
  public Camera fieldCamera;

  private void Awake()
  {
    if (!Instance) Instance = this;
  }

  // Update is called once per frame
  void Update()
  {
    switch (GameManager.Instance.mode)
    {
      case GameManager.Mode.Main:
        mainCamera.gameObject.SetActive(true);
        fieldCamera.gameObject.SetActive(false);
        break;
      case GameManager.Mode.Field:
        mainCamera.gameObject.SetActive(false);
        fieldCamera.gameObject.SetActive(true);
        break;
      default:
        mainCamera.gameObject.SetActive(false);
        fieldCamera.gameObject.SetActive(false);
        break;
    }
  }
}
