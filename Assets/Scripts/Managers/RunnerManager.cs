using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : MonoBehaviour
{
  [SerializeField] GameObject[] runners;
  [SerializeField] GameObject[] bases;

  // Update is called once per frame
  void Update()
  {
    for (int i = 0; i < runners.Length; i++)
    {
      if (!runners[i]) continue;

      var runner = runners[i].GetComponent<Runner>();
      var rate = runner.rate;
      if (rate >= 0 && rate < 25) runners[i].transform.position = ((25f - rate) / 25f) * bases[0].transform.position + (rate / 25f) * bases[1].transform.position;
      if (rate >= 25 && rate < 50) runners[i].transform.position = ((50f - rate) / 25f) * bases[1].transform.position + ((rate - 25f) / 25f) * bases[2].transform.position;
      if (rate >= 50 && rate < 75) runners[i].transform.position = ((75f - rate) / 25f) * bases[2].transform.position + ((rate - 50f) / 25f) * bases[3].transform.position;
      if (rate >= 75 && rate < 100) runners[i].transform.position = ((100f - rate) / 25f) * bases[3].transform.position + ((rate - 75f) / 25f) * bases[0].transform.position;
    }
  }
}
