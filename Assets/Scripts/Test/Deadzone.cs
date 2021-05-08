using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Deadzone : MonoBehaviour
  {
    private GameObject _pitcher;

    // Start is called before the first frame update
    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
      Debug.Log("Foul");

      if (other.gameObject.CompareTag("Ball"))
      {
        // ピッチャーを投げれる状態にする
        ExecuteEvents.Execute<ICustomMessageTarget>(
            target: _pitcher,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnablePitch()
        );

        // ボールを消す（即時）->捕球
        Destroy(other.gameObject);
      }
    }
  }
}