// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/Player Controlls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControlls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControlls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Controlls"",
    ""maps"": [
        {
            ""name"": ""GamePlay"",
            ""id"": ""72ff41a5-aeef-443e-8263-d005cafe3ba7"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""20eb195f-98ef-4a65-a70f-8396e4c36de0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start Grapple"",
                    ""type"": ""Button"",
                    ""id"": ""b6a89ec6-3a04-4316-a9e4-0820621fa862"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Stop Grapple"",
                    ""type"": ""Button"",
                    ""id"": ""4bc05b36-892f-4948-96c5-c4b1595e330e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""dfd0a6a1-abb8-4c6b-9d7e-f976e9c760c3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""40780caa-96a5-4c2a-81d4-ce238e48c331"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""70dcee86-a434-4e8c-b574-78479c829339"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""686145d9-bcd3-42f7-9a0b-de05052178bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""9c74fe6f-4117-4e90-b778-704b9c761058"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""58eb3391-721e-4b7e-b558-e8799c4e31b0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""88acd956-d717-44c6-8af0-293277a04469"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""d68ac358-1cbc-411a-b516-e727bccd4f78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Controller Back"",
                    ""type"": ""Button"",
                    ""id"": ""87c1964a-3c7c-435f-a849-d110ee3b1b17"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""StartBatman Grapple"",
                    ""type"": ""Button"",
                    ""id"": ""dfcc6911-f34f-4caf-92c2-b9e347252065"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop Cube"",
                    ""type"": ""Button"",
                    ""id"": ""fd9bbf7d-5dc1-44e7-a7b7-3ac1649c50b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""bcf3e5e1-f05c-4e91-a015-6e266461b85a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""e87a9b53-23c4-4e54-aca2-3e68e0a48df5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""981a912d-502d-4cd6-bf36-755889b3d005"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""ebec2286-daf5-4f5f-8f8e-fea5b8ab536c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""eba20251-2ac0-43c6-9f69-6ca6d6ef3257"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1e871f95-c8f1-4e2e-9c1b-b62483df9f8d"",
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
                    ""id"": ""36591dda-35b3-498d-bc67-d9a9290f11b8"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4da47996-5bf7-4252-af6d-098ef25c225c"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d7a34548-b4d8-4040-92db-2e639f10bcda"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b0183e8b-1834-4e4d-8cc4-e331b67d3945"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a2732b9f-b4af-4067-b305-dd281b0f6c8b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd7497ab-9310-454a-82fe-ab03cdf87807"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press(pressPoint=0.2)"",
                    ""processors"": ""Normalize(max=1)"",
                    ""groups"": """",
                    ""action"": ""Start Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""003fe933-9e3f-4dd8-a97d-8585594e199c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cff95999-9bae-475f-88ab-e3fcaf78bf1e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stop Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64c84bfa-3172-4d61-a336-c7df1d602730"",
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
                    ""id"": ""072be38d-690c-4290-a420-49fea24b4d67"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b9b8f25-b262-4264-9791-0f843ca36858"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef970605-69cb-4362-be61-ec244fa61e13"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5840cc25-bcc6-4ba4-b92b-723f4b367a81"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ea7bd63-fe8e-4754-9a5e-000c374d403f"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""580c1ca0-a69d-444a-a6e1-fc00bb972bfe"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec2ca1fc-beb2-4b0b-a40c-e5310d7a499e"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""503c8cfc-b29c-4a1a-b12f-68bb8990a3a3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03d49fd5-b45f-4ef5-97c6-dee2e54bd590"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07a61e25-d32a-4106-9b50-a0cbd960edfb"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a9e52c5-47ca-48c2-8b2c-f1fac02f78fc"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=2.6,y=2.6)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0bfadb5c-f354-4bdf-b4a7-37144ca99f67"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df635693-dba7-43f6-9268-f2680bf18910"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c42164c7-b612-418c-a12c-3cd83c587101"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""703488aa-4e76-4bdb-904d-5bb203452a0d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec88dc2f-baaa-4cf0-80fb-735d0582530c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Controller Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""494e8edc-3c8f-4c7c-b465-e477903c7847"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StartBatman Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b413451-918f-4f15-9074-d9c1c3f80777"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": ""Press(pressPoint=0.2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StartBatman Grapple"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a5fd8d4-0f95-4f16-9123-f350968bd561"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d40f55e4-a6dc-4643-bc35-f0e499f1ddb3"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop Cube"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GamePlay
        m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
        m_GamePlay_Move = m_GamePlay.FindAction("Move", throwIfNotFound: true);
        m_GamePlay_StartGrapple = m_GamePlay.FindAction("Start Grapple", throwIfNotFound: true);
        m_GamePlay_StopGrapple = m_GamePlay.FindAction("Stop Grapple", throwIfNotFound: true);
        m_GamePlay_Scroll = m_GamePlay.FindAction("Scroll", throwIfNotFound: true);
        m_GamePlay_Jump = m_GamePlay.FindAction("Jump", throwIfNotFound: true);
        m_GamePlay_Dash = m_GamePlay.FindAction("Dash", throwIfNotFound: true);
        m_GamePlay_Crouch = m_GamePlay.FindAction("Crouch", throwIfNotFound: true);
        m_GamePlay_Pause = m_GamePlay.FindAction("Pause", throwIfNotFound: true);
        m_GamePlay_Look = m_GamePlay.FindAction("Look", throwIfNotFound: true);
        m_GamePlay_Back = m_GamePlay.FindAction("Back", throwIfNotFound: true);
        m_GamePlay_Interact = m_GamePlay.FindAction("Interact", throwIfNotFound: true);
        m_GamePlay_ControllerBack = m_GamePlay.FindAction("Controller Back", throwIfNotFound: true);
        m_GamePlay_StartBatmanGrapple = m_GamePlay.FindAction("StartBatman Grapple", throwIfNotFound: true);
        m_GamePlay_DropCube = m_GamePlay.FindAction("Drop Cube", throwIfNotFound: true);
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

    // GamePlay
    private readonly InputActionMap m_GamePlay;
    private IGamePlayActions m_GamePlayActionsCallbackInterface;
    private readonly InputAction m_GamePlay_Move;
    private readonly InputAction m_GamePlay_StartGrapple;
    private readonly InputAction m_GamePlay_StopGrapple;
    private readonly InputAction m_GamePlay_Scroll;
    private readonly InputAction m_GamePlay_Jump;
    private readonly InputAction m_GamePlay_Dash;
    private readonly InputAction m_GamePlay_Crouch;
    private readonly InputAction m_GamePlay_Pause;
    private readonly InputAction m_GamePlay_Look;
    private readonly InputAction m_GamePlay_Back;
    private readonly InputAction m_GamePlay_Interact;
    private readonly InputAction m_GamePlay_ControllerBack;
    private readonly InputAction m_GamePlay_StartBatmanGrapple;
    private readonly InputAction m_GamePlay_DropCube;
    public struct GamePlayActions
    {
        private @PlayerControlls m_Wrapper;
        public GamePlayActions(@PlayerControlls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_GamePlay_Move;
        public InputAction @StartGrapple => m_Wrapper.m_GamePlay_StartGrapple;
        public InputAction @StopGrapple => m_Wrapper.m_GamePlay_StopGrapple;
        public InputAction @Scroll => m_Wrapper.m_GamePlay_Scroll;
        public InputAction @Jump => m_Wrapper.m_GamePlay_Jump;
        public InputAction @Dash => m_Wrapper.m_GamePlay_Dash;
        public InputAction @Crouch => m_Wrapper.m_GamePlay_Crouch;
        public InputAction @Pause => m_Wrapper.m_GamePlay_Pause;
        public InputAction @Look => m_Wrapper.m_GamePlay_Look;
        public InputAction @Back => m_Wrapper.m_GamePlay_Back;
        public InputAction @Interact => m_Wrapper.m_GamePlay_Interact;
        public InputAction @ControllerBack => m_Wrapper.m_GamePlay_ControllerBack;
        public InputAction @StartBatmanGrapple => m_Wrapper.m_GamePlay_StartBatmanGrapple;
        public InputAction @DropCube => m_Wrapper.m_GamePlay_DropCube;
        public InputActionMap Get() { return m_Wrapper.m_GamePlay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GamePlayActions set) { return set.Get(); }
        public void SetCallbacks(IGamePlayActions instance)
        {
            if (m_Wrapper.m_GamePlayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                @StartGrapple.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartGrapple;
                @StartGrapple.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartGrapple;
                @StartGrapple.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartGrapple;
                @StopGrapple.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStopGrapple;
                @StopGrapple.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStopGrapple;
                @StopGrapple.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStopGrapple;
                @Scroll.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnScroll;
                @Jump.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnJump;
                @Dash.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDash;
                @Crouch.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnCrouch;
                @Pause.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPause;
                @Look.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnLook;
                @Back.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnBack;
                @Interact.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
                @ControllerBack.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnControllerBack;
                @ControllerBack.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnControllerBack;
                @ControllerBack.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnControllerBack;
                @StartBatmanGrapple.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartBatmanGrapple;
                @StartBatmanGrapple.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartBatmanGrapple;
                @StartBatmanGrapple.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnStartBatmanGrapple;
                @DropCube.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropCube;
                @DropCube.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropCube;
                @DropCube.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDropCube;
            }
            m_Wrapper.m_GamePlayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @StartGrapple.started += instance.OnStartGrapple;
                @StartGrapple.performed += instance.OnStartGrapple;
                @StartGrapple.canceled += instance.OnStartGrapple;
                @StopGrapple.started += instance.OnStopGrapple;
                @StopGrapple.performed += instance.OnStopGrapple;
                @StopGrapple.canceled += instance.OnStopGrapple;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @ControllerBack.started += instance.OnControllerBack;
                @ControllerBack.performed += instance.OnControllerBack;
                @ControllerBack.canceled += instance.OnControllerBack;
                @StartBatmanGrapple.started += instance.OnStartBatmanGrapple;
                @StartBatmanGrapple.performed += instance.OnStartBatmanGrapple;
                @StartBatmanGrapple.canceled += instance.OnStartBatmanGrapple;
                @DropCube.started += instance.OnDropCube;
                @DropCube.performed += instance.OnDropCube;
                @DropCube.canceled += instance.OnDropCube;
            }
        }
    }
    public GamePlayActions @GamePlay => new GamePlayActions(this);
    public interface IGamePlayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnStartGrapple(InputAction.CallbackContext context);
        void OnStopGrapple(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnControllerBack(InputAction.CallbackContext context);
        void OnStartBatmanGrapple(InputAction.CallbackContext context);
        void OnDropCube(InputAction.CallbackContext context);
    }
}
