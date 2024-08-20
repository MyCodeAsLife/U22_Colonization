using UnityEngine;

public class Building : SelectableObject
{
    private BuildingPlacer _buildingPlacer;
    private Vector3 _square;
    protected Vector3 SelfSize;

    public int Price { get; protected set; }
    public Vector3 Size { get { return SelfSize; } }

    protected override void Awake()
    {
        base.Awake();
        SelfSize.x = 3f;
        SelfSize.z = 3f;
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

    private void OnDrawGizmosSelected()
    {
        float startPositionX = SelfSize.x * _buildingPlacer.CellSize * 0.5f - 1;
        float startPositionZ = SelfSize.z * _buildingPlacer.CellSize * 0.5f - 1;
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
