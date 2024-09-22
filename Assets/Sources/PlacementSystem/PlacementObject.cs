using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlacementSystem
{
    public enum Object_State
    {
        None,
        Placable,
        Unplacable,
    }

    public class PlacementObject : MonoBehaviour
    {
        private PlacementObjectData _data;
        private Transform _transform;
        public Vector3 ObjectSize
        {
            get
            {
                return _data.size;
            }
        }
        public int CellState
        {
            get
            {
                return _data.cellState;
            }
        }
        public Vector3 RotateAxis
        {
            get
            {
                return Vector3.up;
            }
        }

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }
        public virtual void Initialize(PlacementObjectData data)
        {
            _data = data;
        }

        public virtual void SetObjectState(Object_State state) { }
    }
}

