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

    private PatientData _patientData;
    private StepData[] _steps;

    private bool _isInit = false;

    internal void Initialize()
    {
        _patientData = ScriptableObject.CreateInstance<PatientData>();

        _patientData.RandomizeData(4);

        _steps = _patientData.GetStepDatas();

        _isInit = true;
    }

    internal void LoadStep(int e)
    {
        if (_isInit) DisplayStep(e % _steps.Length);
    }
    private void DisplayStep(int stepIndex)
    {
        Debug.Log($"Displaying step {stepIndex}");

        StepData currentStep = _steps[stepIndex];

        // Remove all children of the parent in reverse order
        for (int i = _documentContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_documentContentParent.GetChild(i).gameObject);
        }

        GameObject newContent = Instantiate(currentStep.GetStepDocumentPrefab(), _documentContentParent);
        RectTransform rect = _documentContentParent.GetComponent<RectTransform>();
        rect.sizeDelta = stepIndex switch
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

        _diagButtonGroup.ResetButtons();
        _actionButtonGroup.ResetButtons();
    }
}
