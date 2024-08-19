using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControlSystem))]
public class BuildingPlacer : MonoBehaviour
{
    private SingleReactiveProperty<float> _cellSize = new();
    private PlayerControlSystem _controlSysytem;
    [SerializeField] private Building _flyingBuilding;                                   // ++++++++++++

    public float CellSize { get { return _cellSize.Value; } }

    private void Awake()
    {
        _cellSize.Value = 1f;
    }

    private void Start()
    {
        _controlSysytem = GetComponent<PlayerControlSystem>();
        //_controlSysytem.InputActions.Mouse.LeftButtonClick.performed += OnMouseLeftButtonClick;
        //_controlSysytem.InputActions.Mouse.Delta.started += OnMouseMove;                                                    //+++++++++++++
    }

    public void SubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change += fun;
    }

    public void UnSubscribeOnCellSizeChanged(Action<float> fun)
    {
        _cellSize.Change -= fun;
    }

    private void OnMouseLeftButtonClick(InputAction.CallbackContext context)                // Доработать, есть логические ошибки
    {
        //if (_flyingBuilding == null)
        //{
        //    _controlSysytem.InputActions.Mouse.Delta.started += OnMouseMove;
        //}
        //else
        //{
        //    _flyingBuilding = null;
        //    _controlSysytem.InputActions.Mouse.Delta.started -= OnMouseMove;
        //}

        if (_flyingBuilding != null)
        {
            _flyingBuilding = null;
            _controlSysytem.InputActions.Mouse.Delta.started -= OnMouseMove;
        }
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector3 point = _controlSysytem.GetRaycastPoint() / _cellSize.Value;

        int x = Mathf.RoundToInt(point.x);
        int z = Mathf.RoundToInt(point.z);

        _flyingBuilding.transform.position = new Vector3(x, 0, z) * _cellSize.Value;
    }
}
