using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.Sound {
	interface IGameSoundMakerPlayer {
		void playMusic();
		void actualPlaying();
		void nowPlaying(string s);
		void stopActualSong();
		void pause();

		// Volume 
		void volumeUp();
		void volumeDown();

		// Overlay
		void hideBox(float f);
	}
}
