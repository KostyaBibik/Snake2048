using Components.Boosts.Impl;
using Database;
using Enums;
using Installers;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Infrastructure.Factories.Impl
{
    public class BoostFactory : IEntityFactory<EBoxBoost, BoostView>
    {
        private readonly BoostPrefabsConfig _boostPrefabsConfig;
        
        public BoostFactory(BoostPrefabsConfig boostPrefabsConfig)
        {
            _boostPrefabsConfig = boostPrefabsConfig;
        }
        
        public BoostView Create(EBoxBoost param)
        {
            var prefab = _boostPrefabsConfig.GetPrefab(param).prefab;
            var instance = DiContainerRef.Container.InstantiatePrefabForComponent<BoostView>(prefab);
            var instanceTransform = instance.transform;

            instanceTransform.position = Vector3.zero;
            instanceTransform.rotation = Quaternion.identity;
            
            return instance;
        }
    }
}