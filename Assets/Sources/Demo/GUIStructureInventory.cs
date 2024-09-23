using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StructureInventory
{
    public List<StructureData> structures;

    [Serializable]
    public class StructureData
    {
        public GameObject model;
        public Sprite icon;
    }
}

public class GUIStructureInventory : MonoBehaviour
{
    [SerializeField] private StructureInventory _inventory;
    public Transform slotGroup;
    public GameObject dummySlot;
    public Transform placeBtn;

    public UnityAction<GameObject> placeObjectAction;

    private GUIStructureInventorySlot _selectedSlot;

    public void Initialize()
    {
        foreach(var data in _inventory.structures)
        {
            var slotObj = GameObject.Instantiate(dummySlot);
            slotObj.transform.SetParent(slotGroup, false);
            slotObj.SetActive(true);

            if(slotObj.TryGetComponent(out GUIStructureInventorySlot slot))
            {
                slot.SetStructure(data.model, data.icon);
                slot.ClickAction += OnSelectSlot;
            }
        }
    }

    private void OnSelectSlot(GUIStructureInventorySlot slot)
    {
        if (slot == null)
            return;
        if(_selectedSlot != null)
        {
            _selectedSlot.SetSelected(false);
        }
        _selectedSlot = slot;
        _selectedSlot.SetSelected(true);
    }

    public void OnPlace()
    {
        if (_selectedSlot == null)
            return;

        if(placeObjectAction != null)
        {
            placeObjectAction.Invoke(_selectedSlot.Model);
        }    
    }
}
