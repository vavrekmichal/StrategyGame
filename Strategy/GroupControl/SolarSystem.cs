using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GroupControl {
	public class SolarSystem {

		protected IStaticGameObject sun;
		protected List<IStaticGameObject> isgoObjectList;
		protected List<IMovableGameObject> imgoObjectList;
		protected bool active = false;
		protected Mogre.Vector3 position;


		private string name;
		private const int randConst = 40;

		public SolarSystem(string name, Mogre.Vector3 position) {
			this.name = name;
			isgoObjectList = new List<IStaticGameObject>();
			imgoObjectList = new List<IMovableGameObject>();
		}

		public void setSun(IStaticGameObject sun) {
			this.sun = sun;
		}

		public IStaticGameObject getSun() {
			return sun;
		}

		public void addISGO(IStaticGameObject isgo) {
			if (!isgoObjectList.Contains(isgo)) {
				isgoObjectList.Add(isgo);
			}
		}

		public void addISGO(List<IStaticGameObject> listOfISGO) {
			foreach (IStaticGameObject isgo in listOfISGO) {
				addISGO(isgo);
			}
		}

		public void addIMGO(IMovableGameObject imgo) {
			if (!imgoObjectList.Contains(imgo)) {
				imgoObjectList.Add(imgo);
			}
		}

		public void addIMGO(List<IMovableGameObject> listOfIMGO) {
			foreach (IMovableGameObject imgo in listOfIMGO) {
				addIMGO(imgo);
			}
		}

		public void removeIMGO(IMovableGameObject imgo) {
			if (imgoObjectList.Contains(imgo)) {
				imgoObjectList.Remove(imgo);
			}
		}

		public void hideSolarSystem() {
			if (active) {
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.changeVisible(false);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.changeVisible(false);
				}
				if (sun != null) {
					sun.changeVisible(false);
				}
				active = false;
			}
		}

		public void showSolarSystem() {
			if (!active) {
				
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.changeVisible(true);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.changeVisible(true);
					
				}//check nonActive collisions
				repairHidenCollision(imgoObjectList);
				if (sun != null) {
					sun.changeVisible(true);
				}

				active = true;
			}
		}

		public void update(float delay) {
			if (active) {
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.rotate(delay);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.move(delay);
				}
				if (sun != null) {
					sun.rotate(delay);
				}
			} else {
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.nonActiveRotate(delay);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.nonActiveMove(delay);
				}
				if (sun != null) {
					sun.nonActiveRotate(delay);
				}
			}

		}

		public string Name {
			get { return name; }
		}

		public Mogre.Vector3 Position {
			get { return position; }
		}

		//public List<IStaticGameObject> getISGO() {
		//	return isgoObjects;
		//}

		public List<IStaticGameObject> getISGOs() {
			return isgoObjectList;
		}

		//public List<IMovableGameObject> getIMGO() {
		//	return imgoObjects;
		//}

		public List<IMovableGameObject> getIMGOs() {
			return imgoObjectList;
		}

		private static Random r = new Random();
		private Mogre.Vector3 randomizeVector(Mogre.Vector3 v) {
			int i = r.Next(4);
			switch (i) {
				case 0: v.x += randConst;
					break;
				case 1: v.x -= randConst;
					break;
				case 2: v.z += randConst;
					break;
				case 3: v.z -= randConst;
					break;
			}
			return v;
		}

		private void repairHidenCollision(List<IMovableGameObject> group) {
			Dictionary<Mogre.Vector3, IMovableGameObject> collision = new Dictionary<Mogre.Vector3,IMovableGameObject>();
			foreach (var imgo in group) {
				if (collision.ContainsKey(imgo.Position)) {
					bool isTaken = true;
					Mogre.Vector3 addVect = imgo.Position;
					
					while (isTaken) {
						if (!collision.ContainsKey(addVect)) {
							collision.Add(addVect,imgo);
							isTaken = false;
						} else {
							addVect = randomizeVector(addVect);

						}
					}
					imgo.jumpNextLocation(addVect);
				} else {
					collision.Add(imgo.Position, imgo);
				}
			}
		}
	}
}
