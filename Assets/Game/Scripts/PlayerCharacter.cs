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
        public float MoveSpeed = 10f;
        [SerializeField] private Dep _dep;
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 offset = new(-7, 15, -7);
        [SerializeField] private float _moveSpeed = 10f;

        private Rigidbody _rb;
        private IUserInput _input;


        private Vector3 direction;
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

        private void SetDirection(Vector3 dir)
        {
            if (!isLocalPlayer) return;
            Debug.LogWarning("direction: " + dir);
            direction = dir;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            Move();
            Rotate();
            CameraFollow();
        }

        private void Rotate()
        {
            var side = direction.x;

            if (side == 0) return;
            var rotationAmount = side * rotationSpeed * Time.fixedDeltaTime;
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, rotationAmount, 0f));
        }

        private void CameraFollow()
        {
            if (_camera != null) _camera.transform.position = transform.position + offset;
        }

        private float _currentSpeed = 0f; // Текущая скорость
        private float maxSpeed = 10f; // Максимальная скорость
        private float _acceleration = 3f; // Коэффициент ускорения (м/с^2)
        private float _deceleration = 5f; // Коэффициент замедления
        private float _reverseSpeed = 2f; // Скорость пятиться назад
        private float _runTimeLimit = 3.5f; // Время до достижения максимальной скорости
        private float _runTime = 0f; // Счётчик времени разгона
        private float currentSpeed = 0f; // текущая скорость
        private float acceleration; // ускорение
        private float deceleration; // замедление
        private float speed = 10f;
        private float rotationSpeed = 300f;

        private void Move()
        {
            var velocity = _rb.linearVelocity;
            // forward 0 0 1
            // back 0 0 -1
            // right 1 0 0
            // left -1 0 0

            // Направление движения по оси Z (вперед, куда смотрит префаб)
            Vector3 moveDirection = transform.forward * direction.z;

            // Обновляем скорость
            _rb.linearVelocity = new Vector3(moveDirection.x * speed, velocity.y, moveDirection.z * speed);
        }

        private void OnDestroy() => _disposables.Dispose();
    }
}
