using TMPro;
using UnityEngine;

public class Step3Content : MonoBehaviour
{
    [SerializeField] private TMP_Text _weberResultText;
    [SerializeField] private TMP_Text _affectedEarText;

    public void SetWeberResultText(WeberTestResult result)
    {
        switch (result)
        {
            case WeberTestResult.Center:
                _weberResultText.text = "Le son est perçu au centre";
                break;
            case WeberTestResult.RightEar:
                _weberResultText.text = "Le son est perçu à droite";
                break;
            case WeberTestResult.LeftEar:
                _weberResultText.text = "Le son est perçu à gauche";
                break;
        }
    }

    public void SetAffectedEarText(Ear affectedEar)
    {
        switch (affectedEar)
        {
            case Ear.RightEar:
                _affectedEarText.text = "Oreille droite";
                break;
            case Ear.LeftEar:
                _affectedEarText.text = "Oreille gauche";
                break;
        }
    }
}
