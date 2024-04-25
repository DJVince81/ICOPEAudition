using UnityEngine;
using UnityEngine.UI;

public class DocumentDisplayTest : MonoBehaviour
{
    [SerializeField] private Transform _documentContentParent;
    [SerializeField] private ScrollRect _documentContentScrollRect;

    private PatientData _patientData;
    private StepData[] _steps;

    private int _currentStepIndex = 0;
    private float _timer = 0f;
    private const float TIME_BETWEEN_STEPS = 5f;

    private void Start()
    {
        _patientData = ScriptableObject.CreateInstance<PatientData>();

        _patientData.RandomizeData(4);

        _steps = _patientData.GetStepDatas();

        DisplayStep(_currentStepIndex);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > TIME_BETWEEN_STEPS)
        {
            _timer %= TIME_BETWEEN_STEPS;
            _currentStepIndex = (_currentStepIndex + 1) % _steps.Length;
            DisplayStep(_currentStepIndex);
        }
    }

    private void DisplayStep(int stepIndex)
    {
        Debug.Log($"Displaying step {stepIndex}");

        StepData currentStep = _steps[stepIndex];

        // Remove all children of the parent
        for (int i = 0; i < _documentContentParent.childCount; i++)
        {
            Destroy(_documentContentParent.GetChild(i).gameObject);
        }

        GameObject newContent = Instantiate(currentStep.GetStepDocumentPrefab(), _documentContentParent);
        _documentContentScrollRect.content = newContent.GetComponent<RectTransform>();

        currentStep.UpdateStepDocumentWithData(newContent);
    }
}
