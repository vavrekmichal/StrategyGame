using System.Collections.Generic;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	/// <summary>
	/// Special type of a static game object. Must not be more than one in one SolarSystem.
	/// The sun is placed in center of the SolarSystem and rotates around vertical axis.
	/// </summary>
	class Sun : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction>();

		protected static Team sunTeam = new Team("Sun");
		private readonly Property<Vector3> position = new Property<Vector3>(new Vector3(0, 0, 0));

		protected Dictionary<PropertyEnum, object> propertyDict;

		/// <summary>
		/// Initializes Mogre Entity and gets mesh from arguments. Also sets position (vector 0,0,0).
		/// </summary>
		/// <param name="name">The unique name.</param>
		/// <param name="args">The arguments (should be just one with mesh name).</param>
		public Sun(string name, object[] args) {
			propertyDict = new Dictionary<PropertyEnum, object>();
			this.name = name;
			this.mesh = (string)args[0];

			entity = Game.SceneManager.CreateEntity(name, mesh);
			propertyDict.Add(PropertyEnum.Position, position);
		}

		/// <summary>
		/// Rotates with the sun's SceneNode. 
		/// </summary>
		/// <param name="f">The deley between last two frames.</param>
		public virtual void Rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(5 * f));
		}

		/// <summary>
		/// Does nothing in a invisible mode.
		/// </summary>
		/// <param name="f">The deley between last two frames.</param>
		public virtual void NonActiveRotate(float f) { }

		/// <summary>
		/// Destroys SceneNode and Entity.
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
		/// Doesn't react on any ActionReason, always answers None.
		/// </summary>
		/// <param name="reason">The reason of calling this function.</param>
		/// <param name="target">The target which invoke this calling.</param>
		/// <returns>Always returns None.</returns>
		public virtual ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}

		/// <summary>
		/// Changes visibility of sun (creates or destroys SceneNode).
		/// </summary>
		/// <param name="visible">boolean value if the sun is visible or not</param>
		public void ChangeVisible(bool visible) {   //now creating
			if (visible) {
				if (entity == null) {
					entity = Game.SceneManager.CreateEntity(name, mesh);
				}
				sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", Mogre.Vector3.ZERO);

				sceneNode.Pitch(new Mogre.Degree(-90f));
				sceneNode.AttachObject(entity);
			} else {
				Game.SceneManager.DestroySceneNode(sceneNode);
				sceneNode = null;
			}
		}

		/// <summary>
		/// Returns a dictionary with position Property.
		/// </summary>
		/// <returns>Returns a dictionary with position Property.</returns>
		public Dictionary<string, object> GetPropertyToDisplay() {
			var result = new Dictionary<string, object>();
			foreach (var property in propertyDict) {
				result.Add(property.Key.ToString(), property.Value);
			}
			return result;
		}

		/// <summary>
		/// Sets or gets a sun's team.
		/// </summary>
		public Team Team {
			get {
				return sunTeam;
			}
			set {
				sunTeam = value;
			}
		}

		/// <summary>
		/// Return sun's unique name.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Returns 250 (distance where objects will stop when they will go to the sun).
		/// </summary>
		public float PickUpDistance {
			get { return 250; }
		}

		/// <summary>
		/// Returns 0 (cannot be occupied).
		/// </summary>
		public float OccupyDistance {
			get { return 0; }
		}

		/// <summary>
		/// Cannot be occupied (return -1).
		/// </summary>
		public int OccupyTime {
			get { return -1; }
		}

		/// <summary>
		/// Returns Property by given name if exists (if not - returns null).
		/// </summary>
		/// <typeparam name="T">The type of the Property.</typeparam>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>Returns Property by given name with given type.</returns>
		public Property<T> GetProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				return null;
			}
			var prop = (Property<T>)propertyDict[propertyName];
			return prop;
		}

		/// <summary>
		/// Returns null. The sun has no user defined Properties.
		/// </summary>
		/// <typeparam name="T">The type of the Property.</typeparam>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>null</returns>
		public Property<T> GetProperty<T>(string name) { return null; }

		/// <summary>
		/// Inserts given property to propertyDict by given name (PropertyEnum).
		/// </summary>
		/// <typeparam name="T">The type of the inserting Property.</typeparam>
		/// <param name="propertyName">The name of the inserting Property.</param>
		/// <param name="property">The inserting Property.</param>
		public void AddProperty<T>(PropertyEnum propertyName, Property<T> property) {
			if (!propertyDict.ContainsKey(propertyName)) {
				propertyDict.Add(propertyName, property);
			}
		}

		/// <summary>
		/// Does nothing, the sun has no user defined Properties.
		/// </summary>
		/// <typeparam name="T">The type of the inserting Property.</typeparam>
		/// <param name="propertyName">The name of the inserting Property.</param>
		/// <param name="property">The inserting Property.</param>
		public void AddProperty<T>(string name, Property<T> property) { }

		/// <summary>
		/// Does nothing, the sun never removes any property.
		/// </summary>
		/// <param name="name">The name of removing Property.</param>
		public void RemoveProperty(PropertyEnum name) { }

		/// <summary>
		/// Does nothing, the sun never removes any property. 
		/// </summary>
		/// <param name="name">The name of removing Property.</param>
		public void RemoveProperty(string name) { }

		/// <summary>
		/// Returns a current sun's position.
		/// </summary>
		public Vector3 Position {
			get {
				return position.Value;
			}
		}

		/// <summary>
		/// Does nothing. The sun cannot die.
		/// </summary>
		public DieEventHandler DieHandler {
			get { return null; }
			set { }
		}

		/// <summary>
		/// Returns always 100. The sun cannot die.
		/// </summary>
		public virtual int Hp {
			get {
				return 100;
			}
		}

		/// <summary>
		/// Returns 0, sun cannot shout.
		/// </summary>
		public int ShoutDistance {
			get { return 0; }
		}

		/// <summary>
		/// The sun can not take damage.
		/// </summary>
		/// <param name="damage">The received damage.</param>
		public void TakeDamage(int damage) { }

		/// <summary>
		/// Does not modify the list.
		/// </summary>
		/// <param name="objectsInDistance">The list with objects in ShoutDistance.</param>
		public void Shout(List<IGameObject> objectsInDistance) { }

		/// <summary>
		/// Returns 0, the sun has no defence.
		/// </summary>
		public int DeffPower {
			get { return 0; }
		}

		/// <summary>
		/// Does not fight.
		/// </summary>
		/// <param name="fight">The instance of a fight.</param>
		public void StartAttack(Strategy.FightMgr.Fight fight) { }

		/// <summary>
		/// Does not fight, so does not stop.
		/// </summary>
		public void StopAttack() { }

		/// <summary>
		/// Cannot has any game action.
		/// </summary>
		/// <param name="gameAction">The inserting game action (will not be inserted).</param>
		public void AddIGameAction(IGameAction gameAction) { }

		/// <summary>
		/// Returns empty list, because has no game action.
		/// </summary>
		/// <returns></returns>
		public List<IGameAction> GetIGameActions() {
			return new List<IGameAction>();
		}
	}
}
