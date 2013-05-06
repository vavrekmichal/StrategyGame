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
		/// <param name="visible">Object shoud be visible (true) or hide (false)</param>
		void changeVisible(bool visible);

		string Name { get; }
		Team Team { get; set; }
		float PickUpDistance { get; }
		float OccupyDistance { get; }
		int OccupyTime { get; }
		Mogre.Vector3 Position { get; }

		void addProperty<T>(PropertyEnum name, Property<T> property);
		void addProperty<T>(string name, Property<T> property);
		void removeProperty(PropertyEnum name);
		void removeProperty(string name);

		/// <summary>
		/// Return Dictionary with all object Property(ies).
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> getPropertyToDisplay();

		/// <summary>
		/// Function is reaction on some stimul (ActionReason)
		/// </summary>
		/// <param name="reason">Action stimul</param>
		/// <param name="target">Object that called the event</param>
		/// <returns>Return reaction</returns>
		ActionReaction reactToInitiative(ActionReason reason, IMovableGameObject target);
	}
}
