using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PatientData.Steps
{
    public class Step6WeberTest : MonoBehaviour
    {
        [Header("Dialogue field")]
        [SerializeField] private TextMeshProUGUI patientDialogueField;
        [SerializeField] private TextMeshProUGUI doctorDialogueField;

        [Header("Patient images")]
        [SerializeField] private Image patient;

        private readonly string doctorDialogue = "De quel côté avez-vous entendu le son ?";

        public void SetTextDialogue(string patientContext)
        {
            doctorDialogueField.text = doctorDialogue;
            patientDialogueField.text = patientContext;
        }

        public void SetImage(Sprite spritePatient)
        {
            patient.sprite = spritePatient;
            patient.SetNativeSize();
        }
    }
}