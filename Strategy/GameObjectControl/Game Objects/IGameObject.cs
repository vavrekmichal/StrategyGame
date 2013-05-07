using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects {
	public interface IGameObject {

		/// <summary>
		/// Function inicializes or destroy SceneNode. It means hide or show object.
		/// </summary>
		/// <param Name="visible">Object shoud be visible (true) or hide (false)</param>
		void ChangeVisible(bool visible);

		/// <summary>
		/// DieHandler is called when any GameObject has less hp then 0
		/// </summary>
		DieEventHandler DieHandler { get; set; }

		string Name { get; }
		Team Team { get; set; }
		int Hp { get; }
		float PickUpDistance { get; }
		float OccupyDistance { get; }
		int OccupyTime { get; }
		Mogre.Vector3 Position { get; }

		/// <summary>
		/// AddProperty inserts given Property in GameObject's container.
		/// </summary>
		/// <typeparam Name="T">Type of inserted Property</typeparam>
		/// <param Name="Name">Property's Name is in this case a PropertyEnum. It is used
		/// for quicker access to specific Property.</param>
		/// <param Name="property">Property</param>
		void AddProperty<T>(PropertyEnum name, Property<T> property);

		/// <summary>
		/// AddProperty inserts given Property in GameObject's container.
		/// </summary>
		/// <typeparam Name="T">Type of inserted Property</typeparam>
		/// <param Name="Name">Property's Name is in this case a string. It is usually used
		/// for user-defined Property.</param>
		/// <param Name="property">Property</param>
		void AddProperty<T>(string name, Property<T> property);

		/// <summary>
		/// Function removes Property from GameObject's container by Name. 
		/// </summary>
		/// <param Name="Name">PropertyEnum with Name of removing Property</param>
		void RemoveProperty(PropertyEnum name);

		/// <summary>
		/// Function removes Property from GameObject's container by Name. 
		/// </summary>
		/// <param Name="Name">String with Name of removing Property</param>
		void RemoveProperty(string name);

		/// <summary>
		/// Destroy is called when a GameObject is removed from the game, so the function
		/// must unregister all Mogre components.
		/// </summary>
		void Destroy();

		/// <summary>
		/// Function subtracts damaged hp and checks if object is alive (hp>0). When
		/// object is "dead" then DieHandler is executed.
		/// </summary>
		/// <param Name="damage">Received damage</param>
		void TakeDamage(int damage);

		/// <summary>
		/// Return Dictionary with all object Property(ies).
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> GetPropertyToDisplay();

		/// <summary>
		/// Function is reaction on some stimul (ActionReason)
		/// </summary>
		/// <param Name="reason">Action stimul</param>
		/// <param Name="target">Object that called the event</param>
		/// <returns>Return reaction</returns>
		ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target);
	}
}
