using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GroupControl {
    class SolarSystem {

        protected Sun sun;
        protected List<IStaticGameObject> isgoObjects;
        protected List<IMovableGameObject> imgoObjects;
        protected bool active = false;

        private string name;


        public SolarSystem(string name) {
            this.name = name;
            isgoObjects = new List<IStaticGameObject>();
            imgoObjects = new List<IMovableGameObject>();
        }

        public void setSun(Sun sun) {
            this.sun = sun;
        }

        public void addISGO(IStaticGameObject isgo) {
            if (!isgoObjects.Contains(isgo)) {
                isgoObjects.Add(isgo);
            }
        }

        public void addISGO(List<IStaticGameObject> listOfISGO) {
            foreach (IStaticGameObject isgo in listOfISGO) {
                addISGO(isgo);
            }
        }

        public void addIMGO(IMovableGameObject imgo) {
            if (!imgoObjects.Contains(imgo)) {
                imgoObjects.Add(imgo);
            }
        }

        public void addIMGO(List<IMovableGameObject> listOfIMGO) {
            foreach (IMovableGameObject imgo in listOfIMGO) {
                addIMGO(imgo);
            }
        }

        public void removeIMGO(IMovableGameObject imgo) {
            if (imgoObjects.Contains(imgo)) {
                imgoObjects.Remove(imgo);
            }
        }

        public void hideSolarSystem() {
            //foreach (IMovableGameObject imgo in imgoObjects) {
            //    imgo.changeVisibility(false);
            //}
            if (active) {
                foreach (IStaticGameObject isgo in isgoObjects) {
                    isgo.changeVisible(false);
                }
				if (sun != null) {
					sun.changeVisible(false);
				}
                active = false;
            }
        }

        public void showSolarSystem() {
            //foreach (IMovableGameObject imgo in imgoObjects) {
            //    imgo.changeVisibility(true);
            //}
            if (!active) {
                foreach (IStaticGameObject isgo in isgoObjects) {
                    isgo.changeVisible(true);
                }
				if (sun != null) {
					sun.changeVisible(true);
				}
                active = true;
            }
        }

        public void update(float delay) {
            //foreach (IMovableGameObject imgo in imgoObjects) {
            //    imgo.move(delay);
            //}
            if (active) {
                foreach (IStaticGameObject isgo in isgoObjects) {
                    isgo.rotate(delay);
                    
                }
                if (sun != null) {
                    sun.rotate(delay);
                }
            } else {
                foreach (IStaticGameObject isgo in isgoObjects) {
                    isgo.nonActiveRotate(delay);
                }
                if (sun != null) {
                    sun.nonActiveRotate(delay);
                }
            }

        }

        public string getName() {
            return name;
        }

        public List<IStaticGameObject> getISGO() {
            return isgoObjects;
        }

        public List<IMovableGameObject> getIMGO() {
            return imgoObjects;
        }
    }
}
