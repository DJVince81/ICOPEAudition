using TMPro;
using UnityEngine;

public class Step0Content : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _yesNoTexts;

    public void SetYesNoTexts(bool[] values)
    {
        if (values.Length != _yesNoTexts.Length)
        {
            Debug.LogError("The number of values doesn't match the number of texts");
            return;
        }

        for (int i = 0; i < values.Length; i++)
        {
            _yesNoTexts[i].text = values[i] ? "Oui" : "Non";
        }
    }
}
