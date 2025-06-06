using Assets.Scripts.PatientData.AlgoData;
using DG.Tweening;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step2WisperTest : MonoBehaviour
    {
        // DOCTOR POSITION
        [Header("Doctor position")]
        [SerializeField] private GameObject doctorPos1;
        [SerializeField] private GameObject doctorPos2;

        // DOCTOR TEXT
        [Header("Doctor texts")]
        [SerializeField] private GameObject goDoctorText1;
        [SerializeField] private GameObject goDoctorText2;
        [SerializeField] private TextMeshProUGUI targerDoctorText1;
        [SerializeField] private TextMeshProUGUI targerDoctorText2;

        // PATIENT SPRITE
        [Header("Patient sprite")]
        [SerializeField] private Image patientSprite;

        // PATIENT TEXT
        [Header("Patient text")]
        [SerializeField] private GameObject goPatientText;
        [SerializeField] private TextMeshProUGUI targetPatientText;

        // DELAI
        [Header("Delay animation")]
        [SerializeField] private float delayBetweenWords = 2f;
        [SerializeField] private float delayBetweenText = 2f;


        // TO change in a scriptable object
        private readonly string[] doctorWords = { "Ami", "Bateau", "Bureau", "Chameau", "Cheval", "Hibou", "Journal", "Lama", "Lapin", "Moto", "Mouton", "Parfait", "Pompier", "Salon", "Serpent"};
        private string patientText;

        private void ClearTexts()
        {
            targerDoctorText1.text = "";
            targerDoctorText2.text = "";
            targetPatientText.text = "";
        }

        private void ClearDialogueBox()
        {
            goDoctorText1.SetActive(false);
            goDoctorText2.SetActive(false);
            goPatientText.SetActive(false);
        }

        private void ClearDoctorSprite()
        {
            doctorPos1.SetActive(false);
            doctorPos2.SetActive(false);
        }

        private string[] GetRandomListWord()
        {
            string[] strings = new string[4];
            
            for (int i = 0; i < strings.Length; i++)
            {
                int nRandom = Random.Range(0, doctorWords.Length);
                strings[i] = doctorWords[nRandom];
            }

            return strings;
        }
        
        private void AnimateText(GameObject goTargert, TextMeshProUGUI target, string[] words, float startDelay = 0f, TweenCallback onComplete = null)
        {
            target.text = "";
            goTargert.SetActive(false);

            for (int i = 0; i < words.Length; i++)
            { 
                string word = words[i];
                float delay = startDelay + i * delayBetweenWords;

                DOVirtual.DelayedCall(delay, () =>
                {
                    target.text = word;
                    goTargert.SetActive(true);
                });

                float hideDelay = delay + delayBetweenWords * 0.8f;
                DOVirtual.DelayedCall(hideDelay, () =>
                {
                    goTargert.SetActive(false);
                });
            }

            if (onComplete != null)
            {
                float totalTime = startDelay + words.Length * delayBetweenWords;
                DOVirtual.DelayedCall(totalTime, onComplete);
            }
        }

        // Patient must be in sitting position on a chair.
        public void SetPatient(Sprite patient)
        {
            if (patient == null) return;
                
            patientSprite.sprite = patient;
            patientSprite.SetNativeSize();
        }

        public void PlayFirstText(AlgoStep step)
        {
            // Load in memory patient text form algoStep
            patientText = step.contextDescription;

            // Clear texts & docotor sprite
            ClearTexts();
            ClearDialogueBox();
            ClearDoctorSprite();

            // Activate doctor sprite position 1
            doctorPos1.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(goDoctorText1, targerDoctorText1, strings, 0f, () =>
            {
                DOVirtual.DelayedCall(delayBetweenText, PlaySecondText);
            });
        }

        private void PlaySecondText()
        {
            ClearTexts();
            ClearDialogueBox();
            ClearDoctorSprite();
            
            doctorPos2.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(goDoctorText2 ,targerDoctorText2, strings, 0f, () =>
            {
                DOVirtual.DelayedCall(delayBetweenText, ShowPatientText);
            });
        }

        private void ShowPatientText()
        {
            ClearTexts();
            ClearDialogueBox();

            goPatientText.SetActive(true);
            targetPatientText.text = patientText;
        }
    }
}
