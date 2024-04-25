using UnityEngine;
using UnityEngine.UI;

public class Step4Content : MonoBehaviour
{
    [SerializeField] private Image _normalAudiogramImage;
    [SerializeField] private Image _asymmetryImage;
    [SerializeField] private Image _invertedSymmetryImage;
    [SerializeField] private Image _perceptionSymmetryImage;
    [SerializeField] private Image _transmissionImage;

    public void DisplayAudiogramImage(AudiometryResult result)
    {
        _normalAudiogramImage.gameObject.SetActive(false);
        _asymmetryImage.gameObject.SetActive(false);
        _invertedSymmetryImage.gameObject.SetActive(false);
        _perceptionSymmetryImage.gameObject.SetActive(false);
        _transmissionImage.gameObject.SetActive(false);

        switch (result)
        {
            case AudiometryResult.NormalAudiogram:
                _normalAudiogramImage.gameObject.SetActive(true);
                break;
            case AudiometryResult.Asymmetry:
                _asymmetryImage.gameObject.SetActive(true);
                break;
            case AudiometryResult.InvertedSymmetry:
                _invertedSymmetryImage.gameObject.SetActive(true);
                break;
            case AudiometryResult.PerceptionSymmetry:
                _perceptionSymmetryImage.gameObject.SetActive(true);
                break;
            case AudiometryResult.Transmission:
                _transmissionImage.gameObject.SetActive(true);
                break;
        }
    }
}
