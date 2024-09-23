using PlacementSystem;
using System.Collections.Generic;
using UnityEngine;

public class CityPlacementManager : PlacementManager
{
    public GUIStructureInventory guiInventory;
    public GUICityPlacementButtonGroup guiButtonGroup;
    public GameObject objectAreaPrefab;

    private List<KeyValuePair<GameObject, Transform>> _objectAreas;
    private GameObjectPool _objectAreaPool;

    public void Initialize()
    {
        guiInventory.Initialize();
        guiInventory.placeObjectAction += OnPlace;

        guiButtonGroup.turnLeftAction += OnTurnLeft;
        guiButtonGroup.turnRightAction += OnTurnRight;
        guiButtonGroup.unplaceAction += OnUnplace;

        _objectAreas = new List<KeyValuePair<GameObject, Transform>>();
        _objectAreaPool = new GameObjectPool(objectAreaPrefab);

        SetVisiblePlacementButtons(false);
    }

    private void OnPlace(GameObject gObj)
    {
        var instance = Instantiate(gObj);

        Vector3 size = Vector3.one;
        if(instance.TryGetComponent(out BoxCollider collider))
        {
            size = collider.size;
        }
        if(!instance.TryGetComponent(out StructureObject placementObject))
        {
            placementObject = instance.AddComponent<StructureObject>();
        }
        placementObject.Initialize(new PlacementObjectData()
        {
            cellState = 2,
            size = size
        });

        var areaObj = _objectAreaPool.Get();
        if (areaObj != null)
        {
            areaObj.transform.localScale = new Vector3(size.x, 1.0f, size.z);
            areaObj.transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
            areaObj.SetActive(true);
            _objectAreas.Add(new KeyValuePair<GameObject, Transform>(placementObject.gameObject, areaObj.transform));
        }

        PutObject(placementObject, Vector3.zero);
        _selectedObject = placementObject;
        _currentMap = GetCurrentMap(placementObject.Transform.position);
    }

    public override void PickObject(PlacementObject placementObject)
    {
        base.PickObject(placementObject);
        SetVisiblePlacementButtons(false);
    }

    public override void DragObject(Vector3 worldPosition)
    {
        base.DragObject(worldPosition);
        SetArea(_selectedObject);
    }

    public override void PutObject(PlacementObject placementObject, Vector3 worldPosition)
    {
        base.PutObject(placementObject, worldPosition);
        guiButtonGroup.SetPosition(placementObject, new Vector3(0.0f, 0.1f, 0.0f));
        SetVisiblePlacementButtons(true);
        SetArea(placementObject);
    }

    public void ReleaseObject()
    {
        _selectedObject = null;
        SetVisiblePlacementButtons(false);
        _currentMap = null;
    }

    public void SetVisiblePlacementButtons(bool visible)
    {
        guiButtonGroup.gameObject.SetActive(visible);
    }

    private void SetArea(PlacementObject placementObject)
    {
        if (placementObject == null)
            return;
        var map = GetCurrentMap(placementObject.Transform.position);
        var size = map.GetObjectArea(placementObject);
        var areaObject = _objectAreas.Find(x => ReferenceEquals(x.Key, placementObject.gameObject));
        if (areaObject.Value == null)
            return;
        areaObject.Value.localScale = new Vector3(size.x, size.z, 0.0f);
        areaObject.Value.position = placementObject.Transform.position;
    }

    private void OnTurnLeft()
    {
        if (_selectedObject == null)
            return;
        TurnObjectLeft(_selectedObject);
        SetArea(_selectedObject);
    }

    private void OnTurnRight()
    {
        if (_selectedObject == null)
            return;
        TurnObjectRight(_selectedObject);
        SetArea(_selectedObject);
    }
    private void OnUnplace()
    {
        if (_selectedObject == null)
            return;

        _currentMap = GetCurrentMap(_selectedObject.Transform.position);
        _currentMap.ClearMap(_selectedObject);
        Unplace(_selectedObject);
        int index = _objectAreas.FindIndex(x => ReferenceEquals(x.Key, _selectedObject.gameObject));
        if (index>=0)
        {
            if(_objectAreas[index].Value != null)
            {
                _objectAreas[index].Value.gameObject.SetActive(false);
                _objectAreaPool.Return(_objectAreas[index].Value.gameObject);
            }
            _objectAreas.RemoveAt(index);

        }
        DestroyImmediate(_selectedObject.gameObject);
        _selectedObject = null;
        SetVisiblePlacementButtons(_selectedObject != null);

    }
}
