using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public class PlacementController : MonoBehaviour
    {
        protected PlacementObject _selectedObject;
        protected PlacementMap _currentMap;

        protected List<PlacementMap> _maps = new List<PlacementMap>();
        protected List<PlacementObject> _objects = new List<PlacementObject>();

        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            var maps = GetComponentsInChildren<PlacementMap>();
            foreach (var map in maps)
            {
                AddMap(map);
            }

            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                if(_objects[i] == null)
                    _objects.RemoveAt(i);
            }
        }

        public void AddMap(PlacementMap map)
        {
            if(_maps.Exists(x=>ReferenceEquals(x, map))) 
                return;
            map.Initialize();
            _maps.Add(map);
        }

        public virtual void PickObject(PlacementObject placementObject)
        {
            if(placementObject == null) return;
            _selectedObject = placementObject;
            _currentMap = GetCurrentMap(placementObject.Transform.position);
            if (_currentMap == null)
                return;

            _currentMap.ClearMap(placementObject);
            UpdateMapOverlap(placementObject);
            _selectedObject.SetObjectState(_currentMap.IsPlacable(_selectedObject, _selectedObject.Transform.position) ? Object_State.Placable : Object_State.Unplacable);
        }

        public virtual void DragObject(Vector3 worldPosition)
        {
            if(_selectedObject == null)
                return;
            if (_currentMap != null)
            {
                if (!_currentMap.IsContains(worldPosition))
                {
                    _currentMap = GetCurrentMap(worldPosition);
                }
            }
            else
            {
                _currentMap = GetCurrentMap(worldPosition);
            }

            if (_currentMap == null)
            {
                worldPosition.y = _selectedObject.Transform.position.y;
                _selectedObject.Transform.position = worldPosition;
                return;
            }

            _selectedObject.Transform.position = _currentMap.GetObjectPosition(_selectedObject, worldPosition);

            _selectedObject.SetObjectState(_currentMap.IsPlacable(_selectedObject, _selectedObject.Transform.position) ? Object_State.Placable : Object_State.Unplacable);
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
            map.UpdateMap(placementObject);

            placementObject.SetObjectState(Object_State.None);
            placementObject.OnInactive += Unplace;
        }

        public virtual void Unplace(PlacementObject placementObject)
        {
            if (placementObject == null)
                return;

            GetCurrentMap(placementObject.Transform.position)?.ClearMap(placementObject);

            int index = _objects.FindIndex(x=>ReferenceEquals(x,placementObject));
            if(index >= 0)
            {
                _objects.RemoveAt(index);
            }
            placementObject.OnInactive += Unplace;
        }

        protected virtual void AddPlacementObject(PlacementObject placementObject)
        {
            if (!_objects.Exists(x => ReferenceEquals(x, placementObject)))
                _objects.Add(placementObject);
        }

        public bool IsPlacable(PlacementObject placementObject, Vector3 worldPosition)
        {
            if(placementObject == null) 
                return false;
            if(_currentMap != null)
            {
                if (!_currentMap.IsContains(worldPosition))
                {
                    _currentMap = GetCurrentMap(worldPosition);
                }
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
            UpdateMapOverlap(placementObject);
            var angle = 90.0f;
            if (_currentMap != null)
            {
                angle = _currentMap.RotationUnit;
            }
            placementObject.Transform.Rotate(placementObject.RotateAxis * angle);
            _currentMap?.UpdateMap(placementObject);
        }

        public void TurnObjectLeft(PlacementObject placementObject)
        {
            if (placementObject == null)
                return;

            _currentMap = GetCurrentMap(placementObject.Transform.position);
            _currentMap?.ClearMap(placementObject);
            UpdateMapOverlap(placementObject);
            var angle = -90.0f;
            if (_currentMap != null)
            {
                angle = -_currentMap.RotationUnit;
            }
            placementObject.Transform.Rotate(placementObject.RotateAxis * angle);
            _currentMap?.UpdateMap(placementObject);
        }

        protected PlacementMap GetCurrentMap(Vector3 position)
        {
            foreach (PlacementMap map in _maps)
            {
                if(map.IsContains(position)) return map;
            }
            return null;
        }

        protected void UpdateMapOverlap(PlacementObject placementObject )
        {
            var map = GetCurrentMap(placementObject.Transform.position);
            if (map != null)
            {
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
