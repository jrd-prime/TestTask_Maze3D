using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class CustomNetworkBehaviour : NetworkBehaviour
    {
        public sealed override void OnStartServer()
        {
            Debug.LogWarning("OnStartServer CustomNetworkBehaviour");
            LoadDependencies();
            OnServerStart();
            base.OnStartServer();
        }


        protected virtual void OnServerStart()
        {
        }

        protected abstract void LoadDependencies();
    }
}
