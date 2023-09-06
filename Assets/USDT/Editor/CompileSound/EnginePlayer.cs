using System.Collections;
using UnityEditor;
using UnityEngine;
using USDT.Components;

namespace USDT.CustomEditor.CompileSound {
    public class EnginePlayer : AbstractPlayer, IPlayer
    {
        private AudioSource _audioSource => InEditModeGameObjectHandler.Instance.GetComp<AudioSource>();

        public void Play()
        {
            if (_audioSource.isPlaying)
                return;
            _audioSource.clip = SoundLibrary.GetSoundClip();
            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void CompileFinished()
        {
            _audioSource.Stop();
            AudioClip clip = Resources.Load<AudioClip>(SoundLibrary.ResourcesDing);
            _audioSource.PlayOneShot(clip);
        }
    }

}