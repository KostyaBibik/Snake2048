using Systems.Action;
using Systems.Initializable;
using Systems.Runtime;
using Helpers;
using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameSceneHandler sceneHandler;
        [SerializeField] private GameInputManager playerInput;
        
        public override void InstallBindings()
        {
            BindSignals();
            BindInputSystems();
            BindSceneComponents();
            BindFactories();
            InstallSystems();
            BindServices();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<PlayerSpawnSignal>();
            Container.DeclareSignal<EatBoxSignal>();
            Container.DeclareSignal<MergeBoxSignal>();
        }

        private void BindInputSystems()
        {
            Container.Bind<GameInputManager>().FromInstance(playerInput).AsSingle();
        }

        private void BindSceneComponents()
        {
            Container.Bind<GameSceneHandler>().FromInstance(sceneHandler).AsTransient();
        }

        private void BindFactories()
        {
            Container.Bind<BoxEntityFactory>().AsSingle().NonLazy();
            Container.Bind<BoxStateFactory>().AsSingle().NonLazy();
        }

        private void InstallSystems()
        {
            Container.BindInterfacesAndSelfTo<SpawnBoxesSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerSpawnSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameInitializeSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EatBoxSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MergeBoxSystem>().AsSingle().NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<BoxService>().AsSingle();
        }
    }
}