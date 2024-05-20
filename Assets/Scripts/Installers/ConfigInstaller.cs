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

        public override void InstallBindings()
        {
            Container.BindInstance(boxPrefabsConfig);
        }
    }
}