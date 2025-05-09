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
        public string answerText;
        public bool isCorrect;
        [TextArea]
        public string correctionText;
        public List<Sprite> sprites;
    }

    [System.Serializable]
    public class PatientQuestionAnswer
    {
        public QuestionData question;
        public YesNo patientAnswer;
    }

    [System.Serializable]
    public class PatientVideoOtoscopie
    {
        public Sprite videoOstoscopie;
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
        
        [Header("Si type: Questionnary")]
        public QuestionnaireData questionnaireData;
        public List<PatientQuestionAnswer> predefinedAnwser;

        [Header("Si type: Video Otoscopie ou test HHIES")]
        public PatientVideoOtoscopie videoOstoscopie;

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
