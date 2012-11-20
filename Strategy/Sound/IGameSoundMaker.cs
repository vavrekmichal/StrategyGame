using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.Sound {
	interface IGameSoundMaker {
		void playMusic();
		void actualPlaying();
		void nowPlaying(string s);
		void stopActualSong();
		void pause();
		//volume 
		void volumeUp();
		void volumeDown();

		//overlay
		void hideBox(float f);
	}
}
