using UnityEngine;

namespace Test
{
  public class FieldCamera : MonoBehaviour
  {
    private Transform chaseTarget;
    public Transform ChaseTarget { get { return chaseTarget; } set { chaseTarget = value; } }
    public Vector3 offset;

    void Update()
    {
      if (!chaseTarget) return;

      this.transform.position = chaseTarget.position + offset;
      this.transform.LookAt(chaseTarget);
    }
  }
}
