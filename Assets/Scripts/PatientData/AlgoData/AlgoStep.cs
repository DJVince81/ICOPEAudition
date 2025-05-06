using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    public class AlgoStep
    {
        public AlgoType type;
        public string stepDescription;

        //To continue

        //Condition de validation
        public bool correctDiagnoticAnswer;
        public bool correctActionAnswer;

        public bool isTerminatingStep;
    }

    public enum AlgoType
    {
        StartTest,
        WisperTest,
        Questionnary,
        Additional_questionnaire,
        Otoscopy,
        Audiometry,
        WeberTest,
    }
}
