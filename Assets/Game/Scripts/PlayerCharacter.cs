using System;
using Game.Scripts.Client.Input;
using Game.Scripts.Help;
using Mirror;
using R3;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class PlayerCharacter : NetworkBehaviour
    {
        [SerializeField] private Dep _dep;
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 offset = new(-7, 15, -7);
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 300f;

        private Rigidbody _rb;
        private IUserInput _input;

        private Vector3 inputDirection;
        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            if (_dep == null) throw new NullReferenceException(nameof(_dep));
        }

        private void Start()
        {
            if (!isLocalPlayer) return;
            Debug.LogWarning("PlayerCharacter started.");
            _input = _dep.GetDependency<IUserInput>();
            _rb = GetComponent<Rigidbody>();
            _camera = Camera.main;

            _input.MoveDirection.Subscribe(SetDirection).AddTo(_disposables);
        }

        private void FixedUpdate()
        {
            if (!isOwned) return;

            Move();
            Rotate();
            CameraFollow();
        }

        private void Rotate()
        {
            var side = inputDirection.x;

            if (side == 0) return;
            var rotationAmount = side * rotationSpeed * Time.fixedDeltaTime;
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, rotationAmount, 0f));
        }

        private void CameraFollow()
        {
            if (_camera != null) _camera.transform.position = transform.position + offset;
        }

        private void SetDirection(Vector3 value) => inputDirection = value;

        private void Move()
        {
            // forward 0 0 1 // back 0 0 -1 // right 1 0 0 // left -1 0 0
            var velocity = _rb.linearVelocity;
            Vector3 newVelocity;
            var forward = transform.forward;

            if (inputDirection.x == 0)
            {
                if (inputDirection.z > 0)
                {
                    // Debug.LogWarning("acceleration");
                    newVelocity = forward * moveSpeed;
                }
                else if (inputDirection.z < 0)
                {
                    // Debug.LogWarning("deceleration");
                    newVelocity = -forward * moveSpeed;
                }
                else
                {
                    // Debug.LogWarning("not acceleration / not deceleration");
                    newVelocity = new Vector3(0, velocity.y, 0);
                }
            }
            else
            {
                // Debug.LogWarning("Moving with rotation without acceleration / deceleration");
                var dir = forward * inputDirection.z;
                newVelocity = new Vector3(dir.x * moveSpeed, velocity.y, dir.z * moveSpeed);
            }

            _rb.linearVelocity = newVelocity;
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}
