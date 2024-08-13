using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Management : MonoBehaviour
{
    private int _selectionMask;
    private bool _isPresedCtrl;
    private Ray _ray;
    private RaycastHit _hit = new();
    private SelectableObject _hovered;
    private List<SelectableObject> _listOfSelected = new();
    private MainInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new MainInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Mouse.LeftButtonClick.canceled += OnLeftMouseClick;
        _inputActions.Mouse.RightButtonClick.canceled += OnRightMouseClick;
        _inputActions.Keyboard.Ctrl.started += OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled += OnReleaseCtrl;
        _inputActions.Shortcut.CtrlLeftMouse.performed += OnShortcutCtrlLeftMouse;
    }

    private void OnDisable()
    {
        _inputActions.Shortcut.CtrlLeftMouse.performed -= OnShortcutCtrlLeftMouse;
        _inputActions.Keyboard.Ctrl.started -= OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled -= OnReleaseCtrl;
        _inputActions.Mouse.RightButtonClick.canceled -= OnRightMouseClick;
        _inputActions.Mouse.LeftButtonClick.canceled -= OnLeftMouseClick;
        _inputActions.Disable();
    }

    private void Start()
    {
        _isPresedCtrl = false;
        _selectionMask = LayerMask.NameToLayer("Interactable") /*& LayerMask.NameToLayer("Resource")*/;
    }

    private void LateUpdate()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);                   // Необходимо каждый раз получать новый луч

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, 100/*, _selectionMask*/ /*^ int.MaxValue*/))    // Отказывается адекватно воспринимать маску
        {
            if (_hit.collider.gameObject.layer == _selectionMask)
            {
                if (_hit.collider.TryGetComponent<SelectableObject>(out SelectableObject obj) == false)
                    throw new Exception("This object does not inherit from SelectableObject");

                if (_hovered != obj)
                {
                    _hovered?.OnUnhover();
                    _hovered = obj;
                    _hovered.OnHover();
                }
            }
            else
            {
                UnhoverCurrentObject();
            }
        }
        else
        {
            UnhoverCurrentObject();
        }
    }

    private void OnReleaseCtrl(InputAction.CallbackContext context)
    {
        _isPresedCtrl = false;
    }

    private void OnPressCtrl(InputAction.CallbackContext context)
    {
        _isPresedCtrl = true;
    }

    private void OnShortcutCtrlLeftMouse(InputAction.CallbackContext context)
    {
        Select();
    }

    private void OnLeftMouseClick(InputAction.CallbackContext context)
    {
        if (_isPresedCtrl == false)
        {
            UnselectAll();
            Select();
        }
        //else
        //{
        //    UnselectAll();
        //}
    }

    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        //UnselectAll();
        for (int i = 0; i < _listOfSelected.Count; i++)
        {
            //if(_listOfSelected[i].TryGetComponent<CollectorBotAI>(out CollectorBotAI bot))
            //    bot.GoTo()                                                                          // Остановился здесь
        }
    }

    private void Select()
    {
        if (_hovered != null && _listOfSelected.Contains(_hovered) == false)
        {
            _listOfSelected.Add(_hovered);
            _hovered.Selected();
        }
        else if (_hovered != null && _listOfSelected.Contains(_hovered) && _isPresedCtrl)
        {
            Unselect();
        }
    }

    private void Unselect()
    {
        _hovered.UnSelect();
        _listOfSelected.Remove(_hovered);
    }

    private void UnselectAll()
    {
        foreach (var item in _listOfSelected.ToArray())
        {
            item.UnSelect();
            _listOfSelected.Remove(item);
        }
    }

    private void UnhoverCurrentObject()
    {
        _hovered?.OnUnhover();
        _hovered = null;
    }
}
