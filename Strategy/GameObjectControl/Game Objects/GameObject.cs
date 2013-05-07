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
		protected Mogre.SceneManager manager;

		private int hp = 100;

		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && !this.isVisible) {

				// Control if the entity is inicialized
				if (entity == null) {
					entity = manager.CreateEntity(name, mesh);
				}

				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
				this.isVisible = true;
				OnDisplayed();
			} else {
				if (this.isVisible) {
					position = sceneNode.Position;
					manager.DestroySceneNode(sceneNode);
					sceneNode = null;
					isVisible = false;
				}
			}
		}

		public void Destroy() {
			manager.DestroySceneNode(sceneNode);
			manager.DestroyEntity(entity);
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

		private void RaiseDie() {
			if (die != null) {
				die(this, new MyDieArgs(hp));
			}
		}

		public void TakeDamage(int damage) {
			var actualHp = hp - damage;
			if (actualHp < 0) {
				RaiseDie();
			}
		}

		public virtual int Hp {
			get {
				return hp;
			}
		}
	}

	


}
