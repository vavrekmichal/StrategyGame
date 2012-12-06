using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	abstract class MovableGameObject : IMovableGameObject{
        protected string name;
        protected Mogre.Entity entity;
        protected Mogre.SceneNode sceneNode;
        protected string mesh;
        //protected List<IGameAction> listOfAction; not implemented
        protected Mogre.Vector3 position;
        protected string team;

        protected Mogre.SceneManager manager;
        
        public abstract void move(float f);

		public abstract void shout();


        /// <summary>
        /// Called when object will be invisible
        /// </summary>
        public virtual void changeVisible(bool visible) {   //now creating
            if (visible) {

                if (entity == null) { //control if the entity is inicialized
                    entity = manager.CreateEntity(name, mesh);
                }

                sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);

                sceneNode.Pitch(new Mogre.Degree(-90f));
                sceneNode.AttachObject(entity);
            } else {
                manager.DestroySceneNode(sceneNode);
            }
        }

        public string getName() {
            return name;
        }
    }
}
