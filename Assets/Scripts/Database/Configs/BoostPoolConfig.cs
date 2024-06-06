using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(BoostPoolConfig),
        fileName = nameof(BoostPoolConfig))]
    public class BoostPoolConfig : ScriptableObject
    {
        public List<BoostPoolOption> config;
    }
    
    [Serializable]
    public struct BoostPoolOption
    {
        public EBoxBoost grade;
        public int maxCount;
        public int initialCount;
    } 
}