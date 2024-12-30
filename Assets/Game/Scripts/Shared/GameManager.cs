using System;
using Game.Scripts.Help;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    public interface IGameManager
    {
        // public ISpawnManager SpawnManager { get; }
    }

    public class GameManager : CustomNetworkBehaviour, IGameManager
    {
        [SerializeField] private PlayerCharacter playerPrefab;
        [SerializeField] private Dep dep;

        // public ISpawnManager SpawnManager { get; private set; }

        // protected override void OnServerStart()
        // {
        //     Debug.Log("<color=red>OnStartServer called on the server.</color>");
        //     Debug.LogWarning("It's server and client");
        //     PlayerCharacter hostPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //     NetworkServer.AddPlayerForConnection(connectionToClient, hostPlayer.gameObject);
        // }
        //
        // public override void OnStartClient()
        // {
        //     base.OnStartClient();
        //     Debug.Log("<color=red>OnStartClient called on the client.</color>");
        //
        //
        //     Debug.LogWarning("It's client only");
        //     PlayerCharacter clientPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //     NetworkServer.AddPlayerForConnection(connectionToClient, clientPlayer.gameObject);
        // }

        protected override void OnServerStart()
        {
            
        }

        protected override void LoadDependencies()
        {
            Debug.LogWarning("<color=red>InitDependencies in GameManager</color>");
            // SpawnManager = dep.GetDependency<ISpawnManager>();
        }
    }
}
