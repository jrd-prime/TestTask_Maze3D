using System;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Coins
{
    [RequireComponent(typeof(Collider))]
    public abstract class CoinBase : NetworkBehaviour
    {
        [SerializeField] protected LayerMask triggerMask;
        protected int Points;

        protected bool IsInitialized;
    }
}
