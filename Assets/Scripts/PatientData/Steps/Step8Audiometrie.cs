using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step8Audiometrie : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image patientImage;
        [Header("Sprite Audiometrie")]
        [SerializeField] private Image imageAudiometrie;


        public void SetSprite(Sprite spriteAudio, Sprite spritePatient)
        {
            imageAudiometrie.sprite = spriteAudio;
            patientImage.sprite = spritePatient;
        }
    }
}