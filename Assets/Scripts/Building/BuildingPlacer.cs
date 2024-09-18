using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControlSystem))]
public class BuildingPlacer : MonoBehaviour
{
    private SingleReactiveProperty<float> _cellSize = new();
    private PlayerControlSystem _controlSysytem;
    [SerializeField] private Building _flyingBuilding;                                   // ++++++++++++
    private Dictionary<Vector2Int, Building> _buildingsPositions = new();
    private MainBaseAI _selectedInteractiveObject;     // ���� ����� ������ ���� �������� � ������� ����� ������, �� ����� ����� �������������
    private int _roundPosX;
    private int _roundPosZ;
    private bool _canBePlaced;

    public float CellSize { get { return _cellSize.Value; } }

    private void Awake()
    {
        _cellSize.Value = 1f;
        _canBePlaced = true;
    }

    private void Start()
    {
        _controlSysytem = GetComponent<PlayerControlSystem>();
    }

    public void SubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change += fun;
    }

    public void UnSubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change -= fun;
    }

    public void SelectInteractivObject(MainBaseAI interactiveObject)
    {
        _selectedInteractiveObject = interactiveObject;
    }

    public void UnSelectInteractivObject()
    {
        _selectedInteractiveObject = null;
    }

    public void CreateBuilding(Building prefab)
    {
        var parent = GameObject.FindGameObjectWithTag("ResourceSpawner").transform.parent;      //Magic string
        _flyingBuilding = Instantiate(prefab, parent);                                      // � �������� �������� ������ MainCanvas
        MoveBuilding();
        _controlSysytem.InputActions.Mouse.Delta.started += OnMouseMove;
        _controlSysytem.InputActions.Mouse.LeftButtonClick.performed += OnMouseLeftButtonClick;
    }

    private void OnMouseLeftButtonClick(InputAction.CallbackContext context)
    {
        if (_flyingBuilding != null && _canBePlaced)
        {
            RecordLocationBuilding(new Vector2Int(_roundPosX, _roundPosZ), _flyingBuilding);
            _controlSysytem.InputActions.Mouse.Delta.started -= OnMouseMove;
            _controlSysytem.InputActions.Mouse.LeftButtonClick.performed -= OnMouseLeftButtonClick;

            if (_selectedInteractiveObject != null)
                _selectedInteractiveObject.AddNewTask(new Task(0, TypesOfTasks.Constructing, _flyingBuilding));        // Magic

            float newPosY = _flyingBuilding.transform.position.y - _flyingBuilding.transform.localScale.y;
            _flyingBuilding.transform.position = new Vector3(_flyingBuilding.transform.position.x, newPosY, _flyingBuilding.transform.position.z);
            _flyingBuilding = null;
        }
    }

    private void MoveBuilding()
    {
        Vector3 point = _controlSysytem.GetRaycastPoint() / _cellSize.Value;
        _roundPosX = Mathf.RoundToInt(point.x);
        _roundPosZ = Mathf.RoundToInt(point.z);
        _flyingBuilding.transform.position = new Vector3(_roundPosX, 0, _roundPosZ) * _cellSize.Value;
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        MoveBuilding();
        bool chaeckAllowPosition = CheckAllowPosition(new Vector2Int(_roundPosX, _roundPosZ), _flyingBuilding);

        if (_canBePlaced != chaeckAllowPosition)
        {
            _canBePlaced = chaeckAllowPosition;

            if (_canBePlaced)
            {
                _flyingBuilding.DisplayValidPosition();
            }
            else
            {
                _flyingBuilding.DisplayInvalidPosition();
            }
        }
    }

    private void RecordLocationBuilding(Vector2Int position, Building building)     // ����� �� ���������� ������ � �������
    {
        int posX = position.x - (int)(building.Size.x * 0.5f);                          // ������������
        int posZ = position.y - (int)(building.Size.z * 0.5f);                          // ������������

        for (int x = 0; x < building.Size.x; x++)
        {
            for (int z = 0; z < building.Size.z; z++)
            {
                Vector2Int cellPosition = new Vector2Int(posX + x, posZ + z);                          // ������������?
                _buildingsPositions.Add(cellPosition, _flyingBuilding);
            }
        }
    }

    private bool CheckAllowPosition(Vector2Int position, Building building)     // ����� �� ���������� ������ � �������
    {
        int posX = position.x - (int)(building.Size.x * 0.5f);                          // ������������
        int posZ = position.y - (int)(building.Size.z * 0.5f);                          // ������������

        for (int x = 0; x < building.Size.x; x++)
        {
            for (int z = 0; z < building.Size.z; z++)
            {
                Vector2Int cellPosition = new Vector2Int(posX + x, posZ + z);                          // ������������?

                if (_buildingsPositions.ContainsKey(cellPosition))
                    return false;
            }
        }

        return true;
    }
}
