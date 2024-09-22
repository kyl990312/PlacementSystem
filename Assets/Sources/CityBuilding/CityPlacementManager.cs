using PlacementSystem;
using UnityEngine;

public class CityPlacementManager : PlacementManager
{
    public GUIStructureInventory guiInventory;
    public GUICityPlacementButtonGroup guiButtonGroup;
    public GameObject objectAreaPrefab;

    public void Initialize()
    {
        guiInventory.Initialize();
        guiInventory.placeObjectAction += OnPlace;

        guiButtonGroup.turnLeftAction += OnTurnLeft;
        guiButtonGroup.turnRightAction += OnTurnRight;
        guiButtonGroup.unplaceAction += OnUnplace;

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

        var areaObj = Instantiate(objectAreaPrefab);
        areaObj.transform.SetParent(instance.transform, false);
        areaObj.transform.localScale = new Vector3(size.x, 1.0f, size.z);
        areaObj.transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
        areaObj.SetActive(true);
        placementObject.SetArea(areaObj.transform);

        PutObject(placementObject, Vector3.zero);
        _selectedObject = placementObject;
        _currentMap = GetCurrentMap(placementObject.Transform.position);
    }

    public override void PickObject(PlacementObject placementObject)
    {
        base.PickObject(placementObject);
        SetVisiblePlacementButtons(false);
    }

    public override void PutObject(PlacementObject placementObject, Vector3 worldPosition)
    {
        base.PutObject(placementObject, worldPosition);
        guiButtonGroup.SetPosition(placementObject, new Vector3(0.0f, 0.1f, 0.0f));
        SetVisiblePlacementButtons(true);
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

    private void OnTurnLeft()
    {
        if (_selectedObject == null)
            return;
        TurnObjectLeft(_selectedObject);
    }

    private void OnTurnRight()
    {
        if (_selectedObject == null)
            return;
        TurnObjectRight(_selectedObject);
    }
    private void OnUnplace()
    {
        if (_selectedObject == null)
            return;

        _currentMap = GetCurrentMap(_selectedObject.Transform.position);
        _currentMap.ClearMap(_selectedObject);
        Unplace(_selectedObject);
        DestroyImmediate(_selectedObject.gameObject);
        _selectedObject = null;
        SetVisiblePlacementButtons(_selectedObject != null);
    }
}
