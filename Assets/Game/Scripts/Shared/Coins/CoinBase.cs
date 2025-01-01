using System;
using Game.Scripts.Server;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared.Coins
{
    [RequireComponent(typeof(Collider))]
    public abstract class CoinBase : NetworkBehaviour
    {
        [SerializeField] protected LayerMask triggerMask;

        [SyncVar] protected int Points;
        [SyncVar] protected bool IsInitialized;

        protected GameManager GameManager;

        private void Awake()
        {
            GameManager = GameManager.Instance;
            if (GameManager == null) throw new NullReferenceException(nameof(GameManager));
        }
    }
}
