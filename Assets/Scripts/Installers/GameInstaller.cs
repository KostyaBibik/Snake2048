using System;
using Systems.Action;
using Systems.Action.BoostSystems;
using Systems.Initializable;
using Systems.Runtime;
using Cinemachine;
using Helpers;
using Infrastructure.Factories.Impl;
using Infrastructure.Pools.Impl;
using Services;
using Services.Impl;
using Signals;
using UI.InitStages;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller, IDisposable
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
            Container.DeclareSignal<BoxBoostSignal>();
            Container.DeclareSignal<AddBoxToTeamSignal>();
            Container.DeclareSignal<RegisterTeamSignal>();
            Container.DeclareSignal<LeaderboardUpdateSignal>();
            Container.DeclareSignal<ChangeSoundSettingsSignal>();
            Container.DeclareSignal<KillTeamSignal>();
        }

        private void BindGameMatcher()
        {
            Container.BindInterfacesAndSelfTo<GameMatchService>().AsSingle().NonLazy();
        }

        private void BindCamera()
        {
            Container.Bind<Camera>().FromInstance(gameCamera).AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromInstance(virtualCamera).AsCached();
        }

        private void BindSounds()
        {
            Container.Bind<SoundHandler>().FromInstance(soundHandler).AsCached();
            Container.BindInterfacesAndSelfTo<PlaySoundsSystem>().AsCached().NonLazy();
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
            Container.Bind<BoxFactory>().AsCached().NonLazy();
            Container.Bind<BoostFactory>().AsCached().NonLazy();
            Container.Bind<BoxStateFactory>().AsCached();
        }

        private void InstallSystems()
        {
            Container.BindInterfacesAndSelfTo<CameraUpdateSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<SpawnBoxesSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<SpawnBoostsSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerSpawnSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<GameInitializeSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<EatBoxSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<BotSpawnSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<UpdateTimeSystem>().AsCached().NonLazy(); 
            Container.BindInterfacesAndSelfTo<SpeedBoostSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<AddGradesBoostSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<AccelerationBoxSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardHandleSystem>().AsCached().NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<BoxService>().AsCached();
            Container.BindInterfacesAndSelfTo<BotService>().AsCached();
            Container.BindInterfacesAndSelfTo<PlayerDataService>().AsCached();
            Container.BindInterfacesAndSelfTo<GameSettingsService>().AsCached();
        }

        private void BindUiInitStages()
        {
            Container.BindInterfacesAndSelfTo<InitTopWindowStage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<InitPauseWindowStage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<InitLoseWindowStage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<InitLeaderboardWindowStage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<InitStartGameWindowStage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<InitKillTeamWindowStage>().AsCached().NonLazy();
        }

        private void BindPools()
        {
            Container.BindInterfacesAndSelfTo<BoxPool>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<BoostPool>().AsCached().NonLazy();
        }

        public void Dispose()
        {
            Container.UnbindAll();
        }
    }
}