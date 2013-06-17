using System;
using IrrKlang;
using System.IO;
using Strategy.Sound;
using System.Collections.Generic;

namespace Strategy {
	/// <summary>
	/// Derives from ISoundStopEventReceiver to make infinite loop of playing songs from given file.
	/// Implements IGameMusicPlayer and IEffectPlayer to play songs and effects.
	/// </summary>
	public class SoundPlayer :ISoundStopEventReceiver , IGameMusicPlayer, IEffectPlayer {
		
		protected ISoundEngine engine; // Main object of this music player
		protected const string songPath = "../../media/music"; // Path to folder witch songs (sounds) --mp3,ogg,wav
		protected const string effectPath = "../../media/music/effects"; // Path to folder witch effects
		protected const float volumeJump = 0.1f; // Difference to volume up/down

		protected int current = 0; // Number of actual song
		protected string[] songs; // Names of each song from folder
		protected Dictionary<string,string> effects; // Names of each sound from folder
		protected ISound sound; // Current playing song
		protected Mogre.RenderWindow mWindow; // RenderWindow instance for making overlays
		protected float mTimer; // Float as timer to determine of duration overlay
		protected Random r; // Makes random number to Select song 

		/// <summary>
		/// Initializes music player and randomizer. After that plays first song
		/// from file and sets event receiver.
		/// </summary>
		/// <param name="mWindow">The RenderWindow instance for making overlays</param>
		public SoundPlayer( Mogre.RenderWindow mWindow) {	
			engine = new ISoundEngine();
		    songs = Directory.GetFiles(songPath);
			r = new Random();
			this.mWindow = mWindow;
			sound = engine.Play2D(songs[0]);
			sound.setSoundStopEventReceiver(this);
            ShowCurrentPlaying(songs[current]);

			engine.SoundVolume = 0; //TODO: DELETE

			effects = new Dictionary<string, string>();
			var tempEff = Directory.GetFiles(effectPath);
			foreach (var effName in tempEff) {
				var splited = effName.Split('\\');
				effects.Add(splited[splited.Length - 1], effName);
			}
		}

		#region Public

		/// <summary>
		/// Shows the name of the currently playing song.
		/// </summary>
		public void ShowCurrentlyPlayingSong() {
			ShowCurrentPlaying(songs[current]);
		}
		/// <summary>
		/// Checks if the MusicBox overlay is showed and decreases the duration of display.
		/// If the duration indicator drops bellow zero so hides MusicBox overlay.
		/// </summary>
		/// <param Name="f">The delay between last two frames.</param>
		public void Update(float f) {
			if (mTimer > 0) {
				mTimer -= f;
				if (mTimer <= 0) {
					Mogre.OverlayManager.Singleton.GetByName("MusicBox").Hide();
				}
			}
		}

		/// <summary>
		/// Stops currently playing song and ISoundStopEventReceiver plays the next one.
		/// </summary>
		public void PlayNextSong() {
			engine.StopAllSounds();
		}

		/// <summary>
		/// Plays or pauses ISoundEngine engine.
		/// </summary>
		public void Pause() {
			if (sound.Paused) {
				sound.Paused = false;
			} else {
				sound.Paused = true;
			}
		}

		/// <summary>
		/// Increases ISoundEngine engine volume.
		/// </summary>
		public void VolumeUp() {
			engine.SoundVolume = ChangeVolume(volumeJump);
		}

		/// <summary>
		/// Decreases ISoundEngine engine volume.
		/// </summary>
		public void VolumeDown() {
			engine.SoundVolume = ChangeVolume(-volumeJump);
		}

		/// <summary>
		/// Plays the next song.
		/// </summary>
		/// <param Name="sound">The ISound which has been stopped.</param>
		/// <param Name="reason">The reason why the sound stop event was fired.</param>
		/// <param Name="userData">UserData pointer sets by the user when registering the interface.</param>
		public void OnSoundStopped(ISound sound, StopEventCause reason, object userData) {
			PlayMusic();
		}

		/// <summary>
		/// Plays the effect from the file with game effects by the given name.
		/// </summary>
		/// <param name="name">The name of the effect.</param>
		public void PlayEffect(string name) {

			// Controls if sound is in media/music/effect
			if (effects.ContainsKey(name)) {
				sound = engine.Play2D(effects[name]);
			} else {
				Console.WriteLine("Sound " + name + " missing");
			}
		}

		#endregion

		/// <summary>
		/// Gets random number which is different than number of current playing song and play the new song.
		/// After that shows the name of the new song and sets the SoundStopEventReceiver.
		/// </summary>
		private void PlayMusic() { 
			int i;
			while (current == (i = r.Next(songs.Length))) { }
			current = i;
			sound = engine.Play2D(songs[current]);
			ShowCurrentPlaying(songs[current]);
			sound.setSoundStopEventReceiver(this);	
		}	

		/// <summary>
		/// Shows MusicBox overlay with currently playing song.
		/// </summary>
		/// <param name="s">The name of the playing song.</param>
		private void ShowCurrentPlaying(string s) {

			var messageBox = Mogre.OverlayManager.Singleton.GetOverlayElement("MusicBox/MessageBox");
			messageBox.Left =(mWindow.Width - messageBox.Width) / 2;

			var messageBody = Mogre.OverlayManager.Singleton.GetOverlayElement("MusicBox/MessageBox/Body");
			messageBody.Left = messageBox.Width / 2;
			messageBody.Caption = "Actual song:\n"+s.Substring(6);
			mTimer = 1;
			Mogre.OverlayManager.Singleton.GetByName("MusicBox").Show();
		}

		/// <summary>
		/// Pauses the music player (engine).
		/// </summary>
		private void muteSound() {
			engine.SetAllSoundsPaused(true);
		}

		/// <summary>
		/// Continues the music player (engine).
		/// </summary>
		private void continueSound() {
			engine.SetAllSoundsPaused(false);
		}

		

		/// <summary>
		/// Changes the value of the music playr volume by a given number. Contols max and min bounds
		/// of the volume (0-1).
		/// </summary>
		/// <param Name="f">The value is the volume change.</param>
		/// <returns>The new value of volume from interval [0-1].</returns>
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
	}
}
