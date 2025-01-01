using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class CustomNetworkBehaviour : NetworkBehaviour
    {
    }
}
