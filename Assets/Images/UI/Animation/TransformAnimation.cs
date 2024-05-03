using UnityEngine;

public class TransformAnimation : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Vector3 _transform = Vector3.zero;
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private Vector3 _scale = Vector3.zero;

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * _transform);
        transform.Rotate(_rotation, _speed * Time.deltaTime);
        transform.localScale += _speed * Time.deltaTime * _scale;
    }
}
