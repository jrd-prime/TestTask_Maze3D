﻿using System;
using Game.Scripts.Client.Input;
using Mirror;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Scope
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;


        protected override void Configure(IContainerBuilder builder)
        {
            Debug.LogWarning("RootScope configured.");
            base.Configure(builder);

            if (networkManager == null) throw new NullReferenceException($"NetworkManager is null. {name}");

            builder.RegisterComponent(networkManager).AsSelf();

            var input = gameObject.AddComponent(typeof(PCUserInput));
            builder.RegisterComponent(input).As<IUserInput>();
        }
    }
}
