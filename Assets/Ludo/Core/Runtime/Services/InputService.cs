using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ludo.Core.Services
{
    public sealed class InputService : IInputService
    {
        readonly InputSystem_Actions _actions;
        readonly Action<InputAction.CallbackContext> _movePerformed;
        readonly Action<InputAction.CallbackContext> _moveCanceled;
        readonly Action<InputAction.CallbackContext> _lookPerformed;
        readonly Action<InputAction.CallbackContext> _lookCanceled;
        readonly Action<InputAction.CallbackContext> _attackPerformed;

        public event Action<Vector2> Move;
        public event Action<Vector2> Look;
        public event Action Attack;

        public InputService()
        {
            _actions = new InputSystem_Actions();

            _movePerformed = ctx => Move?.Invoke(ctx.ReadValue<Vector2>());
            _moveCanceled = ctx => Move?.Invoke(Vector2.zero);
            _lookPerformed = ctx => Look?.Invoke(ctx.ReadValue<Vector2>());
            _lookCanceled = ctx => Look?.Invoke(Vector2.zero);
            _attackPerformed = _ => Attack?.Invoke();

            Bind();
            _actions.Player.Enable();
        }

        void Bind()
        {
            _actions.Player.Move.performed += _movePerformed;
            _actions.Player.Move.canceled += _moveCanceled;
            _actions.Player.Look.performed += _lookPerformed;
            _actions.Player.Look.canceled += _lookCanceled;
            _actions.Player.Attack.performed += _attackPerformed;
        }

        public void EnablePlayer() => _actions.Player.Enable();
        public void DisablePlayer() => _actions.Player.Disable();

        public void Dispose()
        {
            _actions.Player.Move.performed -= _movePerformed;
            _actions.Player.Move.canceled -= _moveCanceled;
            _actions.Player.Look.performed -= _lookPerformed;
            _actions.Player.Look.canceled -= _lookCanceled;
            _actions.Player.Attack.performed -= _attackPerformed;
            _actions.Dispose();
        }
    }
}
