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

		public Sun(string name, string mesh, int solarSystem, Mogre.SceneManager manager) {
			this.name = name;
			this.mesh = mesh;
            this.manager = manager;
            this.solarSystem = solarSystem;
			

            if (solarSystem==0) {
                entity = manager.CreateEntity(name, mesh);
                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", Mogre.Vector3.ZERO);
                sceneNode.AttachObject(entity);
                sceneNode.Pitch(new Mogre.Degree(-90f));
            }
		}

		public void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 *f));
		}

        public void nonActiveRotate(float f) {
        }

        public int team {
            get {
                return sunTeam;
            }
            set {
                sunTeam=team;
            }
        }

        public int getSolarSystem{
            get { return solarSystem; 
            }
        }

        public void changeVisible(bool visible) {
            if (visible) {
                entity = manager.CreateEntity(name, mesh);
                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", Mogre.Vector3.ZERO);

                sceneNode.Pitch(new Mogre.Degree(-90f));
                sceneNode.AttachObject(entity);
            } else {
                entity.Dispose();
                sceneNode.Dispose();
            }
        }
    }
}
