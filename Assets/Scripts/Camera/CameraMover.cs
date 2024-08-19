using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover
{
    private PlayerControlSystem _playerControlSystem;
    //private MainInputActions _mainInputActions;
    private Vector3 _startPoint;
    //private Plane _plane;
    //private Ray _ray;

    public CameraMover(PlayerControlSystem playerControlSystem, MainInputActions inputActions)
    {
        //_plane = new Plane(Vector3.up, Vector3.zero);               // ������ plane ������������ �����? ��� ����������� ���������� �� ���� �� PlayerControlSystem (
        _playerControlSystem = playerControlSystem;
        //_mainInputActions = inputActions;

        _playerControlSystem.InputActions.Mouse.Scroll.started += OnMouseScroll;
        _playerControlSystem.InputActions.Mouse.MiddleButtonClick.performed += OnMouseMiddleButtonClick;
        _playerControlSystem.InputActions.Mouse.MiddleButtonSlowTap.performed += OnMouseMiddleButtonSlowTap;
    }

    ~CameraMover()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        _playerControlSystem.InputActions.Mouse.Scroll.started -= OnMouseScroll;
        _playerControlSystem.InputActions.Mouse.MiddleButtonClick.performed -= OnMouseMiddleButtonClick;
        _playerControlSystem.InputActions.Mouse.MiddleButtonSlowTap.performed -= OnMouseMiddleButtonSlowTap;
    }

    private void OnMouseScroll(InputAction.CallbackContext context)
    {
        const float ScrollSpeed = 3f;
        _playerControlSystem.transform.Translate(0f, 0f, _playerControlSystem.InputActions.Mouse.Scroll.ReadValue<Vector2>().y * ScrollSpeed);
    }

    private void OnMouseMiddleButtonClick(InputAction.CallbackContext context)
    {
        _startPoint = _playerControlSystem.GetRaycastPoint();
        _playerControlSystem.InputActions.Mouse.Delta.started += OnMouseMoveAndMouseMiddleButtonHold;
    }

    private void OnMouseMiddleButtonSlowTap(InputAction.CallbackContext context)
    {
        _playerControlSystem.InputActions.Mouse.Delta.started -= OnMouseMoveAndMouseMiddleButtonHold;
    }

    private void OnMouseMoveAndMouseMiddleButtonHold(InputAction.CallbackContext context)
    {
        Vector3 offset = _playerControlSystem.GetRaycastPoint() - _startPoint;
        _playerControlSystem.transform.position -= offset;
    }

    //private Vector3 GetRaycastPoint()                                           // ������� � Control System
    //{
    //    _ray = _playerControlSystem.Ray;
    //    _plane.Raycast(_ray, out float distance);
    //    return _ray.GetPoint(distance);
    //}

    //private void Start()
    //{
    //    _plane = new Plane(Vector3.up, Vector3.zero);               // ������ plane ������������ �����? ��� ����������� ���������� �� ���� �� PlayerControlSystem (
    //}

    //private void LateUpdate()                                       // �������� ��� �� PlayerControlSystem
    //{
    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    //float distance;
    //    //_plane.Raycast(ray, out distance);
    //    //Vector3 point = ray.GetPoint(distance);

    //    //if (Input.GetMouseButtonDown(2))                            // ������� Input Actions � ��������� ���������� � ����� ���� ������������ � �������� ������.
    //    //{
    //    //    _startPoint = point;
    //    //}

    //    //if (Input.GetMouseButton(2))                                // ���������� ��� � ������� ���������
    //    //{
    //    //    Vector3 offset = point - _startPoint;
    //    //    transform.position -= offset;
    //    //}

    //    //if (Input.mouseScrollDelta.y != 0)                          // �������� �������� ������ 1, �������� ����� -1
    //    //    transform.Translate(0f, 0f, Input.mouseScrollDelta.y * 3);
    //}
}
