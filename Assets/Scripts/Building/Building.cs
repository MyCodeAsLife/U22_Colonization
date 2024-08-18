using UnityEngine;

public class Building : SelectableObject
{
    private BuildingPlacer _buildingPlacer;
    private Vector3 _square;
    private Vector3 _size;

    public int Price { get; private set; }
    public Vector3 Size { get { return _size; } }

    protected override void Awake()
    {
        base.Awake();
        _size.x = 3f;
        _size.z = 3f;
        _buildingPlacer = Object.FindFirstObjectByType<BuildingPlacer>();
        OnCellSizeChange(_buildingPlacer.CellSize);
    }

    protected virtual void OnEnable()
    {
        _buildingPlacer.SubscribeOnCellSizeChanged(OnCellSizeChange);
    }

    protected virtual void OnDisable()
    {
        _buildingPlacer.UnSubscribeOnCellSizeChanged(OnCellSizeChange);
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < _size.x; x++)
        {
            float sizeX = x * _buildingPlacer.CellSize;

            for (int z = 0; z < _size.z; z++)
            {
                float sizeZ = z * _buildingPlacer.CellSize;
                Gizmos.DrawWireCube(transform.position + new Vector3(sizeX, 0, sizeZ), _square);
            }
        }
    }

    private void OnCellSizeChange(float cellSize)
    {
        _square = new Vector3(cellSize, 0, cellSize);
    }
}
