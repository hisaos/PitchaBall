using UnityEngine;

namespace Test
{
  public class FieldCamera : MonoBehaviour
  {
    private Transform chaseTarget;
    public Transform ChaseTarget { get { return chaseTarget; } set { chaseTarget = value; } }
    public Vector3 offset;

    private Vector3 cameraAngle;

    void Start()
    {
      cameraAngle = new Vector3(85f, 180f, 0f);
    }

    void Update()
    {
      if (!chaseTarget) return;

      this.transform.position = chaseTarget.position + offset;
      this.transform.LookAt(chaseTarget);
      this.transform.localEulerAngles = cameraAngle;
    }
  }
}
