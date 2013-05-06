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
	public abstract class GameObject :IGameObject {


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

		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public virtual void changeVisible(bool visible) {
			if (visible && !this.isVisible) {

				// Control if the entity is inicialized
				if (entity == null) {
					entity = manager.CreateEntity(name, mesh);
				}

				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
				this.isVisible = true;
				onDisplayed();
			} else {
				if (this.isVisible) {
					position = sceneNode.Position;
					manager.DestroySceneNode(sceneNode);
					sceneNode = null;
					this.isVisible = false;
				}
			}
		}

		/// <summary>
		/// Calls when object is showed by SolarSystem
		/// </summary>
		protected virtual void onDisplayed() { }

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

		public void addProperty<T>(PropertyEnum propertyName, Property<T> property) {
			if (!propertyDict.ContainsKey(propertyName)) {
				propertyDict.Add(propertyName, property);
			}
		}
		public void addProperty<T>(string propertyName, Property<T> property) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				propertyDictUserDefined.Add(propertyName, property);
			}
		}
		public void removeProperty(string propertyName) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				propertyDictUserDefined.Remove(propertyName);
				return;
			}
		}

		public void removeProperty(PropertyEnum propertyName) {
			if (propertyDict.ContainsKey(propertyName) && (int)propertyName > 10) {
				propertyDict.Remove(propertyName);
				return;
			}
		}


		public Property<T> getProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDict[propertyName];
			return prop;
		}

		public Property<T> getProperty<T>(string propertyName) {
			if (!propertyDictUserDefined.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDictUserDefined[propertyName];
			return prop;
		}

		protected void setProperty(PropertyEnum name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition() == typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (propertyDict.ContainsKey(name)) {
				propertyDict[name] = prop;
			} else {
				propertyDict.Add(name, prop);
			}
		}

		protected void setProperty(string name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition() == typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (propertyDictUserDefined.ContainsKey(name)) {
				propertyDictUserDefined[name] = prop;
			} else {
				propertyDictUserDefined.Add(name, prop);
			}
		}


		public Dictionary<string, object> getPropertyToDisplay() {
			var result = new Dictionary<string, object>(propertyDictUserDefined);
			foreach (var property in propertyDict) {
				result.Add(property.Key.ToString(), property.Value);
			}
			return result;
		}

		public virtual ActionReaction reactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}
	}
}
