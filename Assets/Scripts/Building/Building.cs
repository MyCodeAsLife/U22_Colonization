using UnityEngine;

public class Building : ChangingObject
{
    protected Vector3 SelfSize;

    private Vector3 _square;
    private Color[] _startColors;
    private Renderer[] _renderers;
    private BuildingPlacer _buildingPlacer;

    public Vector3 Size { get { return SelfSize; } }

    protected override void Awake()
    {
        base.Awake();
        SelfSize.x = 6f;
        SelfSize.z = 6f;

        _renderers = GetComponentsInChildren<Renderer>();
        _startColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
            _startColors[i] = _renderers[i].material.color;
    }

    protected virtual void OnDisable()
    {
        _buildingPlacer?.UnSubscribeOnCellSizeChanged(OnCellSizeChange);
    }

    protected virtual void Start()
    {
        _buildingPlacer = Object.FindFirstObjectByType<BuildingPlacer>();
        _buildingPlacer.SubscribeOnCellSizeChanged(OnCellSizeChange);
        OnCellSizeChange(_buildingPlacer.CellSize);
    }

    public void DisplayInvalidPosition()
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color += Color.red;
    }

    public void DisplayValidPosition()
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _startColors[i];
    }

    private void OnDrawGizmosSelected()
    {
        float startPositionX = SelfSize.x * _buildingPlacer.CellSize * 0.5f - 0.5f;
        float startPositionZ = SelfSize.z * _buildingPlacer.CellSize * 0.5f - 0.5f;
        Vector3 pos = transform.position - new Vector3(startPositionX, 0, startPositionZ);

        for (int x = 0; x < SelfSize.x; x++)
        {
            float sizeX = x * _buildingPlacer.CellSize;

            for (int z = 0; z < SelfSize.z; z++)
            {
                float sizeZ = z * _buildingPlacer.CellSize;
                Gizmos.DrawWireCube(pos + new Vector3(sizeX, 0, sizeZ), _square);
            }
        }
    }

    private void OnCellSizeChange(float cellSize)
    {
        _square = new Vector3(cellSize, 0, cellSize);
    }
}
