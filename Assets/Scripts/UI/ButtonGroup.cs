using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    [HideInInspector] public int selectedButtonIndex = -1;

    private Button[] _buttons;

    private void Awake()
    {
        _buttons = GetComponentsInChildren<Button>();
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        foreach (Button button in _buttons)
        {
            button.interactable = button == clickedButton;
        }
    }

    public void ResetButtons()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = true;
        }
    }
}
