using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step5Otoscopie : MonoBehaviour
    {
        [Header("Patient video otoscopie field")]
        [SerializeField] private Image videoOtoscopiePatient;
        
        public void SetImageOtoscopiePatient(Sprite sprite)
        {
            videoOtoscopiePatient.sprite = sprite;
        }
    }
}
