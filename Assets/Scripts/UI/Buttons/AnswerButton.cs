using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class AnswerButton : MonoBehaviour
{
    public bool IsConfirmed { get; private set; }
    public Button AssociatedButton
    {
        get { return _button; }
    }
    private Button _button;
    private TMP_Text _text;
    private Image _image;

    [Header("Colors")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _correctColor;
    [SerializeField] private Color _incorrectColor;

    void Awake()
    {
        _button = gameObject.GetComponent<Button>();
        _text = gameObject.GetComponentInChildren<TMP_Text>();
        _image = gameObject.GetComponent<Image>();
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }

    public void Reset()
    {
        IsConfirmed = false;
        _image.color = _normalColor;
        SetInteractable(true);
    }

    public void SetCorrect()
    {
        IsConfirmed = true;
        _image.color = _correctColor;
        SetInteractable(false);
    }

    public void SetIncorrect()
    {
        IsConfirmed = true;
        _image.color = _incorrectColor;
        SetInteractable(false);
    }
}
