using UnityEditor;
using UnityEngine;

namespace USDT.CustomEditor.CompileSound {
    public class EnginePlayer : AbstractPlayer, IPlayer
    {
        private AudioSource _audioSource
        {
            get
            {
                if (GameObject.Find("tempPlay"))
                    return GameObject.Find("tempPlay").GetComponent<AudioSource>();
                else
                    return new GameObject("tempPlay").AddComponent<AudioSource>();
            }
        }

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
            EditorApplication.update -= EditorUpdateTick;
            scheduledTime = (float)EditorApplication.timeSinceStartup + clip.length;
            EditorApplication.update += EditorUpdateTick;
        }

        public override void CleanUp()
        {
            MonoBehaviour.DestroyImmediate(_audioSource.gameObject);
            EditorApplication.update -= EditorUpdateTick;
            scheduledTime = 0f;
        }
    }

}