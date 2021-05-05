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
        ExecuteEvents.Execute<ICustomMessageTarget>(
            target: _pitcher,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnablePitch()
        );
      }
    }
  }
}