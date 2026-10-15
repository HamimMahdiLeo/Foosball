using UnityEngine;

namespace Foosball
{
    public class Camera : MonoBehaviour
    {
        public Transform target; // Drag the Ball (or Table) here
        public float followStrength = 0.25f;
        public float smoothSpeed = 5f;

        private Vector3 startPos;

        void Start()
        {
            // Keeps the exact camera position you set up in the Scene view
            startPos = transform.position;
        }

        void LateUpdate()
        {
            if (target == null) return;

            // Shifts camera slightly along the Z axis (along the table) toward the target
            Vector3 desiredPos = startPos;
            desiredPos.z = startPos.z + (target.position.z * followStrength);

            transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        }
    }
}