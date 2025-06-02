using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step5Otoscopie : MonoBehaviour
    {
        [Header("Patient video otoscopie field")]
        [SerializeField] private Image videoOtoscopiePatient;

        [Header("Patient sprite")]
        [SerializeField] private Image patient;

        public void SetImages(Sprite sprite, Sprite spritePatient)
        {
            if (sprite == null) return; 
            if (spritePatient == null) return;
            videoOtoscopiePatient.sprite = sprite;

            patient.sprite = spritePatient;
            patient.SetNativeSize();
        }
    }
}
