using Unity.Hierarchy;
using UnityEngine;

namespace Assets.Scripts
{
    public class DoorController : MonoBehaviour
    {
        // Control the doors
        [Header("Sprite doors")]
        [SerializeField] public RectTransform leftDoor;
        [SerializeField] public RectTransform rightDoor;

        [Header("Variables")]
        [SerializeField] public float openAngle = 90f;
        [SerializeField] public float speed = 200f;

        // Private variable
        private float current = 0f;


        /// <summary>
        /// Open doors on call.
        /// </summary>
        private void openDoor()
        {    
            current = Mathf.MoveTowards(current, openAngle, speed * Time.deltaTime);
            leftDoor.localRotation = Quaternion.Euler(0f, 0f, -current);
            rightDoor.localRotation = Quaternion.Euler(0f, 0f, current);
        }
        /// <summary>
        /// Close doors on call.
        /// </summary>
        private void closeDoor() 
        {
            current = Mathf.MoveTowards(current, 0f, speed * Time.deltaTime);
            leftDoor.localRotation = Quaternion.Euler(0f, 0f, -current);
            rightDoor.localRotation = Quaternion.Euler(0f, 0f, current);

        }
    }
}