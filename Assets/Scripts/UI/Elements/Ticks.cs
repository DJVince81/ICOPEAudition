using UnityEngine;
using UnityEngine.UI;

public class Ticks : MonoBehaviour
{
    [SerializeField] private Image _yesTickChecked;
    [SerializeField] private Image _yesTickUnchecked;
    [SerializeField] private Image _noTickChecked;
    [SerializeField] private Image _noTickUnchecked;

    public void DisplayValue(bool value)
    {
        _yesTickChecked.gameObject.SetActive(value);
        _yesTickUnchecked.gameObject.SetActive(!value);
        _noTickChecked.gameObject.SetActive(!value);
        _noTickUnchecked.gameObject.SetActive(value);
    }
}
