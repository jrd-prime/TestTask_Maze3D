using System;
using Game.Scripts.Data.SO;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Manager
{
    public class CoinsManager : NetworkBehaviour
    {
        [SerializeField] private CoinsManagerSO settings;

        private void Awake()
        {
            if (settings == null) throw new NullReferenceException(nameof(settings));
        }
    }
}
