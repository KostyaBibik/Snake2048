using System;
using Components.Boosts.Impl;
using Enums;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(BoostPrefabsConfig),
        fileName = nameof(BoostPrefabsConfig))]
    public class BoostPrefabsConfig : ScriptableObject
    {
        [Space] [SerializeField] private BoostPrefabVo[] prefabs;
        
        public BoostPrefabVo GetPrefab(EBoxBoost type)
        {
            foreach (var prefab in prefabs)
            {
                if (prefab.type == type)
                    return prefab;
            }

            throw new Exception($"{nameof(BoxPrefabsConfig)} Can't find prefab with type: {type}");
        }
    }

    [Serializable]
    public struct BoostPrefabVo
    {
        public EBoxBoost type;
        public BoostView prefab;
    } 
}