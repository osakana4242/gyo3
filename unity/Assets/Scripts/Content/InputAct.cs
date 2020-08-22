// GENERATED AUTOMATICALLY FROM 'Assets/InputAct.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Osakana4242.Content
{
    public class @InputAct : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputAct()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputAct"",
    ""maps"": [
        {
            ""name"": ""ShipAction"",
            ""id"": ""8f2315f1-fcc8-4259-babd-f1f1dcbd4a8f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8f9d2b60-0663-4334-98e0-e419534a58d3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Press"",
                    ""type"": ""Button"",
                    ""id"": ""0db04426-3fd9-4cbb-b70a-e4548f08eb36"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fb80de3f-ea1a-40fa-ac22-73e7f3867286"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ebbabde-fd0c-4336-8273-26f6dfb03292"",
                    ""path"": ""<Pointer>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Press"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc55ea17-bc83-445f-912c-6975c1ca7649"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Press"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // ShipAction
            m_ShipAction = asset.FindActionMap("ShipAction", throwIfNotFound: true);
            m_ShipAction_Move = m_ShipAction.FindAction("Move", throwIfNotFound: true);
            m_ShipAction_Press = m_ShipAction.FindAction("Press", throwIfNotFound: true);
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

        // ShipAction
        private readonly InputActionMap m_ShipAction;
        private IShipActionActions m_ShipActionActionsCallbackInterface;
        private readonly InputAction m_ShipAction_Move;
        private readonly InputAction m_ShipAction_Press;
        public struct ShipActionActions
        {
            private @InputAct m_Wrapper;
            public ShipActionActions(@InputAct wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_ShipAction_Move;
            public InputAction @Press => m_Wrapper.m_ShipAction_Press;
            public InputActionMap Get() { return m_Wrapper.m_ShipAction; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ShipActionActions set) { return set.Get(); }
            public void SetCallbacks(IShipActionActions instance)
            {
                if (m_Wrapper.m_ShipActionActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnMove;
                    @Press.started -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnPress;
                    @Press.performed -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnPress;
                    @Press.canceled -= m_Wrapper.m_ShipActionActionsCallbackInterface.OnPress;
                }
                m_Wrapper.m_ShipActionActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Press.started += instance.OnPress;
                    @Press.performed += instance.OnPress;
                    @Press.canceled += instance.OnPress;
                }
            }
        }
        public ShipActionActions @ShipAction => new ShipActionActions(this);
        public interface IShipActionActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnPress(InputAction.CallbackContext context);
        }
    }
}
