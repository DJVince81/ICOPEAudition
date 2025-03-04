using Assets.Scripts.Managers;
using UnityEngine;

public class MoneyGUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TextMeshProUGUI _moneyText;
    [SerializeField] private TMPro.TextMeshProUGUI _amountChangeText;
    [SerializeField] private Animation _animation;

    [Header("Colors")]
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Color _negativeColor;

    private void Start()
    {
        _moneyText.text = GameManager.Instance.Money.ToString();
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnMoneyChanged(int money, int amountChange)
    {
        _moneyText.text = money.ToString();
        string displayText = amountChange > 0 ? "+ " : "- ";
        displayText += Mathf.Abs(amountChange).ToString();
        _amountChangeText.text = displayText;
        _amountChangeText.color = amountChange > 0 ? _positiveColor : _negativeColor;

        _animation.Stop();
        _animation.Play();
    }
}