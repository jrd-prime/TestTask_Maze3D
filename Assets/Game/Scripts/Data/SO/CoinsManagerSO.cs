using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Data.SO
{
    [CreateAssetMenu(menuName = "Settings/CoinsManagerSO", fileName = "CoinsManagerSO", order = 100)]
    public class CoinsManagerSO : ScriptableObject
    {
        public int pointsPerCoin = 5;
        public int onStartSpawnDelaySeconds = 5;
        public int spawnPointsCount = 8;
        public List<Transform> spawnPoints;
    }
}
