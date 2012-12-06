using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.TeamControl {
    class Team {

        protected List<IMovableGameObject> imgoObjects;
        protected List<IStaticGameObject> isgoObjects;
        protected string name;

        public Team(string name) {
            this.name = name;
            imgoObjects = new List<IMovableGameObject>();
            isgoObjects = new List<IStaticGameObject>();
        }

        //isgo
        public void addISGO(IStaticGameObject isgo) {
            if (!isgoObjects.Contains(isgo)) {
                isgoObjects.Add(isgo);   
            }        
        }

        public void removeISGO(IStaticGameObject isgo) {
            if (isgoObjects.Contains(isgo)) {
                isgoObjects.Remove(isgo);
            }   
        }

        //imgo
        public void addIMGO(IMovableGameObject imgo) {
            if (!imgoObjects.Contains(imgo)) {
                imgoObjects.Add(imgo);
            }
        }

        public void removeIMGO(IMovableGameObject imgo) {
            if (imgoObjects.Contains(imgo)) {
                imgoObjects.Remove(imgo);
            }
        }

        //others
        public string getName() {
            return name;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(name+"\n");
            s.Append("\t ISGO" + "\n");
            foreach (IStaticGameObject isgo in isgoObjects) {
                s.Append("\t\t"+isgo.getName()+"\n");
            }

            s.Append("\t IMGO" + "\n");
            foreach (IMovableGameObject imgo in imgoObjects) {
                s.Append("\t\t" + imgo.getName() + "\n");
            }

            return s.ToString();
        }
    }
}
