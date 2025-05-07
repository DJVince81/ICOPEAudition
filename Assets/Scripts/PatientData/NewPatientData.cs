using Assets.Scripts.PatientData.AlgoData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData
{
    [CreateAssetMenu(fileName = "NewPatientData", menuName = "Medical/Patient")]
    public class NewPatientData : ScriptableObject
    {
        [Header("Profil")]
        public string surname;
        public string fisrtName;
        public int age;

        [Header("Family Situation")]
        public string familySituation;

        [Header("Occupational Activities")]
        public string occupationalActivities;

        [Header("Context")]
        [TextArea]
        public string context;

        [Header("Autonomies")]
        public List<NamedValue> autonomies;

        [Header("Medical History")]
        [TextArea]
        public string medicalHistory;

        [Header("Algoritm steps")]
        public List<AlgoStep> steps;

        [Header("Questionaire Go-No-Go")]
        public List<PatientQuestionnaireSession> questionnaireSessions;
    }

    [System.Serializable]
    public class NamedValue
    {
        public string name;
        public string value;
    }

    [System.Serializable]
    public class PatientQuestionnaireSession
    {
        public QuestionnaireData source;
        public List<QuestionAnswer> reponses;
    }
}
