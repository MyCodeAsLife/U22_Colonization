using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover
{
    private PlayerControlSystem _playerControlSystem;
    private Vector3 _startPoint;

    public CameraMover(PlayerControlSystem playerControlSystem, MainInputActions inputActions)
    {
        _playerControlSystem = playerControlSystem;
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
}
