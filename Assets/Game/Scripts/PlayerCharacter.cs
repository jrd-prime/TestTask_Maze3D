using System;
using Game.Scripts.Help;
using Game.Scripts.Input;
using Mirror;
using R3;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class PlayerCharacter : NetworkBehaviour
    {
        public float MoveSpeed { get; private set; } = 10f;
        [SerializeField] private Dep _dep;
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 offset = new(-7, 15, -7);
        [SerializeField] private float _moveSpeed = 10f;

        private Rigidbody _rb;
        private IUserInput _input;


        private Vector3 _direction;
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

        private void SetDirection(Vector3 direction)
        {
            if (!isLocalPlayer) return;
            Debug.LogWarning("SetDirection in PlayerCharacter called. Direction: " + direction);
            _direction = direction;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            Move();
            CameraFollow();
        }

        private void CameraFollow()
        {
            if (_camera != null) _camera.transform.position = transform.position + offset;
        }

        private void Move()
        {
            // Преобразуем ввод относительно направления камеры
            if (_direction.magnitude > 0.1f) // Проверяем, есть ли ввод
            {
                Vector3 cameraForward = _camera.transform.forward;
                Vector3 cameraRight = _camera.transform.right;

                // Проецируем forward и right камеры на горизонтальную плоскость
                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                // Преобразуем локальный ввод в мировые координаты
                Vector3 moveDirection = _direction.z * cameraForward + _direction.x * cameraRight;
                transform.Translate(moveDirection * (10f * Time.fixedDeltaTime), Space.World);
            }
            // _rb.AddForce(_direction * _moveSpeed * 3f);
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}
