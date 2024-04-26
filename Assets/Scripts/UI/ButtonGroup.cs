using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    public int SelectedButtonIndex { get; private set; }

    private Button[] _buttons;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<Button>();
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
        ResetButtons();
    }

    private void OnButtonClicked(Button clickedButton)
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = _buttons[i] != clickedButton;
            if (!_buttons[i].interactable)
            {
                SelectedButtonIndex = i;
            }
        }
    }

    public void ResetButtons()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = true;
        }
        SelectedButtonIndex = -1;
    }
}
