using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test

{
  public class Pitcher : MonoBehaviour, IPitcherMessageHandler
  {
    private InputActions _ia;
    private bool isPlayer;
    public bool IsPlayer { get { return isPlayer; } set { isPlayer = value; } }

    public GameObject ball;
    private float pitchInterval;
    public float PitchInterval = 1.5f;
    public float pitchForwardForce = 0.8f;
    public float pitchUpForce = 0.4f;

    private bool isPitched = false;

    private List<Fielder> fielders;

    void Awake()
    {
      _ia = new InputActions();

      _ia.Player.A.performed += (context) =>
      {
        Debug.Log(this.name + ": A Press");
        if (isPlayer && !isPitched) ExecutePitch();
      };
    }

    void Start()
    {
      isPlayer = false;
      pitchInterval = PitchInterval;
      fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    // Update is called once per frame
    void Update()
    {
      if (isPitched) return;

      // CPU側
      // ボールを投げる（CPU側のトリガー）
      if (!isPlayer)
      {
        pitchInterval -= Time.deltaTime;
        if (pitchInterval < 0f)
        {
          ExecutePitch();
        }
      }
    }

    private void ExecutePitch()
    {
      var b = Instantiate(ball);
      b.transform.localPosition = this.transform.position + Vector3.forward;
      pitchInterval = PitchInterval;
      var rb = b.GetComponent<Rigidbody>();
      rb.useGravity = false;
      rb.AddForce(Vector3.forward * pitchForwardForce, ForceMode.Impulse);
      isPitched = true;

      foreach (var f in fielders)
      {
        ExecuteEvents.Execute<IFielderMessageHandler>(
          target: f.gameObject,
          eventData: null,
          functor: (receiver, eventData) => receiver.SetFielderBall(b)
        );
      }
    }

    public void EnablePitch()
    {
      Debug.Log(this.gameObject.name + "EnablePitch");
      isPlayer = !isPlayer;
      isPitched = false;
    }

    void OnEnable()
    {
      _ia.Player.Enable();
    }

    void OnDisable()
    {
      _ia.Player.Disable();
    }
  }
}