using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	abstract class StaticGameObject : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction;
        protected LinkedList<Mogre.Vector3> circularPositions;

		protected int solarSystem;

        protected int planetTeam;
        protected Mogre.SceneManager manager;

		public abstract void rotate(float f);
        public abstract void nonActiveRotate(float f);

        public abstract void changeVisible(bool visible);

		public abstract void produce(float f);

        public int team {
            get {
                return planetTeam;
            }
            set {
                planetTeam = team;
            }
        }


        public int getSolarSystem {
            get { return solarSystem; }
        }
    }
}
