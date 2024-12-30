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
            Debug.LogWarning("direction: " + direction);
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

        private float _currentSpeed = 0f; // Текущая скорость
        private float _maxSpeed = 10f; // Максимальная скорость
        private float _acceleration = 3f; // Коэффициент ускорения (м/с^2)
        private float _deceleration = 5f; // Коэффициент замедления
        private float _reverseSpeed = 2f; // Скорость пятиться назад
        private float _runTimeLimit = 3.5f; // Время до достижения максимальной скорости
        private float _runTime = 0f; // Счётчик времени разгона

       private void Move()
{
    var velocity = _rb.linearVelocity;

    if (_direction.magnitude > 0.1f)
    {
        // Направление камеры
        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;

        // Проецируем forward и right камеры на горизонтальную плоскость
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Преобразуем локальный ввод в мировые координаты
        Vector3 moveDirection = _direction.z * cameraForward + _direction.x * cameraRight;

        if (_direction.z > 0) // Если движение вперед
        {
            // Увеличиваем скорость до максимальной
            _runTime += Time.fixedDeltaTime;
            _currentSpeed = Mathf.Clamp(
                _currentSpeed + _acceleration * Time.fixedDeltaTime,
                0,
                _maxSpeed);

            // После достижения максимальной скорости прекращаем разгон
            if (_runTime >= _runTimeLimit)
            {
                _currentSpeed = _maxSpeed;
            }
        }
        else if (_direction.z < 0) // Если движение назад
        {
            // Инвертируем вектор для движения назад
            moveDirection = -moveDirection;

            // При движении назад сбрасываем скорость до минимальной (если было сильное ускорение)
            if (_currentSpeed > 0)
            {
                // Замедление до остановки
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.fixedDeltaTime);
            }
            else
            {
                // Пятимся назад после остановки
                _currentSpeed = Mathf.Clamp(
                    _currentSpeed - _acceleration * Time.fixedDeltaTime,
                    -_reverseSpeed,
                    0);
            }
        }

        // Применяем силу для изменения скорости на основе движения
        Vector3 targetVelocity = moveDirection.normalized * _currentSpeed;
        Vector3 velocityChange = targetVelocity - velocity;

        // Используем ForceMode.VelocityChange для быстрого изменения скорости
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    else
    {
        // Если нет ввода, замедляем игрока
        if (velocity.magnitude > 0.1f)
        {
            // Замедление: постепенно уменьшаем скорость
            Vector3 decelerationForce = -velocity.normalized * _deceleration;
            _rb.AddForce(decelerationForce, ForceMode.Acceleration);
        }
        else
        {
            // Остановка, если скорость очень мала
            _rb.linearVelocity = Vector3.zero; // Останавливаем полностью, если скорость мала
        }

        // Постепенно снижаем скорость до нуля
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0, _deceleration * Time.fixedDeltaTime);
    }
}



        private void OnDestroy() => _disposables.Dispose();
    }
}
