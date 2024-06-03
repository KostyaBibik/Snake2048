using UnityEngine;

namespace Helpers
{
    public class SoundHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource BgSource;
        [SerializeField] private AudioSource SoundEffectsSource;
        
        private void Awake()
        {
            transform.parent = null;
        }

        public void PlayEffectClip(AudioClip audioClip)
        {
            if(SoundEffectsSource == null)
                return;
            
            SoundEffectsSource.PlayOneShot(audioClip);
        }
    }
}