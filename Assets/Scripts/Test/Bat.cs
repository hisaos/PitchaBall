using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    public float batterPower { get; set; }

    private Collider _col;

    private GameObject _pitcher;
    private List<Fielder> _fielders;

    private const float deg2rad = Mathf.PI / 180f;

    void Start()
    {
      _col = GetComponentInChildren<Collider>();
      _pitcher = FindObjectOfType<Pitcher>().gameObject;
      _fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを飛ばす
        var r = other.gameObject.GetComponent<Rigidbody>();
        var t = other.gameObject.transform;
        var pt = this.transform.parent.transform;
        r.velocity = Vector3.zero;
        r.useGravity = true;
        // otherとのコンタクト点よりボールの方が前にあるなら、ボール - コンタクト点の位置の向きに飛ばすと前に飛ぶ
        // r.AddForce(((t.position - other.GetContact(0).point).normalized + Vector3.up / 2f) * batterPower, ForceMode.Impulse);
        Debug.Log("Bat angle y:" + pt.localEulerAngles.y);
        r.AddForce(new Vector3(-Mathf.Sin(pt.localEulerAngles.y * deg2rad), 1f, -Mathf.Cos(pt.localEulerAngles.y * deg2rad)) * batterPower, ForceMode.Impulse);

        // バットを一時的に無効化（1.5秒後復活）
        _col.enabled = false;
        Invoke("EnableBat", 1.5f);

        // カメラをFieldCameraに切り替え
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(false, t)
        );

        foreach (var f in _fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.EnableFielderMove()
          );
        }

      }
    }

    void EnableBat()
    {
      _col.enabled = true;
    }
  }
}