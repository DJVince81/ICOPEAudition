using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PatientData.AlgoData
{
    [CreateAssetMenu(fileName = "OtoscopyData", menuName = "Medical/OtoscopyData")]
    public class OtoscopieData : ScriptableObject
    {
        public List<Sprite> imageBank;
    }
}