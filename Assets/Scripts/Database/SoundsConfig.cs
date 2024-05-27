using System;
using Enums;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(SoundsConfig),
        fileName = nameof(SoundsConfig), order = 3)]
    public class SoundsConfig : ScriptableObject
    {
        [SerializeField] private SoundVo[] sounds;
        
        public AudioClip GetSound(ESoundType grade)
        {
            foreach (var soundVo in sounds)
            {
                if (soundVo.type == grade)
                    return soundVo.clip;
            }

            throw new Exception($"{nameof(SoundsConfig)} Can't find sound with type: {grade}");
        }
    }

    [Serializable]
    public struct SoundVo
    {
        public ESoundType type;
        public AudioClip clip;
    }
}