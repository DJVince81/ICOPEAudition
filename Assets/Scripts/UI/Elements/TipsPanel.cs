using UnityEngine;

public class TipsPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] _tipsGameObjects;

    public void Display()
    {
        gameObject.SetActive(true);

        foreach (GameObject tip in _tipsGameObjects)
        {
            tip.SetActive(false);
        }

        int randomIndex = Random.Range(0, _tipsGameObjects.Length);
        _tipsGameObjects[randomIndex].SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
