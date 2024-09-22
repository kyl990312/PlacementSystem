using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTileData
{
    public int id;
    public Sprite icon;
    public GameObject prefab;
}

public class PlacementTileBrush : MonoBehaviour
{
    public SpriteRenderer iconRenderer;
    private int _id;
}
