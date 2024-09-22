using PlacementSystem;
using UnityEngine;
using UnityEngine.Events;

public class GUICityPlacementButtonGroup : MonoBehaviour
{
    [SerializeField] Camera _camera;
    public UnityAction turnRightAction;
    public UnityAction turnLeftAction;
    public UnityAction unplaceAction;

    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;

    public void SetPosition(PlacementObject structure, Vector3 offset)
    {
        if(_rectTransform == null)
        {
            _rectTransform = transform as RectTransform;
        }
        if (_parentRectTransform == null)
        {
            _parentRectTransform = transform.parent as RectTransform;
        }

        Vector3 position = structure.Transform.position;
        position.y += structure.ObjectSize.y;
        position += offset;

        // world to screen
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform,
            RectTransformUtility.WorldToScreenPoint(_camera, position),
            null,
            out var localPoint);
        _rectTransform.anchoredPosition = localPoint;
    }

    public void OnTurnRightButton()
    {
        if (turnRightAction != null)
        {
            turnRightAction();
        }
    }

    public void OnTurnLeftButton()
    {
        if (turnLeftAction != null)
        {
            turnLeftAction();
        }
    }
    public void OnUnplaceButton()
    {
        if(unplaceAction != null)
        {
            unplaceAction();
        }
    }
}
