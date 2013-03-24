using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
    public class SpaceShip :MovableGameObject{

		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager,	Vector3 position) {
			//this.name = name;
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			//throw new Exception(Team.Name);
			this.manager = manager;
			this.position = position;

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}



		public override void nonActiveMove(float f) {
			//sceneNode.Roll(new Mogre.Degree(.1f));
		}

		protected override void onDisplayed() {
			
		}
	}
}
