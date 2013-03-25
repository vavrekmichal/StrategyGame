﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GroupControl.RuntimeProperty;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
    public class SpaceShip :MovableGameObject{

		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager,	Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			this.flySpeed = propMgr.getProperty<float>("speed");

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
