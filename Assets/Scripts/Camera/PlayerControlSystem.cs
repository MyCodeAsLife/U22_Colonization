using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(BuildingPlacer))]
public class PlayerControlSystem : MonoBehaviour
{
    private int _selectionMask;
    private int _groundMask;
    private int _uiMask;
    private CameraMover _cameraMover;
    private BuildingPlacer _buildingPlacer;
    private Plane _plane;
    //private bool _isPresedCtrl;                         // Состояние
    //private bool _isHoldLeftMouseButton;                // Состояние
    //private bool _isMouseMove;                          // Состояние
    //private bool _isWork;                               // Состояние
    private PlayerControlStates _states/* = new()*/;
    private Ray _ray;
    private RaycastHit _hit = new();
    private SelectableObject _hovered;
    private List<SelectableObject> _listOfSelected = new();
    private MainInputActions _inputActions;
    [SerializeField] private Image _frameImage;                                      // ++++++++++
    private Vector2 _cursoreStartPosition;
    private Vector2 _cursoreCurrentPosition;
    private Vector2 _startPos;
    private Vector2 _frameSize;
    private Coroutine _frameStretching;

    //public Ray Ray { get { return _ray; } }
    public MainInputActions InputActions { get { return _inputActions; } }

    private void Awake()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);
        _inputActions = new MainInputActions();
        //_buildingPlacer = transform.AddComponent<BuildingPlacer>();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Mouse.Delta.started += OnMouseMove;
        _inputActions.Mouse.LeftButtonClick.performed += OnLeftMousePress;
        _inputActions.Mouse.LeftButtonSlowTap.performed += OnLeftMouseSlowTap;
        _inputActions.Mouse.RightButtonClick.canceled += OnRightMouseClick;
        _inputActions.Keyboard.Ctrl.started += OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled += OnReleaseCtrl;
        _cameraMover = new CameraMover(this, _inputActions);
        //_buildingPlacer.SetControlSystem(this);
        //_buildingPlacer.SetInputActions(_inputActions);
    }

    private void OnDisable()
    {
        _inputActions.Keyboard.Ctrl.started -= OnPressCtrl;
        _inputActions.Keyboard.Ctrl.canceled -= OnReleaseCtrl;
        _inputActions.Mouse.RightButtonClick.canceled -= OnRightMouseClick;
        _inputActions.Mouse.LeftButtonSlowTap.performed += OnLeftMouseSlowTap;
        _inputActions.Mouse.LeftButtonClick.performed -= OnLeftMousePress;
        _inputActions.Mouse.Delta.started -= OnMouseMove;
        _inputActions.Disable();
    }

    private void Start()
    {
        _states = PlayerControlStates.None;
        _selectionMask = LayerMask.NameToLayer("Interactable");
        _groundMask = LayerMask.NameToLayer("Ground");
        _uiMask = LayerMask.NameToLayer("UI");
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

    //private void OnLeftMouseHold(InputAction.CallbackContext context) => _isHoldLeftMouseButton = true;
    //private void OnReleaseCtrl(InputAction.CallbackContext context) => _isPresedCtrl = false;
    //private void OnPressCtrl(InputAction.CallbackContext context) => _isPresedCtrl = true;

    public Vector3 GetRaycastPoint()                                           // Вынести в Control System
    {
        //_ray = _playerControlSystem.Ray;
        _plane.Raycast(_ray, out float distance);
        return _ray.GetPoint(distance);
    }

    private void OnReleaseCtrl(InputAction.CallbackContext context) => _states ^= PlayerControlStates.PresedCtrl;
    private void OnPressCtrl(InputAction.CallbackContext context) => _states |= PlayerControlStates.PresedCtrl;

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if ((_states & PlayerControlStates.HoldLeftMouseButton) == PlayerControlStates.HoldLeftMouseButton)
        {
            Vector2 endPos;
            _cursoreCurrentPosition = Input.mousePosition;
            _startPos = Vector2.Min(_cursoreStartPosition, _cursoreCurrentPosition);
            endPos = Vector2.Max(_cursoreStartPosition, _cursoreCurrentPosition);
            _frameImage.rectTransform.anchoredPosition = _startPos;
            _frameSize = endPos - _startPos;
            _frameImage.rectTransform.sizeDelta = _frameSize;

            if (_frameStretching == null)
                _frameStretching = StartCoroutine(ObjectsSelecting());
        }
    }

    private void OnLeftMouseSlowTap(InputAction.CallbackContext context)
    {
        //_isWork = false;
        _states ^= PlayerControlStates.HoldLeftMouseButton;
        //_states ^= PlayerControlStates.Frame;
        _frameImage.enabled = false;
    }

    private void OnLeftMousePress(InputAction.CallbackContext context)
    {
        if (_hit.collider != null && _hit.collider.gameObject.layer == _uiMask)     // Найти способ фиксировать нажатие по UI
            return;

        Debug.Log(_hit.collider.gameObject.layer);                                  //+++++++++++

        //if (_isPresedCtrl == false && _isHoldLeftMouseButton == false)
        if ((_states & PlayerControlStates.PresedCtrl) != PlayerControlStates.PresedCtrl &&
        (_states & PlayerControlStates.HoldLeftMouseButton) != PlayerControlStates.HoldLeftMouseButton)
        {
            UnselectAll();
            Select(_hovered);
        }
        else if (_listOfSelected.Contains(_hovered))                                        // Добавить проверку что если клацнули на UI, то это не срабатывает
        {
            Unselect(_hovered);
        }
        else
        {
            Select(_hovered);
        }

        _frameImage.rectTransform.sizeDelta = Vector2.zero;
        _frameImage.enabled = true;
        _cursoreStartPosition = Input.mousePosition;
        _states |= PlayerControlStates.HoldLeftMouseButton;
    }

    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (_hit.collider != null && _hit.collider.gameObject.layer == _groundMask)
            for (int i = 0; i < _listOfSelected.Count; i++)
                if (_listOfSelected[i].TryGetComponent<CollectorBotAI>(out CollectorBotAI bot))
                    bot.GoTo(_hit.point);
    }

    private void Select(SelectableObject obj)
    {
        if (obj != null && _listOfSelected.Contains(obj) == false)
        {
            _listOfSelected.Add(obj);
            obj.Selected();
        }
    }

    private void Unselect(SelectableObject obj)
    {
        if (obj != null)
        {
            obj.UnSelect();
            _listOfSelected.Remove(obj);
        }
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

    private IEnumerator ObjectsSelecting()               // Переименовать метод выделения
    {
        var delay = new WaitForEndOfFrame();
        //_isWork = true;
        _states |= PlayerControlStates.FrameStretching;

        //while (_isWork)
        while ((_states & PlayerControlStates.FrameStretching) == PlayerControlStates.FrameStretching &&
            (_states & PlayerControlStates.HoldLeftMouseButton) == PlayerControlStates.HoldLeftMouseButton)
        {
            yield return delay;
            Rect rect = new Rect(_startPos, _frameSize);                                                          // Вынести cоздание rect из цикла
            CollectorBotAI[] allBots = FindObjectsByType<CollectorBotAI>(FindObjectsSortMode.None);             // Вынести создание массива из цикла, тяжелая операция, нежелательно выполнение в цикле.

            for (int i = 0; i < allBots.Length; i++)
            {
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(allBots[i].transform.position);

                if (rect.Contains(screenPosition))
                {
                    //if (_isPresedCtrl == false)
                    if ((_states & PlayerControlStates.PresedCtrl) != PlayerControlStates.PresedCtrl)
                        Select(allBots[i]);
                    else
                        Unselect(allBots[i]);
                }
                //else if (_isPresedCtrl == false)
                else if ((_states & PlayerControlStates.PresedCtrl) != PlayerControlStates.PresedCtrl)
                {
                    Unselect(allBots[i]);
                }
            }
        }

        _states ^= PlayerControlStates.FrameStretching;
        _frameStretching = null;
    }
}
