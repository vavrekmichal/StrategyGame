using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	class Sun : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction>();
        protected Mogre.SceneManager manager;
        
        protected int sunTeam = 0;
        protected int solarSystem;

        /// <summary>
        /// Public constructor. Detect active solar system (0)
        /// </summary>
        /// <param name="name">Unique name</param>
        /// <param name="mesh">mesh of this sun</param>
        /// <param name="solarSystem">number of solar system</param>
        /// <param name="manager">Mogre SceneManager</param>
		public Sun(string name, string mesh, int solarSystem, Mogre.SceneManager manager) {
			this.name = name;
			this.mesh = mesh;
            this.manager = manager;
            this.solarSystem = solarSystem;
            entity = manager.CreateEntity(name, mesh);

            if (solarSystem==0) {
                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", Mogre.Vector3.ZERO);
                sceneNode.AttachObject(entity);
                sceneNode.Pitch(new Mogre.Degree(-90f));
            }
		}

        /// <summary>
        /// Rotating function 
        /// </summary>
        /// <param name="f">deley of frames</param>
		public void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 *f));
		}

        public void nonActiveRotate(float f) {
        }

        /// <summary>
        /// It will be always 0.
        /// </summary>
        public int team {
            get {
                return sunTeam;
            }
            set {
                sunTeam=team;
            }
        }

        /// <summary>
        /// returning int value of planet´s solar system
        /// </summary>
        public int getSolarSystem{
            get { return solarSystem; 
            }
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
    }
}
