using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.Sound {
	interface IGameSoundMakerPlayer {
		void PlayMusic();
		void ActualPlaying();
		void NowPlaying(string s);
		void StopActualSong();
		void Pause();

		// Volume 
		void VolumeUp();
		void VolumeDown();

		// Overlay
		void HideBox(float f);
	}
}
