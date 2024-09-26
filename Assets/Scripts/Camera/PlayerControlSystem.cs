using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private PlayerControlStates _states = new();
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

    public MainInputActions InputActions { get { return _inputActions; } }

    private void Awake()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);
        _inputActions = new MainInputActions();
        _buildingPlacer = GetComponent<BuildingPlacer>();
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
        _selectionMask = LayerMask.NameToLayer("Interactable");
        _groundMask = LayerMask.NameToLayer("Ground");
        _uiMask = LayerMask.NameToLayer("UI");
    }

    private void LateUpdate()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);                   // Необходимо каждый раз получать новый луч
        _states.CursoreOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, 100/*, _selectionMask*/ /*^ int.MaxValue*/))    // Отказывается адекватно воспринимать маску
        {
            if (_hit.collider.gameObject.layer == _selectionMask)
            {
                if (_hit.collider.TryGetComponent<SelectableObject>(out SelectableObject obj) == false)
                    throw new Exception("This object does not inherit from SelectableObject");

                if (_hovered != obj)
                {
                    if (_hovered != null)
                    {
                        _hovered.OnUnhover();
                        _hovered.Destroyed -= OnDestroyObject;
                    }

                    _hovered = obj;
                    _hovered.Destroyed += OnDestroyObject;
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

    public Vector3 GetRaycastPoint()
    {
        _plane.Raycast(_ray, out float distance);
        return _ray.GetPoint(distance);
    }

    private void OnReleaseCtrl(InputAction.CallbackContext context) => _states.PresedCtrl = false;
    private void OnPressCtrl(InputAction.CallbackContext context) => _states.PresedCtrl = true;

    private void OnDestroyObject()
    {
        Unselect(_hovered);
        _listOfSelected.Remove(_hovered);
        _hovered = null;
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (_states.HoldLeftMouseButton)
        {
            _cursoreCurrentPosition = Input.mousePosition;
            _startPos = Vector2.Min(_cursoreStartPosition, _cursoreCurrentPosition);
            Vector2 endPos = Vector2.Max(_cursoreStartPosition, _cursoreCurrentPosition);
            _frameImage.rectTransform.anchoredPosition = _startPos;
            _frameSize = endPos - _startPos;
            _frameImage.rectTransform.sizeDelta = _frameSize;

            if (_frameStretching == null && (_frameSize.x > 10 && _frameSize.y > 10))                             // Magic - погрешность в 10 едениц
                _frameStretching = StartCoroutine(ObjectsSelecting());
        }
    }

    private void OnLeftMouseSlowTap(InputAction.CallbackContext context)
    {
        _states.HoldLeftMouseButton = false;
        _frameImage.enabled = false;
    }

    private void OnLeftMousePress(InputAction.CallbackContext context)
    {
        if (_states.CursoreOverUI)                                  // Если нажали по UI то ничего не делать
            return;

        if (_states.PresedCtrl == false && _states.HoldLeftMouseButton == false)
        {
            UnselectAll();
            Select(_hovered);
        }
        else if (_listOfSelected.Contains(_hovered))
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
        _states.HoldLeftMouseButton = true;
    }

    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (_hit.collider != null && _hit.collider.gameObject.layer == _groundMask)
            for (int i = 0; i < _listOfSelected.Count; i++)
                if (_listOfSelected[i].TryGetComponent<CollectorBotAI>(out CollectorBotAI bot))
                    bot.GoTo(_hit.point);                           // Сделать вызов движения по клику мышки через задачу ???
    }

    private void Select(SelectableObject obj)
    {
        if (obj != null && _listOfSelected.Contains(obj) == false)
        {
            _listOfSelected.Add(obj);
            obj.Selected();

            if (obj is MainBaseAI)
                _buildingPlacer.SelectInteractivObject(obj as MainBaseAI);
        }
    }

    private void Unselect(SelectableObject obj)
    {
        if (obj != null)
        {
            obj.UnSelect();
            _listOfSelected.Remove(obj);

            if (obj is MainBaseAI)
                _buildingPlacer.UnSelectInteractivObject();
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
        _states.FrameStretching = true;

        while (_states.FrameStretching && _states.HoldLeftMouseButton)
        {
            yield return delay;
            Rect rect = new Rect(_startPos, _frameSize);                                                          // Вынести cоздание rect из цикла
            CollectorBotAI[] allBots = FindObjectsByType<CollectorBotAI>(FindObjectsSortMode.None);             // Вынести создание массива из цикла, тяжелая операция, нежелательно выполнение в цикле.

            for (int i = 0; i < allBots.Length; i++)
            {
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(allBots[i].transform.position);

                if (rect.Contains(screenPosition))
                {
                    if (_states.PresedCtrl == false)
                        Select(allBots[i]);
                    else
                        Unselect(allBots[i]);
                }
                else if (_states.PresedCtrl == false)
                {
                    Unselect(allBots[i]);
                }
            }
        }

        _states.FrameStretching = false;
        _frameStretching = null;
    }
}
