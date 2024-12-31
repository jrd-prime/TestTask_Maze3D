using System;
using System.Collections.Generic;
using Game.Scripts.Coins;
using UnityEngine;

namespace Game.Scripts.Data.SO
{
    [CreateAssetMenu(menuName = "Settings/CoinsManagerSO", fileName = "CoinsManagerSO", order = 100)]
    public class CoinsManagerSO : ScriptableObject
    {
        public Coin coinPrefab;
        public int pointsPerCoin = 5;
        public int onStartSpawnDelaySeconds = 5;
        public int spawnPointsCount = 8;
        public List<Transform> spawnPoints;

        private void OnValidate()
        {
            if (coinPrefab == null) throw new NullReferenceException(nameof(coinPrefab));
            if (spawnPoints.Count == 0) throw new NullReferenceException(nameof(spawnPoints));
        }
    }
}
