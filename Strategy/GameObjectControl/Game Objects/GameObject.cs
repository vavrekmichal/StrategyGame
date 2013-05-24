using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.Exceptions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects {

	public delegate void DieEventHandler(IGameObject igo, MyDieArgs m);

	public abstract class GameObject : IGameObject {


		protected Dictionary<PropertyEnum, object> propertyDict = new Dictionary<PropertyEnum, object>();
		protected Dictionary<string, object> propertyDictUserDefined = new Dictionary<string, object>();

		protected Team team;
		protected string name;
		protected bool isVisible;

		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected Mogre.Vector3 position;
		protected bool attack;
		protected Strategy.FightMgr.Fight fight;

		private int hp = 100;

		/// <summary>
		/// Called when visibility is changed. Visible is true -> Function creates SceneNode and checks Entity (if is null -> initializes.
		/// Visible is false -> Destroy SceneNode and save actual position.
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && !this.isVisible) {

				// Control if the entity is inicialized
				if (entity == null) {
					entity = Game.SceneManager.CreateEntity(name, mesh);
				}

				sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
				this.isVisible = true;
				OnDisplayed();
			} else {
				if (this.isVisible) {
					position = sceneNode.Position;
					Game.SceneManager.DestroySceneNode(sceneNode);
					sceneNode = null;
					isVisible = false;
				}
			}
		}

		public void Destroy() {
			if (entity!= null) {
				Game.SceneManager.DestroySceneNode(sceneNode);
				Game.SceneManager.DestroyEntity(entity);
				sceneNode = null;
				entity = null;
			}
		}


		/// <summary>
		/// Calls when object is showed by SolarSystem
		/// </summary>
		protected virtual void OnDisplayed() { }

		public string Name {
			get { return name; }
		}

		public bool Visible {
			get { return isVisible; }
		}

		public Team Team {
			get {
				return team;
			}
			set {
				team = value;
			}
		}

		public virtual float PickUpDistance {
			get { return 200; }
		}

		public virtual float OccupyDistance {
			get { return 220; }
		}

		public virtual int OccupyTime {
			get { return 120; }
		}

		public virtual Vector3 Position {
			get {
				if (sceneNode == null) {
					return position;
				} else {
					return sceneNode.Position;
				}
			}
		}

		public void AddProperty<T>(PropertyEnum propertyName, Property<T> property) {
			if (!propertyDict.ContainsKey(propertyName)) {
				propertyDict.Add(propertyName, property);
			}
		}
		public void AddProperty<T>(string propertyName, Property<T> property) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				propertyDictUserDefined.Add(propertyName, property);
			}
		}
		public void RemoveProperty(string propertyName) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				propertyDictUserDefined.Remove(propertyName);
				return;
			}
		}

		public void RemoveProperty(PropertyEnum propertyName) {
			if (propertyDict.ContainsKey(propertyName) && (int)propertyName > 10) {
				propertyDict.Remove(propertyName);
				return;
			}
		}


		public Property<T> GetProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDict[propertyName];
			return prop;
		}

		public Property<T> GetProperty<T>(string propertyName) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDictUserDefined[propertyName];
			return prop;
		}

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

		protected void SetProperty(string name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition() == typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (propertyDictUserDefined.ContainsKey(name)) {
				propertyDictUserDefined[name] = prop;
			} else {
				propertyDictUserDefined.Add(name, prop);
			}
		}


		public Dictionary<string, object> GetPropertyToDisplay() {
			var result = new Dictionary<string, object>(propertyDictUserDefined);
			foreach (var property in propertyDict) {
				result.Add(property.Key.ToString(), property.Value);
			}
			return result;
		}

		public virtual ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}

		private DieEventHandler die = null;
		public DieEventHandler DieHandler {
			get { return die; }
			set { die = value; }
		}

		protected void RaiseDie() {
			if (die != null) {
				die(this, new MyDieArgs(hp));
			}
		}

		public virtual void TakeDamage(int damage) {
			var actualHp = hp - damage;
			if (actualHp < 0) {
				RaiseDie();
			}
		}

		public virtual int Hp {
			get { return hp; }
		}

		public virtual int ShoutDistance {
			get { return 70; }
		}

		public virtual int AttackPower {
			get { return 0; }
		}

		public virtual int DeffPower {
			get { return 0; }
		}

		public void Shout(List<IGameObject> objectsInDistance) {

			int solarSystemNumber = Game.GroupManager.GetSolarSystemsNumber(this);
			var solarSystem = Game.GroupManager.GetSolarSystem(solarSystemNumber);

			foreach (var imgoPair in solarSystem.GetIMGOs()) {
				if (IsTargetInShoutDistance(imgoPair.Value.Position) && !objectsInDistance.Contains(imgoPair.Value) &&
					team == imgoPair.Value.Team) {
					objectsInDistance.Add(imgoPair.Value);
					imgoPair.Value.Shout(objectsInDistance);
				}
			}
			foreach (var isgoPair in solarSystem.GetISGOs()) {
				if (IsTargetInShoutDistance(isgoPair.Value.Position) && !objectsInDistance.Contains(isgoPair.Value) &&
					team == isgoPair.Value.Team) {
					objectsInDistance.Add(isgoPair.Value);
					isgoPair.Value.Shout(objectsInDistance);
				}
			}
		}

		private bool IsTargetInShoutDistance(Vector3 targetPosition) {
			var xd = targetPosition.x - position.x;
			var yd = targetPosition.z - position.z;
			var squaredDistance = xd * xd + yd * yd;

			return squaredDistance < (ShoutDistance * ShoutDistance);
		}

		public void StartAttack(Strategy.FightMgr.Fight fight){
			attack = true;
			this.fight = fight;
		}

		public void StopAttack() {
			attack = false;
			fight = null;
		}


		public void AddIGameAction(GameActions.IGameAction gameAction) {
			throw new NotImplementedException();
		}

		public List<GameActions.IGameAction> GetIGameActions() {
			throw new NotImplementedException();
		}
	}




}
