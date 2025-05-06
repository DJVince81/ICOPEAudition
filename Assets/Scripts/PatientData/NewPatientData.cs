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
        public string context;

        [Header("Autonomies")]
        public string[] autonomies;

        [Header("Medical History")]
        public string medicalHistory;

        // each steps of algorithm
    }
}
