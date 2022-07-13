// GENERATED AUTOMATICALLY FROM 'Assets/NewData/Settings/InputControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Assets.NewData.Scripts
{
    public class @InputControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""System"",
            ""id"": ""f8087705-a19e-4ca8-81c3-18ac66e5e336"",
            ""actions"": [
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""f3c0ccb7-3251-4b28-a036-dc95571ea965"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""09586518-a81f-463e-96dc-4e110b2ae3a1"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""d9a30bb3-fcb1-4dc7-a0ed-a801de799903"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""f32164a7-e4e5-49e7-9ca3-7781b2f3bf9d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""0c9d5701-d48d-4027-8a5a-8a5d1cd2b004"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""4ed34363-f76b-4583-97cf-04e09eb253d3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""13423105-ed4f-47d5-9d36-46d9f1ba60e8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""35d312c1-a236-4e5c-a452-de79993a9aec"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e1993e6f-7f85-49c5-b5c5-e0d7d0211155"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a06647c5-2c9f-4c9b-a08a-6512a96d093d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1bb9a796-7b15-4fcb-8a94-99ffed0bf012"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Cutscene"",
            ""id"": ""8da3ee3f-4bea-4b6e-9802-989d36dbfb08"",
            ""actions"": [
                {
                    ""name"": ""MoveNext"",
                    ""type"": ""Button"",
                    ""id"": ""3df572dc-883e-4a4c-b18d-a389e25fb928"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d31a4b43-3683-444b-a508-190f67954661"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveNext"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""6e94c8ca-ce93-467a-acd5-b02664300a92"",
            ""actions"": [
                {
                    ""name"": ""CloseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""b8a01667-2acd-46eb-ae83-5a1b485c8950"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b0ff4461-58cc-4945-8999-ba5cfdeb2833"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CloseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // System
            m_System = asset.FindActionMap("System", throwIfNotFound: true);
            m_System_OpenMenu = m_System.FindAction("OpenMenu", throwIfNotFound: true);
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            // Cutscene
            m_Cutscene = asset.FindActionMap("Cutscene", throwIfNotFound: true);
            m_Cutscene_MoveNext = m_Cutscene.FindAction("MoveNext", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_CloseMenu = m_UI.FindAction("CloseMenu", throwIfNotFound: true);
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

        // System
        private readonly InputActionMap m_System;
        private ISystemActions m_SystemActionsCallbackInterface;
        private readonly InputAction m_System_OpenMenu;
        public struct SystemActions
        {
            private @InputControls m_Wrapper;
            public SystemActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @OpenMenu => m_Wrapper.m_System_OpenMenu;
            public InputActionMap Get() { return m_Wrapper.m_System; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(SystemActions set) { return set.Get(); }
            public void SetCallbacks(ISystemActions instance)
            {
                if (m_Wrapper.m_SystemActionsCallbackInterface != null)
                {
                    @OpenMenu.started -= m_Wrapper.m_SystemActionsCallbackInterface.OnOpenMenu;
                    @OpenMenu.performed -= m_Wrapper.m_SystemActionsCallbackInterface.OnOpenMenu;
                    @OpenMenu.canceled -= m_Wrapper.m_SystemActionsCallbackInterface.OnOpenMenu;
                }
                m_Wrapper.m_SystemActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @OpenMenu.started += instance.OnOpenMenu;
                    @OpenMenu.performed += instance.OnOpenMenu;
                    @OpenMenu.canceled += instance.OnOpenMenu;
                }
            }
        }
        public SystemActions @System => new SystemActions(this);

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Jump;
        public struct PlayerActions
        {
            private @InputControls m_Wrapper;
            public PlayerActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // Cutscene
        private readonly InputActionMap m_Cutscene;
        private ICutsceneActions m_CutsceneActionsCallbackInterface;
        private readonly InputAction m_Cutscene_MoveNext;
        public struct CutsceneActions
        {
            private @InputControls m_Wrapper;
            public CutsceneActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveNext => m_Wrapper.m_Cutscene_MoveNext;
            public InputActionMap Get() { return m_Wrapper.m_Cutscene; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CutsceneActions set) { return set.Get(); }
            public void SetCallbacks(ICutsceneActions instance)
            {
                if (m_Wrapper.m_CutsceneActionsCallbackInterface != null)
                {
                    @MoveNext.started -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnMoveNext;
                    @MoveNext.performed -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnMoveNext;
                    @MoveNext.canceled -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnMoveNext;
                }
                m_Wrapper.m_CutsceneActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveNext.started += instance.OnMoveNext;
                    @MoveNext.performed += instance.OnMoveNext;
                    @MoveNext.canceled += instance.OnMoveNext;
                }
            }
        }
        public CutsceneActions @Cutscene => new CutsceneActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_CloseMenu;
        public struct UIActions
        {
            private @InputControls m_Wrapper;
            public UIActions(@InputControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @CloseMenu => m_Wrapper.m_UI_CloseMenu;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @CloseMenu.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseMenu;
                    @CloseMenu.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseMenu;
                    @CloseMenu.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCloseMenu;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @CloseMenu.started += instance.OnCloseMenu;
                    @CloseMenu.performed += instance.OnCloseMenu;
                    @CloseMenu.canceled += instance.OnCloseMenu;
                }
            }
        }
        public UIActions @UI => new UIActions(this);
        public interface ISystemActions
        {
            void OnOpenMenu(InputAction.CallbackContext context);
        }
        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
        }
        public interface ICutsceneActions
        {
            void OnMoveNext(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnCloseMenu(InputAction.CallbackContext context);
        }
    }
}
