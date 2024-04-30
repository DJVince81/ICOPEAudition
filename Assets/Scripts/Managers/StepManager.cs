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
    private int _currentStepIndex = 0;

    public bool WasCorrectlyAnswered { get; private set; } = false;
    private bool _isInit = false;

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
        Debug.Log($"Displaying step {_currentStepIndex}/{_steps.Length - 1}");

        StepData currentStep = GetCurrentStep();

        if (currentStep.IsEndStep)
        {
            GameManager.Instance.ChangeState();
        }

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

        _confirmButton.interactable = true;
        _nextButton.interactable = false;
    }

    private bool CheckDiagnosticsValidity()
    {
        StepData currentStep = GetCurrentStep();
        if (_diagButtonGroup.SelectedButtonIndex == -1) return false;
        int selecedButtonIndex = _diagButtonGroup.SelectedButtonIndex;
        bool isCorrect = currentStep.IsDiagnosticCorrect(selecedButtonIndex);
        _diagButtonGroup.SetAnswerValidity(selecedButtonIndex, isCorrect);
        return isCorrect;
    }

    private bool CheckActionsValidity()
    {
        StepData currentStep = GetCurrentStep();
        if (_actionButtonGroup.SelectedButtonIndex == -1) return false;
        int selecedButtonIndex = _actionButtonGroup.SelectedButtonIndex;
        bool isCorrect = currentStep.IsActionCorrect(selecedButtonIndex);
        _actionButtonGroup.SetAnswerValidity(selecedButtonIndex, isCorrect);
        return isCorrect;
    }

    public void CheckAnswersValidity()
    {
        bool isDiagValid = CheckDiagnosticsValidity();
        bool isActionValid = CheckActionsValidity();
        if (isDiagValid && isActionValid)
        {
            WasCorrectlyAnswered = true;
            _confirmButton.interactable = false;
            _nextButton.interactable = true;
        }
    }
}
