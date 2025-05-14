using DG.Tweening;
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
        [SerializeField] public TextMeshProUGUI doctorTextPos1;
        [SerializeField] public TextMeshProUGUI doctorTextPos2;
        [SerializeField] public List<TextMeshProUGUI> doctorTextPos;

        // PATIENT TEXT
        [Header("Patient text")]
        [SerializeField] public TextMeshProUGUI patientText;

        // DELAI
        [SerializeField] public float delayBetweenWords = 0.5f;
        [SerializeField] public float delayBetweenReplays = 1f;


        // TO change in a scriptable object
        private readonly string[] doctorWords = { "Ami", "Bateau", "Bureau", "Chameau", "Cheval", "Hibou", "Journal", "Lama", "Lapin", "Moto", "Mouton", "Parfait", "Pompier", "Salon", "Serpent"};
        private float delay = 0;
       
        private void DoctorWisperWordByWord(TextMeshProUGUI textMesh) 
        {
            textMesh.text = "";

            string[] words = GetRandomListWord();

            foreach (string word in words)
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    textMesh.text = word;
                });
                delay += delayBetweenWords;
            }
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
        
        public void PlayAnimation()
        {
            
            delay = 0;
            for (int i = 0; i < doctorTextPos.Count; i++)
            {
                DOVirtual.DelayedCall(delay, () => DoctorWisperWordByWord(doctorTextPos[i]));
                delay += delayBetweenReplays;
            }
        }
    }

}
