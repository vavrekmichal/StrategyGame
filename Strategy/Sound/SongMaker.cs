using System;
using IrrKlang;
using System.IO;
using System.Timers;
using Strategy.Sound;

namespace Strategy {
	/// <summary>
	/// The SoundMaker class derives from ISoundStopEventReceiver to make infinite loop of playing
	/// songs from given file.
	/// </summary>
	class SoundMaker :ISoundStopEventReceiver , IGameSoundMaker {
		
		protected ISoundEngine engine; // Main thing of this music player
		protected string path; // Path to folder witch songs (sounds) --mp3,ogg,wav
		protected bool paused = false; // State of music player
		protected int actual = 0; // Number of actual song (sound)
		protected float volumeJump = 0.1f; // Difference to volume up/down
		protected string[] fileNames; // Names of each song (sound) from given folder
		protected ISound sound; // Actual playing song (sound)
		protected Mogre.RenderWindow mWindow; // RenderWindow instance for making overlays
		protected float mTimer; // Float as timer to determine of duration overlay
		protected Random r; // Makes random number to select song (sound)

		/// <summary>
		/// A constructor inicializes music player and random. After that play first song
		/// (sound) and set event receiver
		/// </summary>
		/// <param name="path">Path to folder with music</param>
		/// <param name="w">RenderWindow instance for making overlays</param>
		public SoundMaker(string path, Mogre.RenderWindow mWindow) {
			this.path = path;
			engine = new ISoundEngine();
		    fileNames = Directory.GetFiles(path);
			r = new Random();
			this.mWindow = mWindow;
			sound = engine.Play2D(fileNames[0]);
			sound.setSoundStopEventReceiver(this);
            nowPlaying(fileNames[actual]);
			engine.SoundVolume = 0; //TODO: DELETE
		}

		/// <summary>
		/// The method select random number which is different than actual and play it.
		/// After that call method for show overlay with playing song (sound) and set
		/// event receiver
		/// </summary>
		public void playMusic() { 
			int i;
			while (actual == (i = r.Next(fileNames.Length))) { }
			actual = i;
			sound = engine.Play2D(fileNames[actual]);
			nowPlaying(fileNames[actual]);
			sound.setSoundStopEventReceiver(this);	
		}

		/// <summary>
		/// Call method for show overlay with playing song (sound)
		/// </summary>
		public void actualPlaying(){
			nowPlaying(fileNames[actual]);
		}

		/// <summary>
		/// The method shows MusicBox overlay with actual playing song
		/// </summary>
		/// <param name="s">Name of playing song</param>
		public void nowPlaying(string s) {

			var messageBox = Mogre.OverlayManager.Singleton.GetOverlayElement("MusicBox/MessageBox");
			messageBox.Left =(mWindow.Width - messageBox.Width) / 2;

			var messageBody = Mogre.OverlayManager.Singleton.GetOverlayElement("MusicBox/MessageBox/Body");
			messageBody.Left = messageBox.Width / 2;
			messageBody.Caption = "Actual song:\n"+s.Substring(6);
			mTimer = 1;
			Mogre.OverlayManager.Singleton.GetByName("MusicBox").Show();
		}

		/// <summary>
		/// This method control mTimer (bigger 1 => MusicBox is showed) and if
		/// mTimer is lower 0 => hide overlay MusicBox
		/// </summary>
		/// <param name="f">Delay between last frames</param>
		public void hideBox(float f){
			if (mTimer > 0) {
				mTimer -= f;
				if (mTimer <= 0) {
					Mogre.OverlayManager.Singleton.GetByName("MusicBox").Hide();
				}
			}
		}

		/// <summary>
		/// The method stops actual song (sound) -> event OnSoundStopped() is called
		/// </summary>
		public void stopActualSong() {
			engine.StopAllSounds();
		}

		/// <summary>
		/// The method pauses ISoundEngine engine
		/// </summary>
		private void muteSound() {
			engine.SetAllSoundsPaused(true);
			paused = true;
		}

		/// <summary>
		/// The method play ISoundEngine engine
		/// </summary>
		private void continueSound() {
			engine.SetAllSoundsPaused(false);
			paused = false;
		}

		/// <summary>
		/// The method plays or pauses ISoundEngine engine
		/// </summary>
		public void pause() {
			if (paused) {
				continueSound();
			} else {
				muteSound();
			}
		}
		
		/// <summary>
		/// The method increase ISoundEngine engine's volume
		/// </summary>
		public void volumeUp(){
			engine.SoundVolume = changeVolume(volumeJump);
		}

		/// <summary>
		/// The method decrease ISoundEngine engine's volume
		/// </summary>
		public void volumeDown(){
			engine.SoundVolume = changeVolume(-volumeJump);
		}

		/// <summary>
		/// The method control music player volume.
		/// </summary>
		/// <param name="f">Value to in(de)crease volume</param>
		/// <returns>New value of volume (1 is max, 0 is min)</returns>
		private float changeVolume(float f){
			float help = engine.SoundVolume + f;
			if (help<=0) {
				return 0;
			}
			if (help >= 1) {
				return 1;
			}
			return help;
		}

		/// <summary>
		/// This method is called when the song stops.
		/// </summary>
		/// <param name="sound">Sound which has been stopped</param>
		/// <param name="reason">The reason why the sound stop event was fired</param>
		/// <param name="userData">UserData pointer set by the user when registering the interface</param>
		public void OnSoundStopped(ISound sound, StopEventCause reason, object userData) {
			playMusic();
		}

	}
}
