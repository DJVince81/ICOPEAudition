using Assets.Scripts.PatientData.AlgoData;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.PatientData.Steps
{
    public class PatientWisperTest : MonoBehaviour
    {
        // DOCTOR POSITION
        [Header("Doctor position")]
        [SerializeField] public GameObject doctorPos1;
        [SerializeField] public GameObject doctorPos2;

        // DOCTOR TEXT
        [Header("Doctor texts")]
        [SerializeField] public TextMeshProUGUI targerDoctorText1;
        [SerializeField] public TextMeshProUGUI targerDoctorText2;

        // PATIENT TEXT
        [Header("Patient text")]
        [SerializeField] public TextMeshProUGUI targetPatientText;

        // DELAI
        [SerializeField] public float delayBetweenWords = 0.3f;
        [SerializeField] public float delayBetweenText = 1f;


        // TO change in a scriptable object
        private readonly string[] doctorWords = { "Ami", "Bateau", "Bureau", "Chameau", "Cheval", "Hibou", "Journal", "Lama", "Lapin", "Moto", "Mouton", "Parfait", "Pompier", "Salon", "Serpent"};
        private string patientText;

        private void ClearTexts()
        {
            targerDoctorText1.text = "";
            targerDoctorText2.text = "";
            targetPatientText.text = "";
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
        
        private void AnimateText(TextMeshProUGUI target, string[] words, float startDelay = 0f, TweenCallback onComplete = null)
        {
            target.text = "";

            for (int i = 0; i < words.Length; i++)
            {
                
                string word = words[i];
                float delay = startDelay + i * delayBetweenWords;

                DOVirtual.DelayedCall(delay, () =>
                {
                    target.text = word;
                });
            }

            if (onComplete != null)
            {
                float totalTime = startDelay + words.Length * delayBetweenWords;
                DOVirtual.DelayedCall(totalTime, onComplete);
            }
        }

        public void PlayFirstText(AlgoStep step)
        {
            // Load in memory patient text form algoStep
            patientText = step.contextDescription;

            // Clear texts & docotor sprite
            ClearTexts();
            ClearDoctorSprite();

            // Activate doctor sprite position 1
            doctorPos1.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(targerDoctorText1, strings, 0f, () =>
            {
                float totalDelay = strings.Length * delayBetweenWords + delayBetweenText;
                DOVirtual.DelayedCall(totalDelay, PlaySecondText);
            });
        }

        private void PlaySecondText()
        {
            ClearTexts();
            ClearDoctorSprite();
            
            doctorPos2.SetActive(true);

            string[] strings = GetRandomListWord();
            AnimateText(targerDoctorText2, strings, 0f, () =>
            {
                float totalDelay = strings.Length * delayBetweenWords + delayBetweenText;
                DOVirtual.DelayedCall(totalDelay, ShowPatientText);
            });
        }

        private void ShowPatientText()
        {
            ClearTexts();
            targetPatientText.text = patientText;
        }

    }

}
