using UnityEngine;

namespace Test

{
  public class Pitcher : MonoBehaviour, ICustomMessageTarget
  {
    public GameObject ball;
    private float _pitchInterval;
    public float pitchInterval = 1.5f;
    public float pitchForwardForce = 0.8f;
    public float pitchUpForce = 0.4f;

    private bool _isPitched = false;

    void Start()
    {
      _pitchInterval = pitchInterval;
    }

    // Update is called once per frame
    void Update()
    {
      if (_isPitched) return;

      _pitchInterval -= Time.deltaTime;
      if (_pitchInterval < 0f)
      {
        var b = Instantiate(ball);
        b.transform.localPosition = this.transform.position + Vector3.forward;
        _pitchInterval = pitchInterval;
        var rb = b.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * pitchForwardForce + Vector3.up * pitchUpForce, ForceMode.Impulse);
        _isPitched = true;
      }
    }

    public void EnablePitch()
    {
      Debug.Log(this.gameObject.name + "EnablePitch");
      _isPitched = false;
    }
  }
}