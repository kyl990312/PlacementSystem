using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCameraController : MonoBehaviour
{
    [SerializeField] private List<Camera> _cameras;
    [SerializeField] private float _sensitivity = 1.0f;
    [SerializeField] private float _maxDistance = 1.0f;

    private Vector3 _initialPosition;
    private Transform _transform;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _transform = transform;
        _initialPosition = _transform.position;
    }

    public bool GetCamera(int index, out Camera camera)
    {
        camera = null;
        if(index < 0)
            return false;
        if(index >= _cameras.Count)
            return false;
        camera = _cameras[index];
        return camera != null;
    }

    public void MoveCamera(Vector3 direction)
    {
        Vector3 position = _transform.position + direction * _sensitivity;
        if(Vector3.Distance(position, _initialPosition) < _maxDistance)
        {
            _transform.position = position;
        }
    }
}
