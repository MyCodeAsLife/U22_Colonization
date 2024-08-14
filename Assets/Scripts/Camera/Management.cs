using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Management : MonoBehaviour
{
    private int _selectionMask;
    private int _groundMask;
    private bool _isPresedCtrl;
    private bool _isHoldLeftMouseButton;
    private Ray _ray;
    private RaycastHit _hit = new();
    private SelectableObject _hovered;
    private List<SelectableObject> _listOfSelected = new();
    private MainInputActions _inputActions;
    [SerializeField] private Image _frameImage;                                      // ++++++++++
    private Vector2 _cursoreStartPosition;
    private Vector2 _cursoreCurrentPosition;

    private void Awake()
    {
        _inputActions = new MainInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Mouse.LeftButtonClick.canceled += OnLeftMouseClick;
        _inputActions.Mouse.LeftButtonHold.performed += OnFrameSelected;                // Нужно????
        _inputActions.Mouse.RightButtonClick.canceled += OnRightMouseClick;
        _inputActions.Keyboard.Ctrl.started += OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled += OnReleaseCtrl;
        _inputActions.Shortcut.CtrlLeftMouse.performed += OnShortcutCtrlLeftMouse;                // Нужно????
    }

    private void OnDisable()
    {
        _inputActions.Shortcut.CtrlLeftMouse.performed -= OnShortcutCtrlLeftMouse;                // Нужно????
        _inputActions.Keyboard.Ctrl.started -= OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled -= OnReleaseCtrl;
        _inputActions.Mouse.RightButtonClick.canceled -= OnRightMouseClick;
        _inputActions.Mouse.LeftButtonHold.performed -= OnFrameSelected;                // Нужно????
        _inputActions.Mouse.LeftButtonClick.canceled -= OnLeftMouseClick;
        _inputActions.Disable();
    }

    private void Start()
    {
        _isHoldLeftMouseButton = false;
        _isPresedCtrl = false;
        _selectionMask = LayerMask.NameToLayer("Interactable");
        _groundMask = LayerMask.NameToLayer("Ground");
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

        // Выделение рамкой

    }

    private void OnFrameSelected(InputAction.CallbackContext context)           // Через корутину
    {
        Debug.Log("Selecting");

        if (_isHoldLeftMouseButton == false)
            StartCoroutine(FrameStretching());
        else
            _isHoldLeftMouseButton = false;
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
    }

    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (_hit.collider.gameObject.layer == _groundMask)
            for (int i = 0; i < _listOfSelected.Count; i++)
                if (_listOfSelected[i].TryGetComponent<CollectorBotAI>(out CollectorBotAI bot))
                    bot.GoTo(_hit.point);
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

    private IEnumerator FrameStretching()
    {
        var delay = new WaitForEndOfFrame();
        Vector2 frameSize;
        Vector2 startPos;
        Vector2 endPos;
        _frameImage.rectTransform.sizeDelta = Vector2.zero;
        _isHoldLeftMouseButton = true;
        _cursoreStartPosition = Input.mousePosition;
        _frameImage.enabled = true;

        while (_isHoldLeftMouseButton)
        {
            yield return delay;
            _cursoreCurrentPosition = Input.mousePosition;
            startPos = Vector2.Min(_cursoreStartPosition, _cursoreCurrentPosition);
            endPos = Vector2.Max(_cursoreStartPosition, _cursoreCurrentPosition);
            _frameImage.rectTransform.anchoredPosition = startPos;
            frameSize = endPos - startPos;
            _frameImage.rectTransform.sizeDelta = frameSize;
        }

        _frameImage.enabled = false;
        // По окончанию, вызвать метод выделения всех кто попал в поле рамки
    }
}
