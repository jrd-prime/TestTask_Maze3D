using System;
using Game.Scripts.Client.Input;
using Game.Scripts.Manager;
using Game.Scripts.Shared;
using Mirror;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Scope
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CoinsManager coinsManager;


        protected override void Configure(IContainerBuilder builder)
        {
            Debug.LogWarning("RootScope configured.");
            base.Configure(builder);

            if (networkManager == null) throw new NullReferenceException($"NetworkManager is null. {name}");
            if (gameManager == null) throw new NullReferenceException($"GameManager is null. {name}");
            if (coinsManager == null) throw new NullReferenceException($"CoinsManager is null. {name}");

            builder.RegisterComponent(networkManager).AsSelf();
            builder.RegisterComponent(gameManager).As<IGameManager>();
            builder.RegisterComponent(coinsManager).As<ICoinsManager>();

            var input = gameObject.AddComponent(typeof(PCUserInput));
            builder.RegisterComponent(input).As<IUserInput>();
        }
    }
}
