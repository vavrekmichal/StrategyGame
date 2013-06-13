using System.Collections.Generic;
using Strategy.FightMgr;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects {
	/// <summary>
	/// Represents common interface for IMovableGameObject and for IStaticGameObject.
	/// Specifies the basic properties and fucntions of game object.
	/// 
	/// </summary>
	public interface IGameObject {

		/// <summary>
		/// Creates or destroy a SceneNode. It means hide or show object.
		/// </summary>
		/// <param name="visible">Object shoud be visible (true) or hidden (false).</param>
		void ChangeVisible(bool visible);

		/// <summary>
		/// Is called when any IGameObject has less health then 0.
		/// </summary>
		DieEventHandler DieHandler { get; set; }

		/// <summary>
		/// Starts attacking and stores reference to the Fight.
		/// </summary>
		/// <param name="fight">The Fight which leads attack.</param>
		void StartAttack(Fight fight);

		/// <summary>
		/// Ends attacking.
		/// </summary>
		void StopAttack();

		/// <summary>
		/// Returns unique object name. The name is used for name of a creating
		/// SceneNode and Entity (that is the reason why it must be unique).
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets or sets object Team. The team is indicator of a friendships and production.
		/// Also can be controled if it is player object by team (team name is also unique).
		/// </summary>
		Team Team { get; set; }

		/// <summary>
		/// Returns the current state of health. The hp indicates if the object is alive (more than 0).
		/// </summary>
		int Hp { get; }

		/// <summary>
		/// Returns the distance where that object may contact the unit from its team.
		/// </summary>
		int ShoutDistance { get; }

		/// <summary>
		/// Returns the distance which is needed to reach this object.
		/// </summary>
		float PickUpDistance { get; }

		/// <summary>
		/// Returns the distance which is needed to occupy this object. Should be bigger than PickUpDistance.
		/// </summary>
		float OccupyDistance { get; }

		/// <summary>
		/// Returns the time which is needed to successful occupation.
		/// </summary>
		int OccupyTime { get; }

		/// <summary>
		/// Returns the deffence power of the object.
		/// </summary>
		int DeffPower { get; }

		/// <summary>
		/// Returns Vector3 with a current object position.
		/// </summary>
		Mogre.Vector3 Position { get; }

		/// <summary>
		/// Returns Property by given name (base properties)
		/// </summary>
		/// <typeparam name="T">The typeof the Property.</typeparam>
		/// <param name="propertyName">The name of the Property from Enum (base properties).</param>
		/// <returns>Returns founded Property.</returns>
		Property<T> GetProperty<T>(PropertyEnum propertyName);

		/// <summary>
		/// Returns Property with given name (user defined properties)
		/// </summary>
		/// <typeparam name="T">The typeof the Property.</typeparam>
		/// <param name="propertyName">The name of the Property (user defined properties).</param>
		/// <returns>Returns founded Property.</returns>
		Property<T> GetProperty<T>(string propertyName);

		/// <summary>
		/// Inserts given Property in IGameObject Property container.
		/// </summary>
		/// <typeparam name="T">The type of the inserted Property.</typeparam>
		/// <param name="name">The Property name is in this case a PropertyEnum. It is used
		/// for quicker access to specific Property.</param>
		/// <param name="property">The inserting Property.</param>
		void AddProperty<T>(PropertyEnum name, Property<T> property);

		/// <summary>
		/// Inserts given Property in IGameObject Property container.
		/// </summary>
		/// <typeparam Name="T">The type of the inserted Property.</typeparam>
		/// <param Name="Name">The Property name is in this case a string. It is usually used
		/// for user-defined Property.</param>
		/// <param Name="property">The inserting Property.</param>
		void AddProperty<T>(string name, Property<T> property);

		/// <summary>
		/// Removes the Property from IGameObject container by name. 
		/// </summary>
		/// <param name="name">The PropertyEnum member with a name of the removing Property.</param>
		void RemoveProperty(PropertyEnum name);

		/// <summary>
		/// Removes the Property from IGameObject container by name. 
		/// </summary>
		/// <param name="name">The string with a name of the removing Property.</param>
		void RemoveProperty(string name);

		/// <summary>
		/// Unregisters all Mogre components (SceneNode, Entity,...).
		/// </summary>
		void Destroy();

		/// <summary>
		/// Subtracts given damage from current hp and checks if object is alive (hp > 0). When
		/// object is "dead" then DieHandler is executed.
		/// </summary>
		/// <param name="damage">The received damage.</param>
		void TakeDamage(int damage);

		/// <summary>
		/// Returns Dictionary with all object Properties.
		/// </summary>
		/// <returns>Returns the dictionary with Properties as objects (runtime generic)</returns>
		Dictionary<string, object> GetPropertyToDisplay();

		/// <summary>
		/// Reacts on the ActionReason. Object can reacts itself or answers required action.
		/// </summary>
		/// <param name="reason">The reason of calling this function.</param>
		/// <param name="target">The target which invoke this calling.</param>
		/// <returns>The answer on the ActionReason.</returns>
		ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target);

		/// <summary>
		/// Sets all IGameObjects in ShoutDistance around this object (recursively) which are in same Team to given list.
		/// </summary>
		/// <param name="objectsInDistance">The list in whitch will be inserted objects in ShoutDistance.</param>
		void Shout(List<IGameObject> objectsInDistance);


		/// <summary>
		/// Inserts given game action to object IGameAction container. The object will update all his IGameActions.
		/// </summary>
		/// <param name="gameAction">The inserting game action.</param>
		void AddIGameAction(IGameAction gameAction);

		/// <summary>
		/// Returns list with all object game actions.
		/// </summary>
		/// <returns>Returns list with all object game actions.</returns>
		List<IGameAction> GetIGameActions();
	}
}
