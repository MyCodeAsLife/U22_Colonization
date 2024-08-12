using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Management : MonoBehaviour
{
    private int _selectionMask;
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
        _inputActions.Mouse.LeftButtonClick.performed += OnLeftMouseClick;
    }

    private void OnDisable()
    {
        _inputActions.Mouse.LeftButtonClick.performed -= OnLeftMouseClick;
        _inputActions.Disable();
    }

    private void Start()
    {
        _selectionMask = LayerMask.NameToLayer("Interactable") /*& LayerMask.NameToLayer("Resource")*/;
        //_hit = new RaycastHit();
    }

    private void OnLeftMouseClick(InputAction.CallbackContext context)
    {
        if (_hovered != null && _listOfSelected.Contains(_hovered) == false)
        {
            _listOfSelected.Add(_hovered);
            _hovered.Select();
        }
        else if (_hovered == null)
        {
            foreach (var item in _listOfSelected.ToArray())
            {
                item.UnSelect();
                _listOfSelected.Remove(item);
            }
        }
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

    private void UnhoverCurrentObject()
    {
        _hovered?.OnUnhover();
        _hovered = null;
    }
}
