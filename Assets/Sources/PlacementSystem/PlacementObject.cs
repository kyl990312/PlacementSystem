using UnityEngine;
using UnityEngine.Events;

namespace PlacementSystem
{
    public enum Object_State
    {
        None,
        Placable,
        Unplacable,
    }

    [RequireComponent(typeof(BoxCollider))]
    public class PlacementObject : MonoBehaviour
    {
        [SerializeField]
        private PlacementObjectData _data;
        private Transform _transform;
        private BoxCollider _boxCollider;
        public virtual Vector3 ObjectSize
        {
            get
            {
                if(_boxCollider == null)
                    _boxCollider = GetComponent<BoxCollider>();
                if(_boxCollider == null )
                    return Vector3.zero;
                return _boxCollider.size;
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

        public UnityAction<PlacementObject> OnInactive;

        public PlacementObjectData data
        {
            set => _data = value;
        }

        private void OnDisable()
        {
            if(OnInactive != null)
                OnInactive(this);
        }

        public virtual void SetObjectState(Object_State state) { }

        public virtual bool IsPlacableState(int state)
        {
            if (_data.unplacableCellState == null)
                return true;
            return !_data.unplacableCellState.Contains(state);
        }
    }
}

