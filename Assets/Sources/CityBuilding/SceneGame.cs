using PlacementSystem;
using UnityEngine;

public class SceneGame : SceneBase
{
    public CityPlacementManager placementManager;
    public InputController inputController;
    public CityCameraController cameraController;
    public LayerMask pickingLayer;

    private ObjectPicker _objectPicker = new ObjectPicker();

    public override void Initialize()
    {
        InitializeMaps();
        InitializeInput();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.Quit();
    }

    private void InitializeMaps()
    {
        placementManager.Initialize();
        var maps = FindObjectsOfType<PlacementMap>();
        foreach (PlacementMap map in maps)
        {
            map.Initialize();
            placementManager.AddMap(map);
        }
    }
    
    private void InitializeInput()
    {
        inputController.TouchDownAction += OnTouchDown;
        inputController.TouchUpAction += OnTouchUp;
        inputController.DragAction += OnDrag;
    }

    private void OnTouchDown(InputData data)
    {
        if (cameraController.GetCamera(0, out var camera))
        {
            if(_objectPicker != null)
            {
                _objectPicker.PickObjct(camera, data.position, pickingLayer);
                if (_objectPicker.PickedObject != null)
                {
                    if (_objectPicker.PickedObject.TryGetComponent(out PlacementObject placementObject))
                    {
                        placementManager.PickObject(placementObject);
                    }
                }
                else
                {
                    placementManager.ReleaseObject();
                }
            }
            else
            {
                placementManager.ReleaseObject();
            }
        }
    }

    private void OnTouchUp(InputData data)
    {
        if(_objectPicker != null)
        {
            if (_objectPicker.PickedObject != null)
            {
                if (_objectPicker.PickedObject.TryGetComponent(out PlacementObject placementObject))
                {
                    if (!placementManager.IsPlacable(placementObject, placementObject.Transform.position))
                        placementObject.Transform.position = _objectPicker.PickedPosition;
                    placementManager.PutObject(placementObject, placementObject.Transform.position);

                }
                _objectPicker.Release();
            }
        }

    }

    private void OnDrag(InputData data)
    {
        bool isInterupt = false;
        if (cameraController.GetCamera(0, out var camera))
        {
            if (_objectPicker != null)
            {
                if (_objectPicker.PickedObject != null)
                {
                    if (_objectPicker.PickedObject.TryGetComponent(out PlacementObject placementObject))
                    {
                        placementManager.DragObject(_objectPicker.DragObject(camera, data.position));
                        isInterupt = true;
                    }
                }
            }
        }

        if (!isInterupt)
        {
            cameraController.MoveCamera(new Vector3(data.Delta.x, 0.0f, data.Delta.y).normalized);
        }
    }
}
