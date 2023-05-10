//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Input Actions/Input.inputactions
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

public partial class @Input: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input"",
    ""maps"": [
        {
            ""name"": ""Carrom"",
            ""id"": ""dd1eca24-cb72-474a-8709-b5ead0e5a192"",
            ""actions"": [
                {
                    ""name"": ""MousePress"",
                    ""type"": ""Button"",
                    ""id"": ""29b2694d-9180-4fa1-b0b0-f107203f1d5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""0d092d87-9f2f-4644-bc6d-7195e7439ff3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""82165b25-2676-4b0b-9984-2c3209f898a4"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffdbda41-78c8-4fad-b402-def87dd660dd"",
                    ""path"": ""<Touchscreen>/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c24cd6c0-6761-4c7a-b035-4dce6e1be356"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a097104f-5be2-4e71-b00d-0eef3534afe7"",
                    ""path"": ""<Touchscreen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Carrom
        m_Carrom = asset.FindActionMap("Carrom", throwIfNotFound: true);
        m_Carrom_MousePress = m_Carrom.FindAction("MousePress", throwIfNotFound: true);
        m_Carrom_MousePosition = m_Carrom.FindAction("MousePosition", throwIfNotFound: true);
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

    // Carrom
    private readonly InputActionMap m_Carrom;
    private List<ICarromActions> m_CarromActionsCallbackInterfaces = new List<ICarromActions>();
    private readonly InputAction m_Carrom_MousePress;
    private readonly InputAction m_Carrom_MousePosition;
    public struct CarromActions
    {
        private @Input m_Wrapper;
        public CarromActions(@Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @MousePress => m_Wrapper.m_Carrom_MousePress;
        public InputAction @MousePosition => m_Wrapper.m_Carrom_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Carrom; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CarromActions set) { return set.Get(); }
        public void AddCallbacks(ICarromActions instance)
        {
            if (instance == null || m_Wrapper.m_CarromActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CarromActionsCallbackInterfaces.Add(instance);
            @MousePress.started += instance.OnMousePress;
            @MousePress.performed += instance.OnMousePress;
            @MousePress.canceled += instance.OnMousePress;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
        }

        private void UnregisterCallbacks(ICarromActions instance)
        {
            @MousePress.started -= instance.OnMousePress;
            @MousePress.performed -= instance.OnMousePress;
            @MousePress.canceled -= instance.OnMousePress;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
        }

        public void RemoveCallbacks(ICarromActions instance)
        {
            if (m_Wrapper.m_CarromActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICarromActions instance)
        {
            foreach (var item in m_Wrapper.m_CarromActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CarromActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CarromActions @Carrom => new CarromActions(this);
    public interface ICarromActions
    {
        void OnMousePress(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
