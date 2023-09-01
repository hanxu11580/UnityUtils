using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Media;
using System;
using UnityEditor;
namespace USDT.CustomEditor.CompileSound {
    public class NativePlayer : AbstractPlayer, IPlayer
    {
        private static Thread _thread;
        private static SoundPlayer _player;
        public void Play()
        {
            var path = SoundLibrary.GetSoundName();
            Thread thread = new Thread(new ParameterizedThreadStart(PlaySound));
            thread.Start(path);
        }

        public void PlaySound(object path)
        {
            if(path is string strPath) {
                using (var player = new SoundPlayer(strPath)) {
                    player.PlaySync();
                }
                _thread.Start();
            }
        }

        public void CompileFinished() {
            _player = new SoundPlayer(string.Format("{0}/{1}/ding.wav",Environment.CurrentDirectory,SoundLibrary.DingFolder));
            _player.Play();
            _player.StreamChanged += _player_StreamChanged;


        }

        private void _player_StreamChanged(object sender, EventArgs e)
        {
            _player.StreamChanged -= _player_StreamChanged;
            scheduledTime = (float)EditorApplication.timeSinceStartup + _player.Stream.Length;
        }

        public override void CleanUp()
        {
            _player.Dispose();
            scheduledTime = 0f;
        }
    }

}