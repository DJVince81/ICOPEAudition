using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    [CreateAssetMenu(fileName = "QuestionnaireData", menuName = "Medical/QuestionnaireData")]
    public class QuestionnaireData : ScriptableObject
    {
        public List<QuestionData> questions;
    }

    [System.Serializable]
    public class QuestionData
    {
        public string questionText;
        public List<string> possibleAnswers;
    }

    [System.Serializable]
    public class QuestionAnswer
    {
        public QuestionData question;
        public string selectedAnswer;
    }
}
