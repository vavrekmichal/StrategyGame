using System;
using System.Collections.Generic;
using System.Linq;
using Strategy.TeamControl;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.GameMaterial;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
    abstract class StaticGameObject : IStaticGameObject {
        protected string name;
        protected Mogre.Entity entity;
        protected Mogre.SceneNode sceneNode;
        protected string mesh;
        protected static Dictionary<string, IGameAction> gameActions; 
        protected static Dictionary<string, List<IStaticGameObject>> gameActionsPermitions;
        protected LinkedList<Mogre.Vector3> circularPositions;

        protected Mogre.Vector3 mDestination = Mogre.Vector3.ZERO; // The destination the object is moving towards

        protected Team planetTeam;
        protected Mogre.SceneManager manager;

        static StaticGameObject() {
            gameActionsPermitions = new Dictionary<string, List<IStaticGameObject>>();
            gameActions = new Dictionary<string, IGameAction>();
            IGameAction o = (IGameAction)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("Strategy.GroupControl.Game_Objects.GameActions.Produce");

            gameActions.Add(o.getName(), o);
            gameActionsPermitions.Add(o.getName(), new List<IStaticGameObject>());
        }

        public void registerExecuter(string nameOfAction, Dictionary<string, IMaterial> materials) {
            if (gameActionsPermitions.ContainsKey(nameOfAction)) {
                gameActionsPermitions[nameOfAction].Add(this);
            }
            registerProducer(materials["Wolenium"], 0.0001);
        }

        private void registerProducer(IMaterial specificType, double value) {
            ((Produce)gameActions["Produce"]).registerExecuter(this,specificType,value);
        }


        public abstract void rotate(float f);
        public abstract void nonActiveRotate(float f);
        protected abstract void onDisplayed();


        public bool tryExecute(string executingAction){
            if (gameActionsPermitions.ContainsKey(executingAction) && gameActionsPermitions[executingAction].Contains(this)) {
                gameActions[executingAction].execute(this, planetTeam);
                return true;
            }
            return false;
        }

        
        

        

        
        /// <summary>
        /// int of planets owner
        /// </summary>
        public Team team {
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
