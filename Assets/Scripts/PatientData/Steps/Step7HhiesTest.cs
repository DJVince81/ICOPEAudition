using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step7HhiesTest : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image imageHHIES;

        public void SetImageHHIES(Sprite sprite)
        {
            imageHHIES.sprite = sprite;
        }
    }
}
