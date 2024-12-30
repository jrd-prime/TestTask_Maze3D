using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Client.Input
{
    public interface IUserInput
    {
        public ReactiveProperty<Vector3> MoveDirection { get; }
    }

    public sealed class PCUserInput : MonoBehaviour, IUserInput
    {
        public ReactiveProperty<Vector3> MoveDirection { get; } = new();

        private PCInputActions _gameInputActions;
        private Vector3 _movementInput;
        private Camera _cam;

        private void Awake()
        {
            InputSystem.EnableDevice(Mouse.current);

            _gameInputActions = new PCInputActions();
            _gameInputActions.Enable();

            _gameInputActions.Player.Move.performed += OnMovePerformed;
            _gameInputActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context) =>
            MoveDirection.Value = context.ReadValue<Vector3>();

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (MoveDirection.CurrentValue != Vector3.zero) MoveDirection.Value = Vector3.zero;
        }

        private void OnDestroy()
        {
            _gameInputActions.Player.Move.performed -= OnMovePerformed;
            _gameInputActions.Player.Move.canceled -= OnMoveCanceled;
            _gameInputActions.Disable();
        }
    }
}
