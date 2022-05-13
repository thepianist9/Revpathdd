using UnityEngine;

namespace HistocachingII
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public float speed = 2.0f;

        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                float interpolation = speed * Time.deltaTime;

                Vector3 position = transform.position;
                position.x = Mathf.Lerp(transform.position.x, target.position.x, interpolation);
                position.z = Mathf.Lerp(transform.position.z, target.position.z, interpolation);

                transform.position = position;
            }
        }
    }
}
