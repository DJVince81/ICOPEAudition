using Assets.Scripts.PatientData.AlgoData;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step3And4Questionnary : MonoBehaviour
    {
        // Content GameObject
        [Header("GameObject content")]
        [SerializeField] private GameObject content;
        [SerializeField] private Image patientSprite;

        public void SetQuestionayText(List<QuestionData> questions,List<PatientQuestionAnswer> answers)
        {
            // Set text in children
            TextMeshProUGUI[] testMeshes = content.GetComponentsInChildren<TextMeshProUGUI>();

            int max = Mathf.Min(answers.Count, testMeshes.Length);

            for (int i = 0; i < max; i++)
            {
                testMeshes[i].text = "- " + questions[i].questionText + " " + (answers[i].patientAnswer == 0 ? "<b>Oui</b>" : "<b>Non</b>");
            }
        }

        public void SetPatientSprite(Sprite sprite)
        {
            patientSprite.sprite = sprite;
            patientSprite.SetNativeSize();
        }
    }
}