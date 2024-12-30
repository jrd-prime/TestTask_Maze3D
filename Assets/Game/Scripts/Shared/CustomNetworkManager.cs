using System;
using Game.Scripts.Help;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Dep dep;

        public ISpawnManager SpawnManager { get; private set; }

        public override void OnStartHost()
        {
            base.OnStartHost();
            SpawnManager = dep.GetDependency<ISpawnManager>();
            Debug.Log("Host started.");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("Server started.");
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log("Client started. Number of players: " + numPlayers);
            if (numPlayers == 0)
            {
                Debug.Log("Spawning player 0.");
                var pp = SpawnManager.GetMainSpawnPoint();
                var p = Instantiate(playerPrefab, pp.position, Quaternion.identity);

                NetworkClient.Ready();
                NetworkServer.AddPlayerForConnection(NetworkServer.localConnection, p);
            }
            else
            {
                Debug.Log("Spawning player " + numPlayers);
                var pp = SpawnManager.GetRandomSpawnPoint();
                var p = Instantiate(playerPrefab, pp.position, Quaternion.identity);

                NetworkClient.Ready();
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[1], p);
            }
        }


        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("<color=green>Client connected.</color>");
        }

        // Обработка отключения клиента (если нужно)
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("<color=red>Client disconnected.</color>");
        }
    }
}
