using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputData
{
    public bool touchHold;
    public Vector3 touchPosition;
    public Vector3 position;
    public Vector3 prevPosition;
    public Vector3 Delta
    {
        get
        {
            return position - prevPosition;
        }
    }
}

public class InputController : MonoBehaviour
{
    public EventSystem eventSystem;
    private InputData _inputData = new InputData();

    private UnityAction<InputData> _touchDownAction;
    private UnityAction<InputData> _touchUpAction;
    private UnityAction<InputData> _dragAction;

    public UnityAction<InputData>  TouchDownAction
    {
        get => _touchDownAction;
        set => _touchDownAction = value;
    }
    public UnityAction<InputData> TouchUpAction
    {
        get => _touchUpAction;
        set => _touchUpAction = value;
    }
    public UnityAction<InputData> DragAction
    {
        get => _dragAction;
        set=> _dragAction = value;

    }

    void Update()
    {
        if (eventSystem.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _inputData.prevPosition = _inputData.position;
            _inputData.position = Input.mousePosition;
            _inputData.touchPosition = Input.mousePosition;
            _inputData.touchHold = true;
            if (_touchDownAction != null)
            {
                _touchDownAction(_inputData);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _inputData.prevPosition = _inputData.position;
            _inputData.position = Input.mousePosition;
            _inputData.touchPosition = Vector3.one * -10000;
            _inputData.touchHold = false;
            if (_touchUpAction != null)
            {
                _touchUpAction(_inputData);
            }
        }
        if (_inputData.touchHold)
        {
            _inputData.prevPosition = _inputData.position;
            _inputData.position = Input.mousePosition;
            if (_dragAction != null)
            {
                _dragAction(_inputData);
            }
        }
    }

}
