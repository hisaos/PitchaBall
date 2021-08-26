using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test
{
  public class Bat : MonoBehaviour
  {
    public float batterPower { get; set; }

    private Collider batCollider;

    private GameObject pitcher;
    private List<Fielder> fielders;

    private const float deg2rad = Mathf.PI / 180f;
    private Transform sweetspot;
    private Transform minDistanceContact; // スイートスポットとバットとのコンタクトが最短になる点
    private float baseDistanceToSweetspot;

    void Start()
    {
      batCollider = GetComponentInChildren<Collider>();
      pitcher = FindObjectOfType<Pitcher>().gameObject;
      fielders = new List<Fielder>(FindObjectsOfType<Fielder>());
      sweetspot = transform.Find("sweetspot"); // sweetspot
      minDistanceContact = transform.Find("minDistanceContact");  // minDistanceContact
      baseDistanceToSweetspot = Vector3.Distance(sweetspot.localPosition, minDistanceContact.localPosition);
      // Debug.Log("minDistanceToSweetspot: " + baseDistanceToSweetspot);
    }

    void OnCollisionEnter(Collision other)
    {
      if (other.gameObject.CompareTag("Ball"))
      {
        // ボールを飛ばす
        var ballRigidbody = other.gameObject.GetComponent<Rigidbody>();
        var ballTransform = other.gameObject.transform;
        var batPivotTransform = this.transform.parent.transform;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.useGravity = true;

        // バットを一時的に無効化（1.5秒後復活）
        batCollider.enabled = false;
        Invoke("EnableBat", 1.5f);

        // otherとのコンタクト点よりボールの方が前にあるなら、ボール - コンタクト点の位置の向きに飛ばすと前に飛ぶ
        // sweetspotからの距離によってパワーと向きを変える
        var dist = Vector3.Distance(ballTransform.position, sweetspot.position);
        // Debug.Log("Distance to sweetspot: " + dist);
        var batAngle = batPivotTransform.localEulerAngles.y;
        // Debug.Log("BatAngle: " + batAngle);
        ballRigidbody.AddForce(new Vector3(-Mathf.Sin(batAngle * deg2rad), 1f / (dist / baseDistanceToSweetspot), -Mathf.Cos(batAngle * deg2rad)) * batterPower, ForceMode.Impulse);

        // カメラをFieldCameraに切り替え
        ExecuteEvents.Execute<ICameraManagerMessageHander>(
          target: CameraManager.Instance,
          eventData: null,
          functor: (receiver, eventData) => receiver.SwitchCamera(false, ballTransform)
        );

        // 野手に打球の情報を伝える
        foreach (var f in fielders)
        {
          ExecuteEvents.Execute<IFielderMessageHandler>(
            target: f.gameObject,
            eventData: null,
            functor: (receiver, eventData) => {
              receiver.EnableFielderMove();
              receiver.NotifyBallAngle(batAngle);
              receiver.SetFielderBall(other.gameObject);
            }
          );
        }

        // 既存のランナーを走らせる
        var runners = FindObjectsOfType<Runner>();
        foreach (var r in runners)
        {
          ExecuteEvents.Execute<IRunnerMessageHandler>(
            target: r.gameObject,
            eventData: null,
            functor: (receiver, eventData) => receiver.ProceedBase(3)
          );
        }

        // バッターランナーを出す
        RunnerManager.Instance.InstantiateRunner();
      }
    }

    public void EnableBat()
    {
      batCollider.enabled = true;
    }

    public void DisableBat()
    {
      batCollider.enabled = false;
    }
  }
}