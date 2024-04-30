using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Item : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _objectToDisplay;

    private Button _button;

    void Start()
    {
        _priceText.text = _price.ToString();
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(Buy);
    }

    public void Buy()
    {
        if (GameManager.Instance.Money >= _price)
        {
            GameManager.Instance.Money -= _price;
            _objectToDisplay.SetActive(true);
            _button.interactable = false;
        }
    }
}
