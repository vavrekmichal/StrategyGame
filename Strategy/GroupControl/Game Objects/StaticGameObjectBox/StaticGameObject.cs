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
        //protected List<IGameAction> listOfAction; not implemented
        protected LinkedList<Mogre.Vector3> circularPositions;

        //protected int solarSystem;
        //protected Mogre.Vector3 invisblePosition;
        protected Mogre.Vector3 mDestination = Mogre.Vector3.ZERO; // The destination the object is moving towards

        protected string planetTeam;
        protected Mogre.SceneManager manager;

        public abstract void rotate(float f);
        public abstract void nonActiveRotate(float f);
        public abstract void produce(float f);

        protected abstract void onDisplayed();

        
        /// <summary>
        /// int of planets owner
        /// </summary>
        public string team {
            get {
                return planetTeam;
            }
            set {
                planetTeam = team;
            }
        }

        /// <summary>
        /// Called when object will be invisible
        /// </summary>
        public virtual void changeVisible(bool visible) {   //now creating
            if (visible) {

                if (entity==null) { //control if the entity is inicialized
                    entity = manager.CreateEntity(name, mesh);
                }

                if (mDestination == null) { //control inicialization
                    mDestination = circularPositions.Last();
                }

                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", mDestination);

                sceneNode.Pitch(new Mogre.Degree(-90f));
                sceneNode.AttachObject(entity);
                onDisplayed(); 
            } else {
                manager.DestroySceneNode(sceneNode);
            }
        }


        public string getName() {
            return name;
        }
    }
}
