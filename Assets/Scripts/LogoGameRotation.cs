using UnityEngine;

namespace Assets.Scripts
{
    public class LogoGameRotation : MonoBehaviour
    {
        [SerializeField] private float speed = 90f;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
        }
    }
}