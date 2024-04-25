using UnityEngine;

public class Step1Content : MonoBehaviour
{
    [SerializeField] private Ticks[] _ticks;

    public void SetTicks(bool[] values)
    {
        if (values.Length != _ticks.Length)
        {
            Debug.LogError("The number of values doesn't match the number of ticks");
            return;
        }

        for (int i = 0; i < values.Length; i++)
        {
            _ticks[i].DisplayValue(values[i]);
        }
    }
}
