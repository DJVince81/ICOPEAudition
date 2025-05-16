using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step8Audiometrie : MonoBehaviour
    {
        [Header("Sprite patient")]
        [SerializeField] private Image imageAudiometrie;

        public void SetImageAudiometrie(Sprite sprite)
        {
            imageAudiometrie.sprite = sprite;
        }
    }
}