using TMPro;
using UnityEngine;

namespace Assets.Scripts.PatientData.Steps
{
    public class PatientPresentation : MonoBehaviour
    {
        // PROFILE DATA
        [SerializeField] public TextMeshProUGUI nameFields;
        [SerializeField] public TextMeshProUGUI surnameFields;
        [SerializeField] public TextMeshProUGUI ageFields;
        [SerializeField] public TextMeshProUGUI situationFields;
        [SerializeField] public TextMeshProUGUI activitiesFields;

        // CONTEXT DATA
        [SerializeField] public TextMeshProUGUI contextField;

        // MEDICAL AUTONOMIE DATA
        [SerializeField] public TextMeshProUGUI adlField;
        [SerializeField] public TextMeshProUGUI iadlField;

        // MEDICAL HISTORY DATA
        [SerializeField] public TextMeshProUGUI historyField;

        public void SetPresentationTexts(NewPatientData patientData)
        {
            // Set profil data
            nameFields.text = patientData.surname;
            surnameFields.text = patientData.fisrtName;
            ageFields.text = patientData.age.ToString();
            situationFields.text = patientData.familySituation;
            activitiesFields.text = patientData.occupationalActivities;

            // Set contexte data
            contextField.text = patientData.context;

            // Set autonomie data
            adlField.text = patientData.autonomies[0].value;
            iadlField.text = patientData.autonomies[1].value;

            // Set medical history data
            historyField.text = patientData.medicalHistory;
        }
    }
}
