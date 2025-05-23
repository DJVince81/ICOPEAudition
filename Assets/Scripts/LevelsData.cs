using Assets.Scripts.PatientData;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "LevelsData", menuName = "Medical/LevelsData")]
    public class LevelsData : ScriptableObject
    {
        public List<LevelPatientData> patientByLevel;
    }


    [System.Serializable]
    public class LevelPatientData
    {
        public string levelName;
        public List<NewPatientData> patientsCase;
    }
}