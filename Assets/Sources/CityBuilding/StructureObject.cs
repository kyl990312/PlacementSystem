using PlacementSystem;
using UnityEngine;

public class StructureObject : PlacementObject
{
    static int SHADER_PROP_HASH_COLOR = Shader.PropertyToID("_BaseColor");
    private Material _placableMaterial;
    private Transform _areaObject;

    public override void Initialize(PlacementObjectData data)
    {
        base.Initialize(data);

        if(ResourcesLoader.Load("Materials/Placable", out Material placableMat))
        {
            var renderer = GetComponentInChildren<MeshRenderer>();
            var materials = new Material[renderer.materials.Length + 1];
            for(int i = 0;i < renderer.materials.Length; i++)
            {
                materials[i] = renderer.materials[i];
            }
            materials[materials.Length-1] = Instantiate(placableMat);
            renderer.materials = materials;
            _placableMaterial = materials[materials.Length - 1];
        }
    }

    public void SetArea(Transform areaObject)
    {
        _areaObject = areaObject;
    }

    public override void SetObjectState(Object_State state)
    {
        base.SetObjectState(state);
        switch (state)
        {
            case Object_State.None:
                _placableMaterial.SetColor(SHADER_PROP_HASH_COLOR, Color.clear);
                break;
            case Object_State.Placable:
                {
                    Color color = Color.blue;
                    color.a = 0.4f;
                    _placableMaterial.SetColor(SHADER_PROP_HASH_COLOR, color);
                }
                break;
            case Object_State.Unplacable:
                {
                    Color color = Color.red;
                    color.a = 0.4f;
                    _placableMaterial.SetColor(SHADER_PROP_HASH_COLOR, color);
                }
                break;

                
        }
    }

    public override void SetObjectArea(Vector3 size)
    {
        base.SetObjectArea(size);
        if (_areaObject == null)
            return;
        size.x /= transform.lossyScale.x;
        size.z /= transform.lossyScale.y;

        _areaObject.localScale = new Vector3(size.x, size.z, 1.0f);
    }
}
