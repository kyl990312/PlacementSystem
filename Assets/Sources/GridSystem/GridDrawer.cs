using UnityEngine;

namespace GridSystem.Component
{
    public class GridDrawer : MonoBehaviour
    {
        private static readonly int HASH_LINE_COLOR = Shader.PropertyToID("_LineColor");
        private static readonly int HASH_LINE_THICKNESS = Shader.PropertyToID("_LineThickness");
        private static readonly int HASH_CELL_COUNT = Shader.PropertyToID("_CellCount");
        private static readonly int HASH_BG_ALPHA= Shader.PropertyToID("_BgAlpha");
        private static readonly int HASH_LINE_ALPHA= Shader.PropertyToID("_LineAlpha");
        private static readonly string PATH_SHADER = "Shader Graphs/GridShader";

        private static Mesh _gridMeshShared;
        private static Material _gridMaterialShared;

        public GridDrawerSettings settings;

        private GridReader _gridViewer;
        private Vector3 _prevCenter;
        private bool _inited = false;
        private Transform _gridPlane;
        private Material _gridMaterial;

        private void Update()
        {
            if (_gridViewer == null)
                return;
            if (!_inited)
            {
                Initialize();
            }
            if (!_inited)
                return;

            if (Vector3.Distance(_gridViewer.Center, _prevCenter) > 0.1f)
            {
                DrawGrid();
                _prevCenter = _gridViewer.Center;
            }
        }

        public void SetGrid(GridReader gridViewer)
        {
            _gridViewer = gridViewer; 
        }

        public virtual void Initialize()
        {
            SetRenderer();
            _inited = true;
        }

        public virtual void DrawGrid()
        {
            if (_gridViewer == null)
                return;

            if (_gridMaterial != null)
            {
                _gridMaterial.SetVector(HASH_CELL_COUNT, new Vector4(_gridViewer.CellCount.x, _gridViewer.CellCount.y, 0.0f, 0.0f));
            }
            if(_gridPlane != null)
            {
                _gridPlane.localScale = new Vector3(_gridViewer.CellCount.x * _gridViewer.CellSize.x, _gridViewer.CellCount.y * _gridViewer.CellSize.y, 1.0f);
                _gridPlane.localPosition = new Vector3(
                    _gridViewer.CellSize.x * ( _gridViewer.CellCount.x % 2 == 0 ? 0.5f : 0.0f),
                    0.0f,
                    _gridViewer.CellSize.y * (_gridViewer.CellCount.y % 2 == 0 ? 0.5f : 0.0f)) * -1f;
            }
        }

        private void SetRenderer()
        {
            if (_gridMeshShared == null)
            {
                // Mesh 생성
                _gridMeshShared = new Mesh();
                _gridMeshShared.name = "Quad";

                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, 0), // Bottom-left
                    new Vector3(0.5f, -0.5f, 0),  // Bottom-right
                    new Vector3(-0.5f, 0.5f, 0),  // Top-left
                    new Vector3(0.5f, 0.5f, 0)    // Top-right
                };

                int[] triangles = new int[]
                {
                    0, 2, 1,  // 첫 번째 삼각형 (bottom-left -> top-left -> bottom-right)
                    2, 3, 1   // 두 번째 삼각형 (top-left -> top-right -> bottom-right)
                };

                // UV 맵핑 (각 정점에 대한 텍스처 좌표)
                Vector2[] uv = new Vector2[]
                {
                    new Vector2(0, 0), // Bottom-left
                    new Vector2(1, 0), // Bottom-right
                    new Vector2(0, 1), // Top-left
                    new Vector2(1, 1)  // Top-right
                };

                // 정점, 삼각형, UV 할당
                _gridMeshShared.vertices = vertices;
                _gridMeshShared.triangles = triangles;
                _gridMeshShared.uv = uv;
            }
            if (_gridMaterialShared == null)
            {
                Shader shader = Shader.Find(PATH_SHADER);
                if (shader != null)
                {
                    _gridMaterialShared = new Material(shader);
                }
                else
                {
                    UnityEngine.Debug.LogError("쉐이더를 찾지 못했습니다.");
                    return;
                }
            }
            var instance = new GameObject("Grid Plane");
            _gridPlane = instance.transform;
            _gridPlane.SetParent(transform, false);
            _gridPlane.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

            var meshFilter = instance.AddComponent<MeshFilter>();
            meshFilter.mesh = _gridMeshShared;
            meshFilter.sharedMesh = _gridMeshShared;
            _gridMeshShared.RecalculateBounds();
            _gridMeshShared.RecalculateNormals();

            var renderer = instance.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial = _gridMaterialShared;
            renderer.material = _gridMaterialShared;
            _gridMaterial = renderer.material;
            if (_gridMaterial != null)
            {
                if (settings != null)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    block.SetFloat(HASH_LINE_ALPHA, 0.6f);
                    block.SetFloat(HASH_BG_ALPHA, 0.0f);
                    block.SetColor(HASH_LINE_COLOR, settings.lineColor);
                    block.SetFloat(HASH_LINE_THICKNESS, settings.lineThickness);
                    renderer.SetPropertyBlock(block);
                }
            }
        }
    }
}

