﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	class Sun : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction>();
		protected Mogre.SceneManager manager;

		protected static Team sunTeam;
		/// <summary>
		/// Public constructor. Detect active solar system (0)
		/// </summary>
		/// <param name="name">Unique name</param>
		/// <param name="mesh">mesh of this sun</param>
		/// <param name="solarSystem">number of solar system</param>
		/// <param name="manager">Mogre SceneManager</param>
		public Sun(string name, string mesh, Mogre.SceneManager manager, Team team) {
			this.name = name;
			this.mesh = mesh;
			this.manager = manager;
			this.Team = team;
			entity = manager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Rotating function 
		/// </summary>
		/// <param name="f">deley of frames</param>
		public virtual void Rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(5 * f));
		}

		public virtual void NonActiveRotate(float f) {

		}


		public void Destroy() {
			manager.DestroySceneNode(sceneNode);
			manager.DestroyEntity(entity);
		}


		public virtual ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}

		/// <summary>
		/// Change visibility of sun
		/// </summary>
		/// <param name="visible">boolean value if the sun is visible or not</param>
		public void ChangeVisible(bool visible) {   //now creating
			if (visible) {
				if (entity == null) {
					entity = manager.CreateEntity(name, mesh);
				}
				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", Mogre.Vector3.ZERO);

				sceneNode.Pitch(new Mogre.Degree(-90f));
				sceneNode.AttachObject(entity);
			} else {
				manager.DestroySceneNode(sceneNode);
			}
		}

		public Dictionary<string, object> GetPropertyToDisplay() {
			var propToDisp = new Dictionary<string, object>();
			return propToDisp;
		}

		/// <summary>
		/// It will be always 0.
		/// </summary>
		public Team Team {
			get {
				return sunTeam;
			}
			set {
				sunTeam = value;
			}
		}

		public string Name {
			get { return name; }
		}

		public string Mesh {
			get { return mesh; }
		}

		public Vector3 getPosition() {
			return Vector3.ZERO;
		}

		public float PickUpDistance {
			get { return 250; }
		}

		public float OccupyDistance {
			get { return 0; }
		}

		public int OccupyTime {
			get { return -1; }
		}

		public void AddProperty<T>(PropertyEnum name, Property<T> property) { }
		public void AddProperty<T>(string name, Property<T> property) { }
		public void RemoveProperty(PropertyEnum name) { }
		public void RemoveProperty(string name) { }

		public Vector3 Position {
			get {
				return Vector3.ZERO;
			}
		}

		public bool TryExecute(string executingAction) {
			throw new NotImplementedException();
		}

		private DieEventHandler die = null;
		public DieEventHandler DieHandler {
			get { return die; }
			set { die = value; }
		}

		public virtual int Hp {
			get {
				return 100;
			}
		}

		public void TakeDamage(int damage) {
			
		}
	}
}
