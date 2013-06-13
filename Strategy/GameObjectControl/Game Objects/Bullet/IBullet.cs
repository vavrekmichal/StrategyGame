using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.Bullet {
	/// <summary>
	/// Represents bullet. Implements move in the space and when IBullet hits must report it to
	/// the appropriate Fight. 
	/// </summary>
	public interface IBullet {
		/// <summary>
		/// Returns name of the IBullet.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns power of the IBullet.
		/// </summary>
		int Attack { get; }

		/// <summary>
		/// Updates IBullet position in a visible SolarSystem.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		void Update(float delay);

		/// <summary>
		/// Updates IBullet position in an invisible SolarSystem.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		void HiddenUpdate(float delay);

		/// <summary>
		/// Changes IBullet's visibility (destroy Mogre SceneNode).
		/// </summary>
		/// <param name="visible"></param>
		void ChangeVisible(bool visible);

		/// <summary>
		/// Destroys IBullet.
		/// </summary>
		void Destroy();
	}
}
