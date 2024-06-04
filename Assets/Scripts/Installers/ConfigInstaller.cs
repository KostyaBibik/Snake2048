using Database;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = nameof(ConfigInstaller),
        menuName = "Installers/" + nameof(ConfigInstaller))]
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private BoxPrefabsConfig boxPrefabsConfig;
        [SerializeField] private GameSettingsConfig gameSettingsConfig;
        [SerializeField] private BoxPoolConfig boxPoolConfig;
        [SerializeField] private SoundsConfig soundsConfig;
        [SerializeField] private NicknamesConfig nicknamesConfig;
        [SerializeField] private BoostPrefabsConfig boostPrefabsConfig;
        [SerializeField] private BoostPoolConfig boostPoolConfig;
        [SerializeField] private BotsSettingsConfig botsSettingsConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(boxPrefabsConfig);
            Container.BindInstance(gameSettingsConfig);
            Container.BindInstance(boxPoolConfig);
            Container.BindInstance(soundsConfig);
            Container.BindInstance(nicknamesConfig);
            Container.BindInstance(boostPrefabsConfig);   
            Container.BindInstance(boostPoolConfig);
            Container.BindInstance(botsSettingsConfig);
        }
    }
}