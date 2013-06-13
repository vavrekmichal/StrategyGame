using System;
using IrrKlang;
using System.IO;
using System.Timers;
using Strategy.Sound;
using System.Collections.Generic;

namespace Strategy {
	/// <summary>
	/// The SoundMaker class derives from ISoundStopEventReceiver to make infinite loop of playing
	/// songs from given file.
	/// </summary>
	public class SoundPlayer :ISoundStopEventReceiver , IGameSoundMakerPlayer, IEffectPlayer {
		
		protected ISoundEngine engine; // Main thing of this music player
		protected const string songPath = "../../media/music"; // Path to folder witch songs (sounds) --mp3,ogg,wav
		protected const string effectPath = "../../media/music/effects"; // Path to folder witch effects

		protected int actual = 0; // Number of actual song (sound)
		protected float volumeJump = 0.1f; // Difference to volume up/down
		protected string[] songs; // Names of each song (sound) from folder
		protected Dictionary<string,string> effects; // Names of each song (effect) from folder
		protected ISound sound; // Actual playing song (sound)
		protected Mogre.RenderWindow mWindow; // RenderWindow instance for making overlays
		protected float mTimer; // Float as timer to determine of duration overlay
		protected Random r; // Makes random number to Select song (sound)

		/// <summary>
		/// A constructor inicializes music player and randomizator. After that play first song
		/// (sound) and Set event receiver
		/// </summary>
		/// <param Name="mWindow">RenderWindow instance for making overlays</param>
		public SoundPlayer( Mogre.RenderWindow mWindow) {	
			engine = new ISoundEngine();
		    songs = Directory.GetFiles(songPath);
			r = new Random();
			this.mWindow = mWindow;
			sound = engine.Play2D(songs[0]);
			sound.setSoundStopEventReceiver(this);
            NowPlaying(songs[actual]);
			engine.SoundVolume = 0; //TODO: DELETE
			effects = new Dictionary<string, string>();
			var tempEff = Directory.GetFiles(effectPath);
			foreach (var effName in tempEff) {
				var splited = effName.Split('\\');
				effects.Add(splited[splited.Length - 1], effName);
			}
		}

		/// <summary>
		/// The method Select random number which is different than actual and play it.
		/// After that call method for show overlay with playing song (sound) and Set
		/// event receiver
		/// </summary>
		public void PlayMusic() { 
			int i;
			while (actual == (i = r.Next(songs.Length))) { }
			actual = i;
			sound = engine.Play2D(songs[actual]);
			NowPlaying(songs[actual]);
			sound.setSoundStopEventReceiver(this);	
		}

		/// <summary>
		/// Call method for show overlay with playing song (sound)
		/// </summary>
		public void ActualPlaying(){
			NowPlaying(songs[actual]);
		}

		/// <summary>
		/// The method shows MusicBox overlay with actual playing song
		/// </summary>
		/// <param Name="s">Name of playing song</param>
		public void NowPlaying(string s) {

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
		/// <param Name="f">Delay between last frames</param>
		public void HideBox(float f){
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
		public void StopActualSong() {
			engine.StopAllSounds();
		}

		/// <summary>
		/// The method pauses ISoundEngine engine
		/// </summary>
		private void muteSound() {
			engine.SetAllSoundsPaused(true);
			//paused = true;
		}

		/// <summary>
		/// The method play ISoundEngine engine
		/// </summary>
		private void continueSound() {
			engine.SetAllSoundsPaused(false);
			//paused = false;
		}

		/// <summary>
		/// The method plays or pauses ISoundEngine engine
		/// </summary>
		public void Pause() {
			if (sound.Paused) {
				sound.Paused = false;
			} else {
				sound.Paused = true;
			}
		}
		
		/// <summary>
		/// The method increase ISoundEngine engine volume
		/// </summary>
		public void VolumeUp(){
			engine.SoundVolume = ChangeVolume(volumeJump);
		}

		/// <summary>
		/// The method decrease ISoundEngine engine volume
		/// </summary>
		public void VolumeDown(){
			engine.SoundVolume = ChangeVolume(-volumeJump);
		}

		/// <summary>
		/// The method control music player volume.
		/// </summary>
		/// <param Name="f">Value to in(de)crease volume</param>
		/// <returns>New value of volume (1 is max, 0 is min)</returns>
		private float ChangeVolume(float f){
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
		/// <param Name="sound">Sound which has been stopped</param>
		/// <param Name="reason">The reason why the sound stop event was fired</param>
		/// <param Name="userData">UserData pointer Set by the user when registering the interface</param>
		public void OnSoundStopped(ISound sound, StopEventCause reason, object userData) {
			PlayMusic();
		}

		/// <summary>
		/// The function play 2D effect from media/music/effects
		/// </summary>
		/// <param Name="Name">Name of effect</param>
		public void PlayEffect(string name) {

			// Controls if sound is in media/music/effect
			if (effects.ContainsKey(name)) {
				sound = engine.Play2D(effects[name]);
			} else {
				Console.WriteLine("Sound " + name + " missing");
			}
		}
	}
}
