using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    public enum AlgoType
    {
        Case_presentation,
        Wisper_test,
        Questionnary,
        Additional_questionnaire,
        Otoscopy,
        Weber_test,
        HHIES_test,
        Audiometry,
    }

    [System.Serializable]
    public class AnswerData
    {
        public string answeText;
        public bool isCorrect;
        [TextArea]
        public string correctionText;
    }

    [System.Serializable]
    public class PhaseData
    {
        public string questionText;
        public List<AnswerData> answerData;

        public bool InAnswerCorrect(int index)
        {
            if (index < 0 || index >= answerData.Count) return false;
            return answerData[index].isCorrect;
        }

        public string GetCorrection(int index)
        {
            if (index < 0 || index >= answerData.Count) return "";
            return answerData[index].correctionText;
        }
    }

    [System.Serializable]
    public class AlgoStep
    {
        public AlgoType type;

        [Header("Contexte medicale")]
        [TextArea]
        public string contextDescription;
        
        [Header("Si type == Questionnary")]
        public QuestionnaireData questionnaireData;

        [Header("Phase 1: Diagnotic")]
        public bool hasDiagnosticPhase;
        public PhaseData diagnosticPhase;

        [Header("Phase 2: Action")]
        public bool hasActionPhase;
        public PhaseData actionPhase;

        public bool IsOptional;
        public bool isTerminatingStep;
    }
}
