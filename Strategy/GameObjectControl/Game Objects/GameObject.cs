using System;
using System.Collections.Generic;
using Mogre;
using Strategy.Exceptions;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects {

	/// <summary>
	/// Delegates when object dies.
	/// </summary>
	/// <param name="igo">The death object.</param>
	/// <param name="m">The arguments of the death.</param>
	public delegate void DieEventHandler(IGameObject igo, MyDieArgs m);

	/// <summary>
	/// Implements all IGameObject functions. Designed to facilitate the implementation of game objects.
	/// </summary>
	public abstract class GameObject : IGameObject {


		protected Dictionary<PropertyEnum, object> propertyDict = new Dictionary<PropertyEnum, object>();
		protected Dictionary<string, object> userDefinedPropertyDict = new Dictionary<string, object>();

		protected List<IGameAction> gameActionList = new List<IGameAction>();

		protected Team team;

		// Unique name
		protected string name;
		protected bool isVisible;

		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected Property<Mogre.Vector3> position = new Property<Vector3>(Vector3.ZERO);

		protected bool attack;
		protected Strategy.FightMgr.Fight fight;

		private int hp = 100;

		#region Public
		/// <summary>
		/// Changes object visibility. 
		/// (true) Creates SceneNode and checks Entity (if is null -> initializes). Calls OnDisplayed.
		/// (false) Destroys SceneNode and stored actual position.
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && !this.isVisible) {

				// Controls if the entity is inicialized
				if (entity == null) {
					entity = Game.SceneManager.CreateEntity(name, mesh);
				}

				sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position.Value);
				sceneNode.AttachObject(entity);
				this.isVisible = true;
				OnDisplayed();
			} else {
				if (this.isVisible) {
					position.Value = sceneNode.Position;
					Game.SceneManager.DestroySceneNode(sceneNode);
					sceneNode = null;
					isVisible = false;
				}
			}
		}

		/// <summary>
		/// Destroys object Mogre components. Rest will be destroyd by Garbage Collector.
		/// </summary>
		public void Destroy() {
			if (sceneNode != null) {
				Game.SceneManager.DestroySceneNode(sceneNode);
				sceneNode = null;
			}
			if (entity != null) {
				Game.SceneManager.DestroyEntity(entity);
				entity = null;
			}
		}

		/// <summary>
		/// Returns object unique name.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Returns if the object is visible.
		/// </summary>
		public bool Visible {
			get { return isVisible; }
		}

		/// <summary>
		/// Gets or sets object team.
		/// </summary>
		public Team Team {
			get {
				return team;
			}
			set {
				team = value;
			}
		}

		/// <summary>
		/// Returns the distance which is needed to reach this object (200).
		/// </summary>
		public virtual float PickUpDistance {
			get { return 200; }
		}

		/// <summary>
		/// Returns the distance which is needed to occupy this object (220).
		/// </summary>
		public virtual float OccupyDistance {
			get { return 220; }
		}

		/// <summary>
		/// Returns the time which is needed to successful occupation (120 seconds).
		/// </summary>
		public virtual int OccupyTime {
			get { return 120; }
		}

		/// <summary>
		/// Returns Vector3 with a current object position.
		/// </summary>
		public virtual Vector3 Position {
			get {
				return position.Value;
			}
		}

		/// <summary>
		/// Inserts given Property in propertyDict.
		/// </summary>
		/// <typeparam name="T">The type of the inserted Property.</typeparam>
		/// <param name="name">The Property name is in this case a PropertyEnum. It is used
		/// for quicker access to specific Property.</param>
		/// <param name="property">The inserting Property.</param>
		public void AddProperty<T>(PropertyEnum propertyName, Property<T> property) {
			if (!propertyDict.ContainsKey(propertyName)) {
				propertyDict.Add(propertyName, property);
			}
		}

		/// <summary>
		/// Inserts given Property in userDefinedPropertyDict.
		/// </summary>
		/// <typeparam Name="T">The type of the inserted Property.</typeparam>
		/// <param Name="Name">The Property name is in this case a string. It is usually used
		/// for user-defined Property.</param>
		/// <param Name="property">The inserting Property.</param>
		public void AddProperty<T>(string propertyName, Property<T> property) {
			if (!userDefinedPropertyDict.ContainsKey(propertyName)) {
				userDefinedPropertyDict.Add(propertyName, property);
			}
		}

		/// <summary>
		/// Removes the Property from userDefinedPropertyDict (if it contains). 
		/// </summary>
		/// <param name="name">The string with a name of the removing Property.</param>
		public void RemoveProperty(string propertyName) {
			if (!userDefinedPropertyDict.ContainsKey(propertyName)) {
				userDefinedPropertyDict.Remove(propertyName);
				return;
			}
		}

		/// <summary>
		/// Removes the Property from propertyDict (if it contains). 
		/// </summary>
		/// <param name="name">The PropertyEnum member with a name of the removing Property.</param>
		public void RemoveProperty(PropertyEnum propertyName) {
			if (propertyDict.ContainsKey(propertyName) && (int)propertyName > 10) {
				propertyDict.Remove(propertyName);
				return;
			}
		}

		/// <summary>
		/// Returns Property by given name (from propertyDict). If propertyDict doesn't contains 
		/// the Property so the exception is thrown.
		/// </summary>
		/// <typeparam name="T">The typeof the Property.</typeparam>
		/// <param name="propertyName">The name of the Property from Enum (base properties).</param>
		/// <returns>Returns founded Property.</returns>
		public Property<T> GetProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDict[propertyName];
			return prop;
		}

		/// <summary>
		/// Returns Property with given name (from userDefinedPropertyDict). If propertyDict doesn't contains 
		/// the Property so the exception is thrown.
		/// </summary>
		/// <typeparam name="T">The typeof the Property.</typeparam>
		/// <param name="propertyName">The name of the Property (user defined properties).</param>
		/// <returns>Returns founded Property.</returns>
		public Property<T> GetProperty<T>(string propertyName) {
			if (!userDefinedPropertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)userDefinedPropertyDict[propertyName];
			return prop;
		}

		/// <summary>
		/// Returns Dictionary with all object Properties. Joins properties from base dictionary (propertyDict) 
		/// and from user defined dictionary (userDefinedPropertyDict).
		/// </summary>
		/// <returns>Returns the dictionary with Properties as objects (runtime generic)</returns>
		public Dictionary<string, object> GetPropertyToDisplay() {
			var result = new Dictionary<string, object>(userDefinedPropertyDict);
			foreach (var property in propertyDict) {
				result.Add(property.Key.ToString(), property.Value);
			}
			return result;
		}

		/// <summary>
		/// Doesn't react on any reason (objects should override).
		/// </summary>
		/// <param name="reason">The reason of calling this function.</param>
		/// <param name="target">The target which invoke this calling.</param>
		/// <returns>The answer on the ActionReason is always None.</returns>
		public virtual ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}

		private DieEventHandler die = null;

		/// <summary>
		/// Is called when the object has less health then 0.
		/// </summary>
		public DieEventHandler DieHandler {
			get { return die; }
			set { die = value; }
		}

		/// <summary>
		/// Subtracts given damage from current hp (currentHp) and checks if object is alive (hp > 0). When
		/// object is "dead" then DieHandler is executed (RaiseDie).
		/// </summary>
		/// <param name="damage">The received damage.</param>
		public virtual void TakeDamage(int damage) {
			var currentHp = hp - damage;
			if (currentHp < 0) {
				RaiseDie();
			}
		}

		/// <summary>
		/// Returns the current state of health.
		/// </summary>
		public virtual int Hp {
			get { return hp; }
		}

		/// <summary>
		/// Returns the distance where that object may contact the unit from its team (70).
		/// </summary>
		public virtual int ShoutDistance {
			get { return 70; }
		}

		/// <summary>
		/// Returns the deffence power of the object (0).
		/// </summary>
		public virtual int DeffPower {
			get { return 0; }
		}

		/// <summary>
		/// Sets all IGameObjects in ShoutDistance around this object (recursively) which are in same Team to given list.
		/// Searchs whole object SolarSysten and checks distance between objects.
		/// </summary>
		/// <param name="objectsInDistance">The list in whitch will be inserted objects in ShoutDistance.</param>
		public void Shout(List<IGameObject> objectsInDistance) {
			var solarSystem = Game.GroupManager.GetSolarSystem(this);

			// Checks movable
			foreach (var imgoPair in solarSystem.GetIMGOs()) {
				if (IsTargetInShoutDistance(imgoPair.Value.Position) && !objectsInDistance.Contains(imgoPair.Value) &&
					team == imgoPair.Value.Team) {
					objectsInDistance.Add(imgoPair.Value);
					imgoPair.Value.Shout(objectsInDistance);
				}
			}

			// Checks static
			foreach (var isgoPair in solarSystem.GetISGOs()) {
				if (IsTargetInShoutDistance(isgoPair.Value.Position) && !objectsInDistance.Contains(isgoPair.Value) &&
					team == isgoPair.Value.Team) {
					objectsInDistance.Add(isgoPair.Value);
					isgoPair.Value.Shout(objectsInDistance);
				}
			}
		}

		/// <summary>
		/// Sets attack indicator to fight mode.
		/// </summary>
		/// <param name="fight">The Fight which leads attack.</param>
		public void StartAttack(Strategy.FightMgr.Fight fight) {
			attack = true;
			this.fight = fight;
		}

		/// <summary>
		/// Unsets attack indicator from fight mode.
		/// </summary>
		public void StopAttack() {
			attack = false;
			fight = null;
		}

		/// <summary>
		/// Inserts given game action to gameActionList.
		/// </summary>
		/// <param name="gameAction">The inserting game action.</param>
		public void AddIGameAction(GameActions.IGameAction gameAction) {
			gameActionList.Add(gameAction);
		}

		/// <summary>
		/// Returns list with all object game actions.
		/// </summary>
		/// <returns>Returns list with all object game actions.</returns>
		public List<GameActions.IGameAction> GetIGameActions() {
			return gameActionList;
		}

		#endregion



		/// <summary>
		/// Calls when object is showed.
		/// </summary>
		protected virtual void OnDisplayed() { }

		/// <summary>
		/// Edits or sets given Property to propertyDict (base Property).
		/// Also checks if the given object is really Property (if not the exception is thrown).
		/// </summary>
		/// <param name="name">The name of the inserting Property (PropertyEnum).</param>
		/// <param name="prop">The inserting Property</param>
		protected void SetProperty(PropertyEnum name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition() == typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (propertyDict.ContainsKey(name)) {
				propertyDict[name] = prop;
			} else {
				propertyDict.Add(name, prop);
			}
		}

		/// <summary>
		/// Edits or sets given Property to userDefinedPropertyDict (user defined Property).
		/// Also checks if the given object is really Property (if not the exception is thrown).
		/// </summary>
		/// <param name="name">The name of the inserting Property (string).</param>
		/// <param name="prop">The inserting Property</param>
		protected void SetProperty(string name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition() == typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (userDefinedPropertyDict.ContainsKey(name)) {
				userDefinedPropertyDict[name] = prop;
			} else {
				userDefinedPropertyDict.Add(name, prop);
			}
		}

		/// <summary>
		/// Delegates the information about object death.
		/// </summary>
		protected void RaiseDie() {
			if (die != null) {
				die(this, new MyDieArgs(hp));
			}
		}

		/// <summary>
		/// Checks if given point is in ShoutDistance of the object.
		/// </summary>
		/// <param name="targetPosition">The checking point.</param>
		/// <returns>Returns if the point is in ShoutDistance.</returns>
		private bool IsTargetInShoutDistance(Vector3 targetPosition) {
			var xd = targetPosition.x - position.Value.x;
			var yd = targetPosition.z - position.Value.z;
			var squaredDistance = xd * xd + yd * yd;

			return squaredDistance < (ShoutDistance * ShoutDistance);
		}
		
		/// <summary>
		/// Updates all object game actions.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		protected virtual void Update(float delay) {
			foreach (var action in gameActionList) {
				action.Update(delay);
			}
		}

		/// <summary>
		/// Parses Mogre.Vector3 from string. In string must be just Vector2 separate by ';'.
		/// </summary>
		/// <param Name="input">The string contains the vector.</param>
		/// <returns>The Mogre.Vector3 parsed from given string</returns>
		protected static Mogre.Vector3 ParseStringToVector3(string input) {
			string[] splitted = input.Split(';');
			Mogre.Vector3 v;
			Console.WriteLine(input);
			try {
				v = new Vector3(Single.Parse(splitted[0]), 0, Single.Parse(splitted[1]));
			} catch (Exception) {
				throw new FormatException("Cannot parse string " + input + " to Mogre.Vector3. Given string was in a bad format (right format: \"x;y\")");
			}
			return v;
		}

	}
}
