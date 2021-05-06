using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Fence : MonoBehaviour
  {
    private GameObject _pitcher;

    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<ICustomMessageTarget>(
            target: _pitcher,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // ボールを消す（1.5秒後）
        Destroy(other.gameObject, 1.5f);
      }
    }
  }
}