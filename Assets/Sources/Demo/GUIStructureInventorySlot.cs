using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIStructureInventorySlot : MonoBehaviour
{
    public Image icon;
    public Image selected;
    private GameObject _model;

    public UnityAction<GUIStructureInventorySlot> ClickAction;
    public GameObject Model
    {
        get => _model;
    }

    public void SetStructure(GameObject model, Sprite iconSprite)
    {
        _model = model;
        icon.sprite = iconSprite;
    }

    public void SetSelected(bool isSelected)
    {
        selected.color = isSelected ? Color.gray : Color.clear;
    }

    public void OnClick()
    {
        if (ClickAction != null)
        {
            ClickAction(this);
        }
    }
}
