using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step7HhiesTest : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image patient;
        [Header("Sprite HHIES")]
        [SerializeField] private Image imageHHIES;

        public void SetImages(Sprite sprite, Sprite spritePatient)
        {
            imageHHIES.sprite = sprite;
            patient.sprite = spritePatient;
        }
    }
}
