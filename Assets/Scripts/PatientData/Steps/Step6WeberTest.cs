using TMPro;
using UnityEngine;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step6WeberTest : MonoBehaviour
    {
        [Header("Dialogue field")]
        [SerializeField] private TextMeshProUGUI patientDialogueField;
        [SerializeField] private TextMeshProUGUI doctorDialogueField;

        private readonly string doctorDialogue = "De quel côté avez-vous entendu le son ?";

        public void SetTextDialogue(string patientContext)
        {
            doctorDialogueField.text = doctorDialogue;
            patientDialogueField.text = patientContext;
        }
    }
}