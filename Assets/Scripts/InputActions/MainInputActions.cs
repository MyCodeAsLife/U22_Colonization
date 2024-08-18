//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/InputActions/MainInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @MainInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainInputActions"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""5b6fc92f-e0c7-4232-bffe-690df94849dc"",
            ""actions"": [
                {
                    ""name"": ""LeftButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""1c1f642e-9a6f-4479-8c52-6851f920ff51"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""4478ad80-2a27-49dd-940e-2eeeb56d07cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftButtonSlowTap"",
                    ""type"": ""Button"",
                    ""id"": ""cf86be7f-be03-4afb-99f4-e20da6e67e38"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Delta"",
                    ""type"": ""Value"",
                    ""id"": ""7899356e-1ec5-44cb-b074-fd9a2954a3bd"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MiddleButtonClick"",
                    ""type"": ""Button"",
                    ""id"": ""7e2e1cc9-086a-4978-8851-2a737a179392"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""7b51861f-50e8-4c6c-8ab1-c07c4a4c3435"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MiddleButtonSlowTap"",
                    ""type"": ""Button"",
                    ""id"": ""171a691a-fd48-448e-acd7-b5c8646d1b62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hold"",
                    ""type"": ""Button"",
                    ""id"": ""63baf8bd-7e46-41e7-963f-67b8d8e424cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7bb67b8a-a472-41c9-b495-56c13b8832bf"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""LeftButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7667a6a2-3620-428f-81ad-89183ec99ad4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RightButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fe2cf45-5ba7-40e4-a7fc-19428b5e49f1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""SlowTap(duration=0.01,pressPoint=0.01)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""LeftButtonSlowTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""776a46c4-e74e-43d1-80d9-acffbc4f1261"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Delta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a001699-e354-44ac-a5f5-60f2de24c697"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MiddleButtonClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b757a09f-a880-48f5-9c70-52518baa34a2"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""943a798f-e67a-47e8-b6d3-a74b5e3d1eaa"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": ""SlowTap(duration=0.01,pressPoint=0.01)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MiddleButtonSlowTap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7118640b-96b9-4538-bfbf-fb228f7f1dfa"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": ""Hold(duration=0.1,pressPoint=0.1)"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""id"": ""1edf01dc-ecd8-42dd-8223-d9c5a3ace60d"",
            ""actions"": [
                {
                    ""name"": ""Ctrl"",
                    ""type"": ""Button"",
                    ""id"": ""859a030a-339a-4271-9b2f-0cd7ecf5b3a5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""43bba0b1-8eb2-4c63-8ed1-7608f72cc441"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_LeftButtonClick = m_Mouse.FindAction("LeftButtonClick", throwIfNotFound: true);
        m_Mouse_RightButtonClick = m_Mouse.FindAction("RightButtonClick", throwIfNotFound: true);
        m_Mouse_LeftButtonSlowTap = m_Mouse.FindAction("LeftButtonSlowTap", throwIfNotFound: true);
        m_Mouse_Delta = m_Mouse.FindAction("Delta", throwIfNotFound: true);
        m_Mouse_MiddleButtonClick = m_Mouse.FindAction("MiddleButtonClick", throwIfNotFound: true);
        m_Mouse_Scroll = m_Mouse.FindAction("Scroll", throwIfNotFound: true);
        m_Mouse_MiddleButtonSlowTap = m_Mouse.FindAction("MiddleButtonSlowTap", throwIfNotFound: true);
        m_Mouse_Hold = m_Mouse.FindAction("Hold", throwIfNotFound: true);
        // Keyboard
        m_Keyboard = asset.FindActionMap("Keyboard", throwIfNotFound: true);
        m_Keyboard_Ctrl = m_Keyboard.FindAction("Ctrl", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Mouse
    private readonly InputActionMap m_Mouse;
    private List<IMouseActions> m_MouseActionsCallbackInterfaces = new List<IMouseActions>();
    private readonly InputAction m_Mouse_LeftButtonClick;
    private readonly InputAction m_Mouse_RightButtonClick;
    private readonly InputAction m_Mouse_LeftButtonSlowTap;
    private readonly InputAction m_Mouse_Delta;
    private readonly InputAction m_Mouse_MiddleButtonClick;
    private readonly InputAction m_Mouse_Scroll;
    private readonly InputAction m_Mouse_MiddleButtonSlowTap;
    private readonly InputAction m_Mouse_Hold;
    public struct MouseActions
    {
        private @MainInputActions m_Wrapper;
        public MouseActions(@MainInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftButtonClick => m_Wrapper.m_Mouse_LeftButtonClick;
        public InputAction @RightButtonClick => m_Wrapper.m_Mouse_RightButtonClick;
        public InputAction @LeftButtonSlowTap => m_Wrapper.m_Mouse_LeftButtonSlowTap;
        public InputAction @Delta => m_Wrapper.m_Mouse_Delta;
        public InputAction @MiddleButtonClick => m_Wrapper.m_Mouse_MiddleButtonClick;
        public InputAction @Scroll => m_Wrapper.m_Mouse_Scroll;
        public InputAction @MiddleButtonSlowTap => m_Wrapper.m_Mouse_MiddleButtonSlowTap;
        public InputAction @Hold => m_Wrapper.m_Mouse_Hold;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void AddCallbacks(IMouseActions instance)
        {
            if (instance == null || m_Wrapper.m_MouseActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MouseActionsCallbackInterfaces.Add(instance);
            @LeftButtonClick.started += instance.OnLeftButtonClick;
            @LeftButtonClick.performed += instance.OnLeftButtonClick;
            @LeftButtonClick.canceled += instance.OnLeftButtonClick;
            @RightButtonClick.started += instance.OnRightButtonClick;
            @RightButtonClick.performed += instance.OnRightButtonClick;
            @RightButtonClick.canceled += instance.OnRightButtonClick;
            @LeftButtonSlowTap.started += instance.OnLeftButtonSlowTap;
            @LeftButtonSlowTap.performed += instance.OnLeftButtonSlowTap;
            @LeftButtonSlowTap.canceled += instance.OnLeftButtonSlowTap;
            @Delta.started += instance.OnDelta;
            @Delta.performed += instance.OnDelta;
            @Delta.canceled += instance.OnDelta;
            @MiddleButtonClick.started += instance.OnMiddleButtonClick;
            @MiddleButtonClick.performed += instance.OnMiddleButtonClick;
            @MiddleButtonClick.canceled += instance.OnMiddleButtonClick;
            @Scroll.started += instance.OnScroll;
            @Scroll.performed += instance.OnScroll;
            @Scroll.canceled += instance.OnScroll;
            @MiddleButtonSlowTap.started += instance.OnMiddleButtonSlowTap;
            @MiddleButtonSlowTap.performed += instance.OnMiddleButtonSlowTap;
            @MiddleButtonSlowTap.canceled += instance.OnMiddleButtonSlowTap;
            @Hold.started += instance.OnHold;
            @Hold.performed += instance.OnHold;
            @Hold.canceled += instance.OnHold;
        }

        private void UnregisterCallbacks(IMouseActions instance)
        {
            @LeftButtonClick.started -= instance.OnLeftButtonClick;
            @LeftButtonClick.performed -= instance.OnLeftButtonClick;
            @LeftButtonClick.canceled -= instance.OnLeftButtonClick;
            @RightButtonClick.started -= instance.OnRightButtonClick;
            @RightButtonClick.performed -= instance.OnRightButtonClick;
            @RightButtonClick.canceled -= instance.OnRightButtonClick;
            @LeftButtonSlowTap.started -= instance.OnLeftButtonSlowTap;
            @LeftButtonSlowTap.performed -= instance.OnLeftButtonSlowTap;
            @LeftButtonSlowTap.canceled -= instance.OnLeftButtonSlowTap;
            @Delta.started -= instance.OnDelta;
            @Delta.performed -= instance.OnDelta;
            @Delta.canceled -= instance.OnDelta;
            @MiddleButtonClick.started -= instance.OnMiddleButtonClick;
            @MiddleButtonClick.performed -= instance.OnMiddleButtonClick;
            @MiddleButtonClick.canceled -= instance.OnMiddleButtonClick;
            @Scroll.started -= instance.OnScroll;
            @Scroll.performed -= instance.OnScroll;
            @Scroll.canceled -= instance.OnScroll;
            @MiddleButtonSlowTap.started -= instance.OnMiddleButtonSlowTap;
            @MiddleButtonSlowTap.performed -= instance.OnMiddleButtonSlowTap;
            @MiddleButtonSlowTap.canceled -= instance.OnMiddleButtonSlowTap;
            @Hold.started -= instance.OnHold;
            @Hold.performed -= instance.OnHold;
            @Hold.canceled -= instance.OnHold;
        }

        public void RemoveCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMouseActions instance)
        {
            foreach (var item in m_Wrapper.m_MouseActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MouseActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MouseActions @Mouse => new MouseActions(this);

    // Keyboard
    private readonly InputActionMap m_Keyboard;
    private List<IKeyboardActions> m_KeyboardActionsCallbackInterfaces = new List<IKeyboardActions>();
    private readonly InputAction m_Keyboard_Ctrl;
    public struct KeyboardActions
    {
        private @MainInputActions m_Wrapper;
        public KeyboardActions(@MainInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Ctrl => m_Wrapper.m_Keyboard_Ctrl;
        public InputActionMap Get() { return m_Wrapper.m_Keyboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardActions set) { return set.Get(); }
        public void AddCallbacks(IKeyboardActions instance)
        {
            if (instance == null || m_Wrapper.m_KeyboardActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_KeyboardActionsCallbackInterfaces.Add(instance);
            @Ctrl.started += instance.OnCtrl;
            @Ctrl.performed += instance.OnCtrl;
            @Ctrl.canceled += instance.OnCtrl;
        }

        private void UnregisterCallbacks(IKeyboardActions instance)
        {
            @Ctrl.started -= instance.OnCtrl;
            @Ctrl.performed -= instance.OnCtrl;
            @Ctrl.canceled -= instance.OnCtrl;
        }

        public void RemoveCallbacks(IKeyboardActions instance)
        {
            if (m_Wrapper.m_KeyboardActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IKeyboardActions instance)
        {
            foreach (var item in m_Wrapper.m_KeyboardActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_KeyboardActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public KeyboardActions @Keyboard => new KeyboardActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IMouseActions
    {
        void OnLeftButtonClick(InputAction.CallbackContext context);
        void OnRightButtonClick(InputAction.CallbackContext context);
        void OnLeftButtonSlowTap(InputAction.CallbackContext context);
        void OnDelta(InputAction.CallbackContext context);
        void OnMiddleButtonClick(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnMiddleButtonSlowTap(InputAction.CallbackContext context);
        void OnHold(InputAction.CallbackContext context);
    }
    public interface IKeyboardActions
    {
        void OnCtrl(InputAction.CallbackContext context);
    }
}
