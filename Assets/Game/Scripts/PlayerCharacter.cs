using Game.Scripts.Client.Input;
using Game.Scripts.Shared;
using Mirror;
using R3;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class PlayerCharacter : NetworkBehaviour
    {
        [SerializeField] private Vector3 offset = new(-7, 15, -7);
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 300f;

        private Camera _camera;
        private Rigidbody _rb;
        private IUserInput _input;

        private Vector3 _inputDirection;
        private readonly CompositeDisposable _disposables = new();
        private GameManager _gameManager;

        public ReactiveProperty<int> Score { get; } = new();

        [ClientCallback]
        private void Start()
        {
            if (!isOwned) return;

            Debug.LogWarning("PlayerCharacter started.");

            _input = FindFirstObjectByType<PCUserInput>();
            _gameManager = GameManager.Instance;

            _rb = GetComponent<Rigidbody>();
            _camera = Camera.main;

            _input.MoveDirection.Subscribe(SetDirection).AddTo(_disposables);
        }

        [ClientCallback]
        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            Move();
            Rotate();
            CameraFollow();
        }

        [TargetRpc]
        public void ShowScoreForClientRpc(NetworkConnection target, int score)
        {
            Debug.LogWarning($"=== Score for You (Player {target}) ===");
            Debug.LogWarning($"Your Score: {score} pts.");
            Debug.LogWarning("=====================================");
        }

        private void CameraFollow()
        {
            if (_camera is not null) _camera.transform.position = transform.position + offset;
        }

        private void Move()
        {
            // forward 0 0 1 // back 0 0 -1 // right 1 0 0 // left -1 0 0
            var velocity = _rb.linearVelocity;
            Vector3 newVelocity;
            var forward = transform.forward;

            if (_inputDirection.x == 0)
            {
                if (_inputDirection.z > 0) newVelocity = forward * moveSpeed; // acceleration
                else if (_inputDirection.z < 0) newVelocity = -forward * moveSpeed; // deceleration
                else newVelocity = new Vector3(0, velocity.y, 0); // not acceleration / not deceleration
            }
            else
            {
                // Moving with rotation without acceleration / deceleration
                var dir = forward * _inputDirection.z;
                newVelocity = new Vector3(dir.x * moveSpeed, velocity.y, dir.z * moveSpeed);
            }

            _rb.linearVelocity = newVelocity;
        }

        private void Rotate()
        {
            var side = _inputDirection.x;
            if (side == 0) return;
            var rotationAmount = side * rotationSpeed * Time.fixedDeltaTime;
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, rotationAmount, 0f));
        }

        private void SetDirection(Vector3 value) => _inputDirection = value;

        private void OnDestroy() => _disposables.Dispose();
    }
}
