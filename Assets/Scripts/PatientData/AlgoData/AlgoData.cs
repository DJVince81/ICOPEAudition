using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    [CreateAssetMenu(fileName = "AlgoData", menuName = "Medical/AlgoData")]
    public class AlgoData : ScriptableObject
    {
        public List<AlgoStep> steps;
    }
}