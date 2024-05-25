using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(BoxPoolConfig),
        fileName = nameof(BoxPoolConfig))]
    public class BoxPoolConfig : ScriptableObject
    {
        public List<BoxPoolOption> config;
    }
    
    [Serializable]
    public struct BoxPoolOption
    {
        public EBoxGrade grade;
        public int maxCount;
        public int initialCount;
    } 
}