using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _documentContentParent;
    [SerializeField] private Button[] _diagButtons;
    [SerializeField] private Button[] _actionButtons;
    [SerializeField] private ButtonGroup _diagButtonGroup;
    [SerializeField] private ButtonGroup _actionButtonGroup;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _nextButton;

    [Header("Money")]
    [SerializeField] private int _moneyAnsweredCorrectly = 10;
    [SerializeField] private int _moneyBonusFirstTry = 20;
    [SerializeField] private int _moneyPatientFinished = 50;


    private PatientData _patientData;
    private StepDataController[] _steps;
    private int _currentStepIndex = -1;

    private int _currentMoneyBonus = 0;

    public bool WasCorrectlyAnswered { get; private set; } = false;
    private bool _isInit = false;

    private void Start()
    {
        _diagButtonGroup.OnButtonSelected += UpdateConfirmButton;
        _actionButtonGroup.OnButtonSelected += UpdateConfirmButton;

        _confirmButton.onClick.AddListener(CheckAnswersValidity);
        //_nextButton.onClick.AddListener(GameManager.Instance.ChangeState);
        _nextButton.onClick.AddListener(GameManager.Instance.GameStateManager.GetNextStep);
    }

    internal void Initialize()
    {
        _patientData = ScriptableObject.CreateInstance<PatientData>();
        //int stepToReach = Random.Range(0, 5);
        _patientData.RandomizeData(GameManager.Instance.GameStateManager.GetNumberSteps());
        _steps = _patientData.GetStepDatas();

        _isInit = true;
    }

    private StepDataController GetCurrentStep()
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
        _currentMoneyBonus = _moneyBonusFirstTry;
    }
    private void DisplayStep()
    {
        GameManager.Instance.TelemetryManager.IncrNbShowSteps(_currentStepIndex);

        StepDataController currentStep = GetCurrentStep();

        // Remove all children of the parent in reverse order
        for (int i = _documentContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_documentContentParent.GetChild(i).gameObject);
        }

        GameObject newContent = Instantiate(currentStep.GetStepDocumentPrefab(), _documentContentParent);
        RectTransform rect = _documentContentParent.GetComponent<RectTransform>();
        rect.sizeDelta = _currentStepIndex switch
        {
            1 => new Vector2(rect.sizeDelta.x, 2800),
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

    private bool IsDiagnosticValid(int selectedDiagIndex)
    {
        StepDataController currentStep = GetCurrentStep();
        bool isCorrect = currentStep.IsDiagnosticCorrect(selectedDiagIndex);
        return isCorrect;
    }

    private bool IsActionValid(int selectedButtonIndex)
    {
        StepDataController currentStep = GetCurrentStep();
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

        bool isDiagValid = IsDiagnosticValid(selectedDiagIndex);
        bool isActionValid = IsActionValid(selectedActionIndex);

        // Update money
        int numberOfPossibleErrors = GetCurrentStep().GetPossibleDiagnostics().Length + GetCurrentStep().GetPossibleActions().Length - 2;
        if (!isDiagValid) _currentMoneyBonus -= _moneyBonusFirstTry / numberOfPossibleErrors;
        if (!isActionValid) _currentMoneyBonus -= _moneyBonusFirstTry / numberOfPossibleErrors;
        _currentMoneyBonus = Mathf.Max(0, _currentMoneyBonus);

        _diagButtonGroup.SetAnswerValidity(selectedDiagIndex, isDiagValid);
        _actionButtonGroup.SetAnswerValidity(selectedActionIndex, isActionValid);
        UpdateConfirmButton();

        RegisterPlayerAnswer(selectedActionIndex, selectedDiagIndex);

        if (isDiagValid && isActionValid)
        {
            WasCorrectlyAnswered = true;
            _confirmButton.interactable = false;
            _nextButton.interactable = true;

            float rand = Random.value;
            if (rand > 0.66f)
            {
                GameManager.Instance.AudioManager.PlaySFX("answer_correct1");
            }
            else if (rand > 0.33f)
            {
                GameManager.Instance.AudioManager.PlaySFX("answer_correct2");
            }
            else
            {
                GameManager.Instance.AudioManager.PlaySFX("answer_correct3");
            }
            // Give money
            int gain = _moneyAnsweredCorrectly + _currentMoneyBonus;
            if (IsLastStep()) gain += _moneyPatientFinished;
            GameManager.Instance.Money += gain;
        }
        else
        {
            if (!isDiagValid) GameManager.Instance.TelemetryManager.IncrNbLosesStepsDiag(_currentStepIndex);
            if (!isActionValid) GameManager.Instance.TelemetryManager.IncrNbLosesStepsAction(_currentStepIndex);
            GameManager.Instance.AudioManager.PlaySFX("answer_wrong");
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

    public void RegisterPlayerAnswer(int actionSelected,int diagnoticsSelected)
    {
        StepDataController currentStep = GetCurrentStep();

        //TODO
        string[] actionT = currentStep.GetPossibleActions();
        string[] diagsT = currentStep.GetPossibleActions();

        // WARNIG : OutOfBound -> Step : Video otoscopie
        GameManager.Instance.GameStateManager.RegiterError(actionT[actionSelected], diagsT[diagnoticsSelected]);
    }
}
