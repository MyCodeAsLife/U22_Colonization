using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover
{
    private PlayerControlSystem _playerControlSystem;
    private MainInputActions _mainInputActions;
    private Vector3 _startPoint;
    private Plane _plane;
    private Ray _ray;

    public CameraMover(PlayerControlSystem playerControlSystem, MainInputActions inputActions)
    {
        _plane = new Plane(Vector3.up, Vector3.zero);               // Вместо plane использовать Карту? или расчитывать расстояние по лучу из PlayerControlSystem (
        _playerControlSystem = playerControlSystem;
        _mainInputActions = inputActions;

        _mainInputActions.Mouse.Scroll.started += OnMouseScroll;
        _mainInputActions.Mouse.MiddleButtonClick.performed += OnMouseMiddleButtonClick;
        _mainInputActions.Mouse.MiddleButtonSlowTap.performed += OnMouseMiddleButtonSlowTap;
    }

    ~CameraMover()
    {
        UnsubscribeAll();
    }

    private void UnsubscribeAll()
    {
        _mainInputActions.Mouse.Scroll.started -= OnMouseScroll;
        _mainInputActions.Mouse.MiddleButtonClick.performed -= OnMouseMiddleButtonClick;
        _mainInputActions.Mouse.MiddleButtonSlowTap.performed -= OnMouseMiddleButtonSlowTap;
    }

    private void OnMouseScroll(InputAction.CallbackContext context)
    {
        const float ScrollSpeed = 3f;
        _playerControlSystem.transform.Translate(0f, 0f, _mainInputActions.Mouse.Scroll.ReadValue<Vector2>().y * ScrollSpeed);
    }

    private void OnMouseMiddleButtonClick(InputAction.CallbackContext context)
    {
        _startPoint = GetRaycastPoint();
        _mainInputActions.Mouse.Delta.started += OnMouseMoveAndMouseMiddleButtonHold;
    }

    private void OnMouseMiddleButtonSlowTap(InputAction.CallbackContext context)
    {
        _mainInputActions.Mouse.Delta.started -= OnMouseMoveAndMouseMiddleButtonHold;
    }

    private void OnMouseMoveAndMouseMiddleButtonHold(InputAction.CallbackContext context)
    {
        Vector3 offset = GetRaycastPoint() - _startPoint;
        _playerControlSystem.transform.position -= offset;
    }

    private Vector3 GetRaycastPoint()
    {
        _ray = _playerControlSystem.Ray;
        _plane.Raycast(_ray, out float distance);
        return _ray.GetPoint(distance);
    }

    //private void Start()
    //{
    //    _plane = new Plane(Vector3.up, Vector3.zero);               // Вместо plane использовать Карту? или расчитывать расстояние по лучу из PlayerControlSystem (
    //}

    //private void LateUpdate()                                       // Забирать луч из PlayerControlSystem
    //{
    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    //float distance;
    //    //_plane.Raycast(ray, out distance);
    //    //Vector3 point = ray.GetPoint(distance);

    //    //if (Input.GetMouseButtonDown(2))                            // Вынести Input Actions в отдельный контроллер и через него привязыватся к нажатиям клавиш.
    //    //{
    //    //    _startPoint = point;
    //    //}

    //    //if (Input.GetMouseButton(2))                                // Перемещять мыш с зажатым колесиком
    //    //{
    //    //    Vector3 offset = point - _startPoint;
    //    //    transform.position -= offset;
    //    //}

    //    //if (Input.mouseScrollDelta.y != 0)                          // Колесико крутится вперед 1, крутится назад -1
    //    //    transform.Translate(0f, 0f, Input.mouseScrollDelta.y * 3);
    //}
}
