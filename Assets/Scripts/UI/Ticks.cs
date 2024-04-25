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
        _yesTickChecked.enabled = value;
        _yesTickUnchecked.enabled = !value;
        _noTickChecked.enabled = !value;
        _noTickUnchecked.enabled = value;
    }
}
