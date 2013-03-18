using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
    class SpaceShip :MovableGameObject{

        public SpaceShip(string name, string mesh, string team, Mogre.SceneManager manager,Mogre.Vector3 position) {
            this.name = name;
            this.mesh = mesh;
            this.team = team;
            this.manager = manager;
            this.position = position;

            //Mogre inicialization of object
            entity = manager.CreateEntity(name, mesh);
			sceneNode = manager.RootSceneNode.CreateChildSceneNode(name+"Bla", position);
			sceneNode.AttachObject(entity);
        }

		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager,	Vector3 position) {
			this.name = name;
			this.mesh = mesh;
			this.team = team;
			this.manager = manager;
			this.position = position;

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
			sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Bla", position);
			sceneNode.AttachObject(entity);
		}



		public override void nonActiveMove(float f) {
			sceneNode.Roll(new Mogre.Degree(.1f));
		}
	}
}
