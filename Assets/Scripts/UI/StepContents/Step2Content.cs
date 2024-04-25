using UnityEngine;
using UnityEngine.UI;

public class Step2Content : MonoBehaviour
{
    [SerializeField] private Image _normalEardrumImage;
    [SerializeField] private Image _ductPathologyImage;
    [SerializeField] private Image _CerumenImpactionImage;
    [SerializeField] private Image _tympanicPathologyImage;

    public void DisplayVideoOtoscopyImage(VideoOtoscopyResult result)
    {
        _normalEardrumImage.gameObject.SetActive(false);
        _ductPathologyImage.gameObject.SetActive(false);
        _CerumenImpactionImage.gameObject.SetActive(false);
        _tympanicPathologyImage.gameObject.SetActive(false);

        switch (result)
        {
            case VideoOtoscopyResult.NormalEardrum:
                _normalEardrumImage.gameObject.SetActive(true);
                break;
            case VideoOtoscopyResult.DuctPathology:
                _ductPathologyImage.gameObject.SetActive(true);
                break;
            case VideoOtoscopyResult.CerumenImpaction:
                _CerumenImpactionImage.gameObject.SetActive(true);
                break;
            case VideoOtoscopyResult.TympanicPathology:
                _tympanicPathologyImage.gameObject.SetActive(true);
                break;
        }
    }
}
