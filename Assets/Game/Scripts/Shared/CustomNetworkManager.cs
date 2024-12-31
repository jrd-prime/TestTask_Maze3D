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

        public override void OnStartHost()
        {
            base.OnStartHost();
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
            Debug.Log("Client started. Number of players: ");
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
