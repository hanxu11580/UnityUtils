using UnityEngine;

namespace EntitiesTest.LineExample.NoJobs {
    public class Target : MonoBehaviour
    {
        public Vector3 Direction;

        public void Update()
        {
            transform.localPosition += Direction * Time.deltaTime;
        }
    }
}