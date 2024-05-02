using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour
{
    [SerializeField] private Transform _documentContentParent;
    [SerializeField] private Button[] _diagButtons;
    [SerializeField] private Button[] _actionButtons;
    [SerializeField] private ButtonGroup _diagButtonGroup;
    [SerializeField] private ButtonGroup _actionButtonGroup;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _nextButton;

    private PatientData _patientData;
    private StepData[] _steps;
    private int _currentStepIndex = -1;

    public bool WasCorrectlyAnswered { get; private set; } = false;
    private bool _isInit = false;

    private void Start()
    {
        _diagButtonGroup.OnButtonSelected += UpdateConfirmButton;
        _actionButtonGroup.OnButtonSelected += UpdateConfirmButton;
    }

    internal void Initialize()
    {
        _patientData = ScriptableObject.CreateInstance<PatientData>();
        int stepToReach = Random.Range(0, 5);
        _patientData.RandomizeData(stepToReach);
        _steps = _patientData.GetStepDatas();

        _isInit = true;
    }

    private StepData GetCurrentStep()
    {
        if (_currentStepIndex < 0 || _currentStepIndex >= _steps.Length)
        {
            Debug.LogError("No step loaded");
            return null;
        }
        return _steps[_currentStepIndex];
    }

    internal void LoadStep(int stepIndex)
    {
        _currentStepIndex = stepIndex;
        if (_isInit) DisplayStep();
        WasCorrectlyAnswered = false;
    }
    private void DisplayStep()
    {
        StepData currentStep = GetCurrentStep();

        // Remove all children of the parent in reverse order
        for (int i = _documentContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_documentContentParent.GetChild(i).gameObject);
        }

        GameObject newContent = Instantiate(currentStep.GetStepDocumentPrefab(), _documentContentParent);
        RectTransform rect = _documentContentParent.GetComponent<RectTransform>();
        rect.sizeDelta = _currentStepIndex switch
        {
            1 => new Vector2(rect.sizeDelta.x, 2300),
            _ => new Vector2(rect.sizeDelta.x, 800),
        };
        currentStep.UpdateStepDocumentWithData(newContent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(newContent.GetComponent<RectTransform>());

        string[] diags = currentStep.GetPossibleDiagnostics();
        for (int i = 0; i < _diagButtons.Length; i++)
        {
            if (i < diags.Length)
            {
                _diagButtons[i].gameObject.SetActive(true);
                _diagButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = diags[i];
            }
            else _diagButtons[i].gameObject.SetActive(false);
        }

        string[] actions = currentStep.GetPossibleActions();
        for (int i = 0; i < _diagButtons.Length; i++)
        {
            if (i < actions.Length)
            {
                _actionButtons[i].gameObject.SetActive(true);
                _actionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = actions[i];
            }
            else _actionButtons[i].gameObject.SetActive(false);
        }

        _diagButtonGroup.Reset();
        _actionButtonGroup.Reset();

        _confirmButton.interactable = false;
        _nextButton.interactable = false;

        UpdateConfirmButton();
    }

    private bool CheckDiagnosticsValidity(int selectedDiagIndex)
    {
        StepData currentStep = GetCurrentStep();
        bool isCorrect = currentStep.IsDiagnosticCorrect(selectedDiagIndex);
        return isCorrect;
    }

    private bool CheckActionsValidity(int selectedButtonIndex)
    {
        StepData currentStep = GetCurrentStep();
        bool isCorrect = currentStep.IsActionCorrect(selectedButtonIndex);
        return isCorrect;
    }

    public void CheckAnswersValidity()
    {
        int selectedDiagIndex = _diagButtonGroup.SelectedButtonIndex;
        int selectedActionIndex = _actionButtonGroup.SelectedButtonIndex;

        if (selectedDiagIndex == -1 || selectedActionIndex == -1)
        {
            Debug.LogError("No answer selected");
            return;
        }

        bool isDiagValid = CheckDiagnosticsValidity(selectedDiagIndex);
        bool isActionValid = CheckActionsValidity(selectedActionIndex);

        _diagButtonGroup.SetAnswerValidity(selectedDiagIndex, isDiagValid);
        _actionButtonGroup.SetAnswerValidity(selectedActionIndex, isActionValid);
        UpdateConfirmButton();

        if (isDiagValid && isActionValid)
        {
            WasCorrectlyAnswered = true;
            _confirmButton.interactable = false;
            _nextButton.interactable = true;
        }
    }

    private void UpdateConfirmButton()
    {
        _confirmButton.interactable = _diagButtonGroup.SelectedButtonIndex != -1 && _actionButtonGroup.SelectedButtonIndex != -1;
    }

    public bool IsLastStep()
    {
        return !GetCurrentStep().LeadsToNextStep();
    }
}
