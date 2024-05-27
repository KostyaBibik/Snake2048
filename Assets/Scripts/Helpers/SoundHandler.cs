using UnityEngine;

namespace Helpers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundHandler : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        private void Awake()
        {
            transform.parent = null;
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayClip(AudioClip audioClip)
        {
            if(_audioSource == null)
                return;
            
            _audioSource.PlayOneShot(audioClip);
        }
    }
}