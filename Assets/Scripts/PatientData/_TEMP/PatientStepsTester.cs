using UnityEngine;

public class PatientStepsTester : MonoBehaviour
{
    public PatientData patientData;
    private void Awake()
    {
        patientData = ScriptableObject.CreateInstance<PatientData>();
    }

    private void Start()
    {
        PrintSteps(4);
    }

    private void PrintSteps(int stepToReach)
    {
        Debug.Log("==================================================");
        Debug.Log($"Generating data to go to step {stepToReach}:");

        patientData.RandomizeData(stepToReach);

        var steps = patientData.GetStepDatas();

        for (int i = 0; i < steps.Length; i++)
        {
            Debug.Log($"Step {i} data:");

            if (steps[i] is Step0Data step0Data)
            {
                Debug.Log($"Whisper test data: {string.Join(", ", step0Data.whisperTestData)}");
            }
            else if (steps[i] is Step1Data step1Data)
            {
                Debug.Log($"Go/No-Go answers: {string.Join(", ", step1Data.goNoGoAnswers)}");
            }
            else if (steps[i] is Step2Data step2Data)
            {
                // Debug.Log($"Video-otoscopy image name: {step2Data.GetVideoOtoscopySprite().name}");
            }
            else if (steps[i] is Step3Data step3Data)
            {
                Debug.Log($"Weber test result: {step3Data.weberTestResult}");
                Debug.Log($"Affected ear: {step3Data.affectedEar}");
            }
            else if (steps[i] is Step4Data step4Data)
            {
                // Debug.Log($"Audiogram image name: {step4Data.GetAudiometrySprite().name}");
            }

            for (int j = 0; j < steps[i].GetPossibleDiagnostics().Length; j++)
            {
                Debug.Log($"Possible diagnostic {j}: {steps[i].GetPossibleDiagnostics()[j]} (correct: {steps[i].IsDiagnosticCorrect(j)})");
            }
            for (int j = 0; j < steps[i].GetPossibleActions().Length; j++)
            {
                Debug.Log($"Possible action {j}: {steps[i].GetPossibleActions()[j]} (correct: {steps[i].IsActionCorrect(j)})");
            }

            Debug.Log($"Should go to next step: {steps[i].LeadsToNextStep()}");
            Debug.Log("--------------------------------------------------");
            if (!steps[i].LeadsToNextStep()) break;
        }

        Debug.Log("Last step reached.");
        Debug.Log("==================================================");
    }
}
