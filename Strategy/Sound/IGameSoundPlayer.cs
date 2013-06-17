
namespace Strategy.Sound {
	/// <summary>
	/// Represents the music player for play music during the game.
	/// </summary>
	public interface IGameMusicPlayer {

		/// <summary>
		/// Shows the name of the currently played song.
		/// </summary>
		void ShowCurrentlyPlayingSong();

		/// <summary>
		/// Stops the currently played song and playes the next.
		/// </summary>
		void PlayNextSong();

		/// <summary>
		/// Pauses all currently played songs.
		/// </summary>
		void Pause();

		/// <summary>
		/// Increases the music player volume.
		/// </summary>
		void VolumeUp();

		/// <summary>
		/// Decreases the music player volume.
		/// </summary>
		void VolumeDown();

		/// <summary>
		/// Updates the music player.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		void Update(float delay);
	}
}
