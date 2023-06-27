using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;  //NEcesario

namespace UnityTutorial.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput m_PlayerInput;

        public Vector2 m_Move { get; private set; }
        public Vector2 m_Look { get; private set; }
        public bool m_Run { get; private set; }
        public bool m_Jump { get; private set; }

        private InputActionMap m_CurrentMap;

        private InputAction m_MoveAction;

        private InputAction m_LookAction;

        private InputAction m_RunAction;

        private InputAction m_JumpAction;

        private void Awake()
        {

            HideCursor();

            m_CurrentMap = m_PlayerInput.currentActionMap;
            m_MoveAction = m_CurrentMap.FindAction("Move");
            m_LookAction = m_CurrentMap.FindAction("Look");
            m_RunAction = m_CurrentMap.FindAction("Run");

            m_JumpAction = m_CurrentMap.FindAction("Jump"); ///


            m_MoveAction.performed += onMove;
            m_LookAction.performed += onLook;
            m_RunAction.performed += onRun;

            m_JumpAction.performed += onJump;//
            //m_JumpAction.started += onJump; //Asi no es cuando se acaba de presionar del todo, si no cuando se inicia

            m_MoveAction.canceled += onMove;
            m_LookAction.canceled += onLook;
            m_RunAction.canceled += onRun;

            m_JumpAction.canceled += onJump; ///
        }


        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Performed actions
        private void onMove(InputAction.CallbackContext context)
        {
            m_Move = context.ReadValue<Vector2>();
        }

        private void onLook(InputAction.CallbackContext context)
        {
            m_Look = context.ReadValue<Vector2>();
        }

        private void onRun(InputAction.CallbackContext context)
        {
            m_Run = context.ReadValueAsButton();
        }

        private void onJump(InputAction.CallbackContext context)
        {
            //if(context.interaction is TapInteraction) //No usar
            m_Jump = context.ReadValueAsButton();
        }


        private void OnEnable()
        {
            m_CurrentMap.Enable();
        }

        private void OnDisable()
        {
            m_CurrentMap.Disable();
        }
    }
}
