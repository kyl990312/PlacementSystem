using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker
{
    private Transform _pickedObject;
    private Vector3 _pickedPosition;

    public GameObject PickedObject {
        get
        {
            if (_pickedObject == null)
                return null;
            return _pickedObject.gameObject;
        }
    }
    public Vector3 PickedPosition => _pickedPosition;

    public bool PickObjct(Camera camera, Vector3 screenPosition, LayerMask layerMask)
    {
        if(Physics.Raycast(camera.ScreenPointToRay(screenPosition), out var hit, Mathf.Infinity, layerMask))
        {
            _pickedObject = hit.collider.transform;
            _pickedPosition = _pickedObject.position;
            return true;
        }

        return false;
    }

    public Vector3 DragObject(Camera camera, Vector3 screenPositon)
    {
        if (_pickedObject == null)
            return Vector3.zero;
        screenPositon.z = camera.WorldToScreenPoint(_pickedObject.transform.position).z;
        return camera.ScreenToWorldPoint(screenPositon);
    }

    public void Release()
    {
        _pickedObject = null;
        _pickedPosition = Vector3.zero;
    }
}
