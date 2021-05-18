using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Catcher : MonoBehaviour
  {
    private GameObject _pitcher;
    private List<Fielder> _fielders;

    void Start()
    {
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    void OnCollisionEnter(Collision other)
    {
      // ピッチャーを投げれる状態にする
      ExecuteEvents.Execute<IPitcherMessageHandler>(
        target: _pitcher,
        eventData: null,
        functor: (receiver, eventData) => receiver.EnablePitch()
      );

      // ボールを取ったら追わずに元の位置へ
      foreach (var f in _fielders)
      {
        ExecuteEvents.Execute<IFielderMessageHandler>(
          target: f.gameObject,
          eventData: null,
          functor: (receiver, eventData) =>
          {
            receiver.ResetFielderBall();
            receiver.DisableFielderMove();
            receiver.ReturnToOriginalPosition();
          }
        );
      }
    }

  }
}