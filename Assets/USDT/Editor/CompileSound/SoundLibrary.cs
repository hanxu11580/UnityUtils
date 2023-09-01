using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace USDT.CustomEditor.CompileSound {
    /// <summary>
    /// 不打乱，默认使用_soundNames第一个音效
    /// </summary>
    public static class SoundLibrary
    {
        private static List<AudioClip> _soundClips;
        private static string[] _soundNames;
        public const string BankLocation = "Assets/USDT/Editor/Resources/CompileSound/PlayList";
        public const string DingFolder = "Assets/Editor/Resources/CompileSound";
        public const string PlayListResourcesFolder = "CompileSound/PlayList";
        public const string ResourcesDing = "CompileSound/ding";
        static SoundLibrary()
        {
            //For Editor mode
            AudioClip[] clips = Resources.LoadAll<AudioClip>(PlayListResourcesFolder);
            if (clips.Length > 0)
                _soundClips = clips.ToList();
            else
                throw new System.NullReferenceException("No sound file detected for Elevator Compiler");

            //For native mode
            if (!Directory.Exists(string.Format("{0}/{1}", System.Environment.CurrentDirectory, BankLocation)))
                throw new System.NullReferenceException("PlayList folder not found!", new System.Exception("Please make sure you have" + BankLocation));

            _soundNames = Directory.GetFiles(BankLocation, "*.wav");
            ThrowNoSoundException();
        }

        public static AudioClip GetSoundClip()
        {
            if (!CompileSoundSettings.Shuffle)
            {
                ThrowNoSoundException();
                return _soundClips[0];
            }
            return _soundClips[Random.Range(0, _soundClips.Count)];
        }

        public static string GetSoundName()
        {
            if (!CompileSoundSettings.Shuffle)
            {
                ThrowNoSoundException();
                return _soundNames[0];
            }
            System.Random rnd = new System.Random();
            return _soundNames[rnd.Next(0, _soundNames.Length)];
        }

        private static void ThrowNoSoundException() {
            if (_soundNames.Length == 0) {
                throw new System.NullReferenceException("No sound file detected for Elevator Compiler");
            }
        }
    }
}