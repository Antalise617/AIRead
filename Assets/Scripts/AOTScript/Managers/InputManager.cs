using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameFramework.Managers
{
    public struct InputData
    {
        public float2 Move;
        public bool Fire;
        public bool Jump;
        public float2 Look;
    }

    public class InputManager : Singleton<InputManager>
    {
        private PlayerInputActions _inputActions;
        private InputData _currentInput;

        public void Initialize()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();

            // 订阅输入事件
            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMove;

            _inputActions.Player.Fire.performed += OnFire;
            _inputActions.Player.Fire.canceled += OnFire;

            _inputActions.Player.Jump.performed += OnJump;
            _inputActions.Player.Jump.canceled += OnJump;

            Debug.Log("输入系统初始化完成");
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _currentInput.Move = new float2(value.x, value.y);
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            _currentInput.Fire = context.performed;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _currentInput.Jump = context.performed;
        }

        public InputData GetInputData()
        {
            return _currentInput;
        }

        private void OnDestroy()
        {
            _inputActions?.Disable();
            _inputActions?.Dispose(); 
        }
    }
}
