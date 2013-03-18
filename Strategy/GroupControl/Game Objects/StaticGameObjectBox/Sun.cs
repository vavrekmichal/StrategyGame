﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GroupControl.Game_Objects.GameActions;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	class Sun : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction>();
        protected Mogre.SceneManager manager;
        
        protected static Team sunTeam = new Team("Suns");
        /// <summary>
        /// Public constructor. Detect active solar system (0)
        /// </summary>
        /// <param name="name">Unique name</param>
        /// <param name="mesh">mesh of this sun</param>
        /// <param name="solarSystem">number of solar system</param>
        /// <param name="manager">Mogre SceneManager</param>
		public Sun(string name, string mesh, Mogre.SceneManager manager) {
			this.name = name;
			this.mesh = mesh;
            this.manager = manager;
            entity = manager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Rotating function 
		/// </summary>
		/// <param name="f">deley of frames</param>
		public virtual void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 * f));
		}

		public virtual void nonActiveRotate(float f) {

		}

        /// <summary>
        /// Change visibility of sun
        /// </summary>
        /// <param name="visible">boolean value if the sun is visible or not</param>
        public void changeVisible(bool visible) {   //now creating
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

		/// <summary>
		/// It will be always 0.
		/// </summary>
		public Team Team {
			get {
				return sunTeam;
			}
			set {

			}
		}

        public string Name{
            get{return name;}
        }

		public string Mesh {
			get { return mesh; }
		}

        public Vector3 getPosition() {
            return Vector3.ZERO;
        }


        public bool tryExecute(string executingAction) {
            throw new NotImplementedException();
        }
    }
}
