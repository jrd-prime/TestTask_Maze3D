using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Game.Scripts.Input
{
    public interface IUserInput
    {
        public ReactiveProperty<Vector3> MoveDirection { get; }
        public ReactiveProperty<Vector2> MousePosition { get; }
        public Subject<Unit> Shoot { get; }
    }

    public sealed class PCUserInput : MonoBehaviour, IUserInput
    {
        public ReactiveProperty<Vector3> MoveDirection { get; } = new();
        public ReactiveProperty<Vector2> MousePosition { get; } = new();
        public Subject<Unit> Shoot { get; } = new();

        private PCInputActions _gameInputActions;
        private Vector3 _movementInput;
        private Camera _cam;

        private void Awake()
        {
            Screen.SetResolution(1280, 720, false);
            InputSystem.EnableDevice(Mouse.current);

            _gameInputActions = new PCInputActions();
            _gameInputActions.Enable();

            _gameInputActions.Player.Move.performed += OnMovePerformed;
            _gameInputActions.Player.Move.canceled += OnMoveCanceled;
            _gameInputActions.Player.Click.performed += OnShootPerformed;
        }

        private void OnMovePerformed(InputAction.CallbackContext context) =>
            MoveDirection.Value = context.ReadValue<Vector3>();


        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (MoveDirection.CurrentValue != Vector3.zero) MoveDirection.Value = Vector3.zero;
        }

        private void OnShootPerformed(InputAction.CallbackContext context) => Shoot.OnNext(Unit.Default);

        private void OnDestroy()
        {
            _gameInputActions.Player.Move.performed -= OnMovePerformed;
            _gameInputActions.Player.Move.canceled -= OnMoveCanceled;
            _gameInputActions.Player.Click.performed -= OnShootPerformed;
            _gameInputActions.Disable();
        }
    }
}
