using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Game.Scripts
{
    public interface ISpawnManager
    {
        Transform GetMainSpawnPoint();
        Transform GetRandomSpawnPoint();
    }

    public class SpawnManager : MonoBehaviour, ISpawnManager
    {
        [SerializeField] private Transform mainSpawnPoint;
        [SerializeField] private Transform secondSpawnPoint;
        [SerializeField] private Transform thirdSpawnPoint;
        [SerializeField] private Transform fourthSpawnPoint;

        private readonly Queue<Transform> _pointsQueue = new();

        private int _spawnCount;

        private void Awake()
        {
            Debug.Log("SpawnManager  awake.");
        }

        private void Start()
        {
            Debug.Log("SpawnManager started.");
        }

        private void OnEnable()
        {
            Debug.Log("SpawnManager enabled.");
            _spawnCount = 0;
            _pointsQueue.Enqueue(secondSpawnPoint);
            _pointsQueue.Enqueue(thirdSpawnPoint);
            _pointsQueue.Enqueue(fourthSpawnPoint);
        }

        private void OnDestroy() => _spawnCount = 0;
        public Transform GetMainSpawnPoint() => mainSpawnPoint;

        public Transform GetRandomSpawnPoint() => secondSpawnPoint;
    }
}
