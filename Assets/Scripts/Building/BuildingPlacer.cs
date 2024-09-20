using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControlSystem))]
public class BuildingPlacer : MonoBehaviour
{
    private SingleReactiveProperty<float> _cellSize = new();
    private PlayerControlSystem _controlSysytem;
    //[SerializeField] private BuildingUnderConstruction _planedBuilding;                                   // ++++++++++++
    private BuildingUnderConstruction _flyingBuilding;
    private Dictionary<Vector2Int, Building> _buildingsPositions = new();
    private MainBaseAI _selectedInteractiveObject;                              // ���� ����� ������ ���� �������� � ������� ����� ������, �� ����� ����� �������������
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

    public void CreateFlyingBuilding(BuildingUnderConstruction prefab)
    {
        var parent = GameObject.FindGameObjectWithTag("ResourceSpawner").transform.parent;      //Magic string
        _flyingBuilding = Instantiate(prefab, parent);                                          // � �������� �������� ������ MainCanvas
        //MoveBuilding();
        _controlSysytem.InputActions.Mouse.Delta.started += OnMouseMove;
        _controlSysytem.InputActions.Mouse.LeftButtonClick.performed += OnMouseLeftButtonClick;
    }

    private void OnMouseLeftButtonClick(InputAction.CallbackContext context)
    {

        if (_flyingBuilding != null && _canBePlaced)
        {
            // ��������� ���� �� ��� ���������� ������������� ����� ����������� ����� ��� ������� ����� �����
            //if (_planedBuilding == null)
            //{
            //    _planedBuilding = _flyingBuilding;
            //    _flyingBuilding = null;
            //    RecordLocationBuilding(new Vector2Int(_roundPosX, _roundPosZ), _planedBuilding);                // ���������� ���������� ��� ������ �������������
            //    _controlSysytem.InputActions.Mouse.Delta.started -= OnMouseMove;
            //    _controlSysytem.InputActions.Mouse.LeftButtonClick.performed -= OnMouseLeftButtonClick;
            //    //if (_selectedInteractiveObject != null)                                                               // ����� ����� ���� null ????
            //        //_selectedInteractiveObject.AddNewTask(new Task(0, TypesOfTasks.Constructing, _planedBuilding));        // ������ �� ������ ������������� ��������� � MainBase
            //    float newPosY = _planedBuilding.transform.position.y - _planedBuilding.transform.localScale.y * 2;        // Magic � ������������
            //    _planedBuilding.SetEndPosition(_planedBuilding.transform.position);
            //    _planedBuilding.transform.position = new Vector3(_planedBuilding.transform.position.x, newPosY, _planedBuilding.transform.position.z);
            //    _planedBuilding.SetStartPosition(_planedBuilding.transform.position);
            //    _selectedInteractiveObject.ScheduleConstruction(_planedBuilding);
            //}
            //else if (_planedBuilding.IsBuildingInProgress)         // ���� ������ ��� �������� �� ������ �� ������
            //{
            //    Destroy(_flyingBuilding.gameObject);
            //    _flyingBuilding = null;
            //}
            //else  // ����������� ����� �������������
            //{
            //    float newPosY = _planedBuilding.transform.position.y - _planedBuilding.transform.localScale.y * 2;        // Magic � ������������
            //    _planedBuilding.SetEndPosition(_planedBuilding.transform.position);
            //    _planedBuilding.transform.position = new Vector3(_planedBuilding.transform.position.x, newPosY, _planedBuilding.transform.position.z);
            //    _planedBuilding.SetStartPosition(_planedBuilding.transform.position);
            //}

            _controlSysytem.InputActions.Mouse.Delta.started -= OnMouseMove;
            _controlSysytem.InputActions.Mouse.LeftButtonClick.performed -= OnMouseLeftButtonClick;

            //if (_selectedInteractiveObject != null)                                                               // ����� ����� ���� null ????
            //_selectedInteractiveObject.AddNewTask(new Task(0, TypesOfTasks.Constructing, _planedBuilding));        // ������ �� ������ ������������� ��������� � MainBase

            float newPosY = _flyingBuilding.transform.position.y - _flyingBuilding.transform.localScale.y * 2;        // Magic
            _flyingBuilding.SetEndPosition(_flyingBuilding.transform.position);
            _flyingBuilding.transform.position = new Vector3(_flyingBuilding.transform.position.x, newPosY, _flyingBuilding.transform.position.z);
            _flyingBuilding.SetStartPosition(_flyingBuilding.transform.position);
            _flyingBuilding.SetRoundPosition(new Vector2Int(_roundPosX, _roundPosZ));
            _flyingBuilding.BuildingStarted += OnStartBuilding;
            _selectedInteractiveObject.ScheduleConstruction(_flyingBuilding);
            _flyingBuilding = null;
        }
    }

    private void OnStartBuilding(BuildingUnderConstruction building)
    {
        RecordLocationBuilding(new Vector2Int(building.RoundPosition.x, building.RoundPosition.y), building);
        building.BuildingStarted -= OnStartBuilding;
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
