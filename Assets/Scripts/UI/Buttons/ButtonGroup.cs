using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    public event System.Action OnButtonSelected;

    public int SelectedButtonIndex { get; private set; }

    private AnswerButton[] _answerButtons = new AnswerButton[0];

    private void Start()
    {
        _answerButtons = GetComponentsInChildren<AnswerButton>(true);
        foreach (AnswerButton answerButton in _answerButtons)
        {
            answerButton.AssociatedButton.onClick.AddListener(() => OnButtonClicked(answerButton));
        }

        Reset();
    }

    private void OnButtonClicked(AnswerButton clickedButton)
    {
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].SetInteractable(_answerButtons[i] != clickedButton && !_answerButtons[i].IsConfirmed);
            if (_answerButtons[i] == clickedButton)
            {
                SelectedButtonIndex = i;
            }
        }
        OnButtonSelected?.Invoke();
    }

    public void Reset()
    {
        foreach (AnswerButton answerButton in _answerButtons) answerButton.Reset();
        SelectedButtonIndex = -1;
    }

    public void SetRightAnswer(int index)
    {
        _answerButtons[index].SetCorrect();
        foreach (AnswerButton answerButton in _answerButtons)
        {
            answerButton.SetInteractable(false);
        }
    }

    public void SetWrongAnswer(int index)
    {
        _answerButtons[index].SetIncorrect();
        SelectedButtonIndex = -1;
    }

    public void SetAnswerValidity(int index, bool isCorrect)
    {
        if (index < 0 || index >= _answerButtons.Length)
        {
            Debug.LogError("Button index out of range");
            return;
        }
        if (isCorrect) SetRightAnswer(index);
        else SetWrongAnswer(index);
    }
}
