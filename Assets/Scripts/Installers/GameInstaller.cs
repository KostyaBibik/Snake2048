using Systems.Action;
using Systems.Initializable;
using Systems.Runtime;
using Cinemachine;
using Helpers;
using Infrastructure.Factories.Impl;
using Services;
using Services.Impl;
using Signals;
using UI.InitStages;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameSceneHandler sceneHandler;
        [SerializeField] private GameInputManager playerInput;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private SoundHandler soundHandler;

        public override void InstallBindings()
        {
            BindSignals();
            BindGameMatcher();
            BindCamera();
            BindSounds();
            BindInputSystems();
            BindSceneComponents();
            BindFactories();
            BindPools();
            InstallSystems();
            BindUiInitStages();
            BindServices();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<PlayerSpawnSignal>();
            Container.DeclareSignal<EatBoxSignal>();
            Container.DeclareSignal<MergeBoxSignal>();
            Container.DeclareSignal<CameraUpdateSignal>();
            Container.DeclareSignal<PlaySoundSignal>();
            Container.DeclareSignal<ChangeGameModeSignal>();
        }

        private void BindGameMatcher()
        {
            Container.BindInterfacesAndSelfTo<GameMatchService>().AsSingle().NonLazy();
        }

        private void BindCamera()
        {
            Container.Bind<Camera>().FromInstance(gameCamera).AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromInstance(virtualCamera).AsSingle();
        }

        private void BindSounds()
        {
            Container.Bind<SoundHandler>().FromInstance(soundHandler).AsSingle();
            Container.BindInterfacesAndSelfTo<PlaySoundsSystem>().AsSingle().NonLazy();
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
            Container.Bind<BoxStateFactory>().AsSingle();
        }

        private void InstallSystems()
        {
            Container.BindInterfacesAndSelfTo<CameraUpdateSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SpawnBoxesSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerSpawnSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameInitializeSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EatBoxSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BotSpawnSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UpdateTimeSystem>().AsSingle().NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<BoxService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BotService>().AsSingle();
        }

        private void BindUiInitStages()
        {
            Container.BindInterfacesAndSelfTo<InitTopWindowStage>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InitPauseMenuStage>().AsSingle().NonLazy();
        }

        private void BindPools()
        {
            Container.BindInterfacesAndSelfTo<BoxPool>().AsSingle().NonLazy();
        }
    }
}