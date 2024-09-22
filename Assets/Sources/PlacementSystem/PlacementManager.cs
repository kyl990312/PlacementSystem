using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public class PlacementManager : MonoBehaviour
    {
        protected PlacementObject _selectedObject;
        protected PlacementMap _currentMap;

        protected List<PlacementMap> _maps = new List<PlacementMap>();
        protected List<PlacementObject> _objects = new List<PlacementObject>();

        public void AddMap(PlacementMap map)
        {
            _maps.Add(map);
        }

        public virtual void PickObject(PlacementObject placementObject)
        {
            if(placementObject == null) return;
            _selectedObject = placementObject;
            _currentMap = GetCurrentMap(placementObject.Transform.position);

            _currentMap.ClearMap(placementObject);
            _selectedObject.SetObjectState(_currentMap.IsPlacable(_selectedObject, _selectedObject.Transform.position) ? Object_State.Placable : Object_State.Unplacable);
        }

        public virtual void DragObject(Vector3 worldPosition)
        {
            if(_selectedObject == null)
                return;
            if (!_currentMap.IsContains(worldPosition))
            {
                _currentMap = GetCurrentMap(worldPosition);
            }
            if(_currentMap == null)
            {
                worldPosition.y = _selectedObject.Transform.position.y;
                _selectedObject.Transform.position = worldPosition;
                return;
            }

            _selectedObject.Transform.position = _currentMap.GetObjectPosition(_selectedObject, worldPosition);

            _selectedObject.SetObjectState(_currentMap.IsPlacable(_selectedObject, _selectedObject.Transform.position) ? Object_State.Placable : Object_State.Unplacable);
            _selectedObject.SetObjectArea(_currentMap.GetObjectArea(_selectedObject));
        }

        public virtual void PutObject(PlacementObject placementObject, Vector3 worldPosition)
        {
            var map = GetCurrentMap(worldPosition);
            if (map == null)
            {
                worldPosition.y = placementObject.Transform.position.y;
                placementObject.Transform.position = worldPosition;
                return;
            }
            placementObject.Transform.position = map.GetObjectPosition(placementObject, worldPosition);
            UpdateMap(placementObject);

            placementObject.SetObjectState(Object_State.None);
            placementObject.SetObjectArea(map.GetObjectArea(placementObject));

            if (!_objects.Exists(x=>ReferenceEquals(x, placementObject)))
                _objects.Add(placementObject);
        }

        public bool IsPlacable(PlacementObject placementObject, Vector3 worldPosition)
        {
            if(placementObject == null) 
                return false;
            if (!_currentMap.IsContains(worldPosition))
            {
                _currentMap = GetCurrentMap(worldPosition);
            }
            if (_currentMap == null)
            {
                return false;
            }

            return _currentMap.IsPlacable(placementObject, worldPosition);
        }

        public void TurnObjectRight(PlacementObject placementObject)
        {
            if (placementObject == null)
                return;

            _currentMap = GetCurrentMap(placementObject.Transform.position);
            _currentMap?.ClearMap(placementObject);
            var angle = 90.0f;
            if (_currentMap != null)
            {
                angle = _currentMap.RotationUnit;
            }
            placementObject.Transform.Rotate(placementObject.RotateAxis * angle);
            UpdateMap(placementObject);
        }

        public void TurnObjectLeft(PlacementObject placementObject)
        {
            if (placementObject == null)
                return;

            _currentMap = GetCurrentMap(placementObject.Transform.position);
            _currentMap?.ClearMap(placementObject);
            var angle = -90.0f;
            if (_currentMap != null)
            {
                angle = -_currentMap.RotationUnit;
            }
            placementObject.Transform.Rotate(placementObject.RotateAxis * angle);
            UpdateMap(placementObject);
        }

        protected PlacementMap GetCurrentMap(Vector3 position)
        {
            foreach (PlacementMap map in _maps)
            {
                if(map.IsContains(position)) return map;
            }
            return null;
        }

        protected void UpdateMap(PlacementObject placementObject )
        {
            var map = GetCurrentMap(placementObject.Transform.position);
            if (map != null)
            {
                map.UpdateMap(placementObject);
                foreach (PlacementObject obj in _objects)
                {
                    if (ReferenceEquals(obj, placementObject))
                        continue;
                    if (map.IsOverlap(obj, placementObject))
                    {
                        map.UpdateMap(obj);
                    }
                }
            }
        }
    }
}
