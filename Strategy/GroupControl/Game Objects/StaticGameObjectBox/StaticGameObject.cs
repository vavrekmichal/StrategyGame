using System;
using System.Collections.Generic;
using System.Linq;
using Strategy.TeamControl;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.GameMaterial;
using Mogre;
using System.Reflection;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
    public abstract class StaticGameObject : IStaticGameObject {
        protected string name;
        protected Entity entity;
        protected SceneNode sceneNode;
        protected string mesh;

        protected LinkedList<Mogre.Vector3> circularPositions;

        protected Vector3 mDestination = Vector3.ZERO; // The destination the object is moving towards

        protected Team planetTeam;
        protected Mogre.SceneManager manager;

		protected static Dictionary<string, IGameAction> gameActions;
		protected static Dictionary<string, List<IStaticGameObject>> gameActionsPermitions;

		//Look here create file load file
        static StaticGameObject() {
            gameActionsPermitions = new Dictionary<string, List<IStaticGameObject>>();
            gameActions = new Dictionary<string, IGameAction>();
            IGameAction o = (IGameAction)Assembly.GetExecutingAssembly().CreateInstance("Strategy.GroupControl.Game_Objects.GameActions.Produce");

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
            ((Produce)gameActions["Produce"]).registerExecuter(this,specificType,value);
        }

		/// <summary>
		/// Rotating function 
		/// </summary>
		/// <param name="f">deley of frames</param>
		public virtual void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 * f));
		}

		/// <summary>
		/// StaticGameObject doesn't move in non-active mode but child can override.
		/// </summary>
		/// <param name="f">deley of frames</param>
		public virtual void nonActiveRotate(float f) {
		}

		//TODO implement answer
		public virtual ActionAnswer onMouseAction(ActionFlag reason, Vector3 point, object hitTestResult) {
			return ActionAnswer.None;
		}

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
        public virtual void changeVisible(bool visible) {   //now creating
            if (visible) {

                if (entity==null) { //control if the entity is inicialized
                    entity = manager.CreateEntity(name, mesh);
                }

                if (mDestination == null) { //control inicialization
                    mDestination = circularPositions.Last();
                }

                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", mDestination);

                sceneNode.Pitch(new Degree(-90f));
                sceneNode.AttachObject(entity);
                onDisplayed(); 
            } else {
                manager.DestroySceneNode(sceneNode);
            }
        }


        public string Name {
            get {return name;}
        }

		public string Mesh {
			get { return mesh; }
		}


	}
}
