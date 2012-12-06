using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        public override void move(float f) {
            throw new NotImplementedException();
        }

        public override void shout() {
            throw new NotImplementedException();
        }

    }
}
