using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    public enum YesNo {Yes, no }

    [CreateAssetMenu(fileName = "QuestionnaireData", menuName = "Medical/QuestionnaireData")]
    public class QuestionnaireData : ScriptableObject
    {
        public List<QuestionData> questions;
    }

    
    [System.Serializable]
    public class QuestionData
    {
        [TextArea]
        public string questionText;
    }
}
