using UnityEngine;

namespace Test
{
  public class FieldCamera : MonoBehaviour
  {
    private Transform chaseTarget;
    public Transform ChaseTarget { get { return chaseTarget; } set { chaseTarget = value; } }
    public float cameraHeight;

    private Vector3 cameraAngle;

    void Start()
    {
      cameraAngle = new Vector3(85f, 180f, 0f);
    }

    void Update()
    {
      if (!chaseTarget) return;

      this.transform.position = new Vector3(chaseTarget.position.x, cameraHeight, chaseTarget.position.z);
      // this.transform.LookAt(chaseTarget);
      this.transform.localEulerAngles = cameraAngle;
    }
  }
}
