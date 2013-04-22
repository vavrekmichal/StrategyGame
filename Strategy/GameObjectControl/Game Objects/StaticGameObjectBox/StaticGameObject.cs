using System;
using System.Collections.Generic;
using System.Linq;
using Strategy.TeamControl;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameMaterial;
using Mogre;
using System.Reflection;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.Exceptions;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	public abstract class StaticGameObject : IStaticGameObject {
		protected string name;
		protected Entity entity;
		protected SceneNode sceneNode;
		protected string mesh;


		protected Vector3 mDestination = Vector3.ZERO; // The position

		protected Team planetTeam;
		protected Mogre.SceneManager manager;

		protected static Dictionary<string, IGameAction> gameActions;
		protected static Dictionary<string, List<IStaticGameObject>> gameActionsPermitions;

		protected Dictionary<PropertyEnum, object> propertyDict = new Dictionary<PropertyEnum, object>();

		//Look here create file load file
		static StaticGameObject() {
			gameActionsPermitions = new Dictionary<string, List<IStaticGameObject>>();
			gameActions = new Dictionary<string, IGameAction>();
			IGameAction o = (IGameAction)Assembly.GetExecutingAssembly().CreateInstance("Strategy.GameObjectControl.Game_Objects.GameActions.Produce"); //TODO delete

			gameActions.Add(o.getName(), o);
			gameActionsPermitions.Add(o.getName(), new List<IStaticGameObject>());
		}

		public void registerExecuter(string nameOfAction, Dictionary<string, IMaterial> materials, string material) {
			if (gameActionsPermitions.ContainsKey(nameOfAction)) {
				gameActionsPermitions[nameOfAction].Add(this);
			}
			registerProducer(materials[material], 0.01);
		}

		private void registerProducer(IMaterial specificType, double value) {
			((Produce)gameActions["Produce"]).registerExecuter(this, specificType, value);
		}

		public Property<T> getProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object " + Name + " doesn't have property " + propertyName + ".");
			}
			var prop = (Property<T>)propertyDict[propertyName];
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

		/// <summary>
		/// Rotating function 
		/// </summary>
		/// <param name="f">Deley of frames</param>
		public virtual void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 * f));
		}

		/// <summary>
		/// StaticGameObject doesn't move in non-active mode but child can override.
		/// </summary>
		/// <param name="f">Deley of frames</param>
		public virtual void nonActiveRotate(float f) {
		}

		public virtual Dictionary<PropertyEnum, object> getPropertyToDisplay() {
			var propToDisp = new Dictionary<PropertyEnum, object>();
			return propToDisp;
		}

		public virtual ActionReaction reactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}

		protected abstract void onDisplayed();


		public bool tryExecute(string executingAction) {
			if (gameActionsPermitions.ContainsKey(executingAction) && gameActionsPermitions[executingAction].Contains(this)) {
				gameActions[executingAction].execute(this, planetTeam);
				return true;
			}
			return false;
		}

		public Team Team {
			get {
				return planetTeam;
			}
			set {
				planetTeam = value;
			}
		}

		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public virtual void changeVisible(bool visible) {  
			if (visible) {
				if (sceneNode == null) {
					if (entity == null) { // Control if the entity is inicialized
						entity = manager.CreateEntity(name, mesh);
					}

					sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", mDestination);

					sceneNode.Pitch(new Degree(-90f));
					sceneNode.AttachObject(entity);
					onDisplayed();
				}
			} else {
				if (sceneNode != null) {
					mDestination = sceneNode.Position;
					manager.DestroySceneNode(sceneNode);
					sceneNode = null;
				}
			}
		}


		public string Name {
			get { return name; }
		}

		public string Mesh {
			get { return mesh; }
		}

		public virtual float PickUpDistance {
			get { return 150; }
		}


		public Vector3 Position {
			get {
				if (sceneNode == null) {
					return mDestination;
				} else {
					return sceneNode.Position;
				}
			}
		}


		
	}
}
