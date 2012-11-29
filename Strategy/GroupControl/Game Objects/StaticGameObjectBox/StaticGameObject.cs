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
        protected Mogre.Vector3 invisblePosition;


        protected int planetTeam;
        protected Mogre.SceneManager manager;

        public abstract void rotate(float f);
        public abstract void nonActiveRotate(float f);
        public abstract void produce(float f);

        protected abstract void onDisplayed();

        
        /// <summary>
        /// int of planets owner
        /// </summary>
        public int team {
            get {
                return planetTeam;
            }
            set {
                planetTeam = team;
            }
        }

        /// <summary>
        /// returning int value of planet´s solar system
        /// </summary>
        public int getSolarSystem {
            get { return solarSystem; }
        }

        /// <summary>
        /// Called when object will be invisible
        /// </summary>
        public void changeVisible(bool visible) {   //now creating
            if (visible) {

                if (entity==null) { //control if the entity is inicialized
                    entity = manager.CreateEntity(name, mesh);
                }
            
                if (invisblePosition == null) { //control inicialization
                    invisblePosition = circularPositions.Last();
                }

                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", invisblePosition);

                sceneNode.Pitch(new Mogre.Degree(-90f));
                sceneNode.AttachObject(entity);
                onDisplayed(); 
            } else {
                invisblePosition = sceneNode.Position;
                manager.DestroySceneNode(sceneNode);
            }
        }
    }
}
