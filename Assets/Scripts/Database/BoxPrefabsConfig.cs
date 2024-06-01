using System;
using Components.Boxes.Views.Impl;
using Enums;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(BoxPrefabsConfig),
        fileName = nameof(BoxPrefabsConfig))]
    public class BoxPrefabsConfig : ScriptableObject
    {
        [Space] [SerializeField] private BoxPrefabVo[] prefabs;
        
        public BoxPrefabVo GetPrefab(EBoxGrade grade)
        {
            foreach (var prefab in prefabs)
            {
                if (prefab.grade == grade)
                    return prefab;
            }

            throw new Exception($"{nameof(BoxPrefabsConfig)} Can't find prefab with type: {grade}");
        }
    }

    [Serializable]
    public struct BoxPrefabVo
    {
        public EBoxGrade grade;
        public BoxView view;
    } 
}