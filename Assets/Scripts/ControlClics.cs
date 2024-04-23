using TMPro;
using UnityEngine;

public class ControlClics : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public void ShowObject(GameObject clicked)
    {
        _tmp.text = clicked.name + " " + clicked.GetComponentInChildren<TextMeshProUGUI>().text;
    }
}
