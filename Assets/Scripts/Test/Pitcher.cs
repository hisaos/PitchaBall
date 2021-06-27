using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test

{
  public class Pitcher : MonoBehaviour, IPitcherMessageHandler
  {
    public GameObject ball;
    private float _pitchInterval;
    public float pitchInterval = 1.5f;
    public float pitchForwardForce = 0.8f;
    public float pitchUpForce = 0.4f;

    private bool _isPitched = false;

    private List<Fielder> _fielders;

    void Start()
    {
      _pitchInterval = pitchInterval;
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    // Update is called once per frame
    void Update()
    {
      if (_isPitched) return;

      _pitchInterval -= Time.deltaTime;

      // ボールを投げる
      if (_pitchInterval < 0f)
      {
        var b = Instantiate(ball);
        b.transform.localPosition = this.transform.position + Vector3.forward;
        _pitchInterval = pitchInterval;
        var rb = b.GetComponent<Rigidbody>();
        rb.useGravity = false;
        // rb.AddForce(Vector3.forward * pitchForwardForce + Vector3.up * pitchUpForce, ForceMode.Impulse);
        rb.AddForce(Vector3.forward * pitchForwardForce, ForceMode.Impulse);
        _isPitched = true;

        foreach (var f in _fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.SetFielderBall(b)
          );
        }
      }
    }

    public void EnablePitch()
    {
      Debug.Log(this.gameObject.name + "EnablePitch");
      _isPitched = false;
    }
  }
}