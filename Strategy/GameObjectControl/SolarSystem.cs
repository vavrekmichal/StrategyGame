using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GameObjectControl {
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
			this.position = position;
			isgoObjectList = new List<IStaticGameObject>();
			imgoObjectList = new List<IMovableGameObject>();
		}

		public IStaticGameObject Sun {
			get { return sun; }
			set { sun = value; }
		}

		public void addISGO(IStaticGameObject isgo) {
			if (!isgoObjectList.Contains(isgo)) {
				isgoObjectList.Add(isgo);
				isgo.ChangeVisible(active);
			}
		}

		public void addISGO(List<IStaticGameObject> listOfISGO) {
			foreach (IStaticGameObject isgo in listOfISGO) {
				addISGO(isgo);
				isgo.ChangeVisible(active);
			}
		}

		public void addIMGO(IMovableGameObject imgo) {
			if (!imgoObjectList.Contains(imgo)) {
				imgoObjectList.Add(imgo);
				imgo.ChangeVisible(active);
			}
		}

		public void addIMGO(List<IMovableGameObject> listOfIMGO) {
			foreach (IMovableGameObject imgo in listOfIMGO) {
				addIMGO(imgo);
				imgo.ChangeVisible(active);
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
					isgo.ChangeVisible(false);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.ChangeVisible(false);
				}
				if (sun != null) {
					sun.ChangeVisible(false);
				}
				active = false;
			}
		}

		public void showSolarSystem() {
			if (!active) {

				Game.GUIManager.SetSolarSystemName(name);

				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.ChangeVisible(true);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.ChangeVisible(true);
					
				}// Check nonActive collisions
				repairHidenCollision(imgoObjectList);
				if (sun != null) {
					sun.ChangeVisible(true);
				}

				active = true;
			}
		}

		public void Update(float delay) {
			if (active) {
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.Rotate(delay);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.Move(delay);
				}
				if (sun != null) {
					sun.Rotate(delay);
				}
			} else {
				foreach (IStaticGameObject isgo in isgoObjectList) {
					isgo.NonActiveRotate(delay);
				}
				foreach (IMovableGameObject imgo in imgoObjectList) {
					imgo.NonActiveMove(delay);
				}
				if (sun != null) {
					sun.NonActiveRotate(delay);
				}
			}

		}

		public string Name {
			get { return name; }
		}

		public Mogre.Vector3 Position {
			get { return position; }
		}

		//public List<IStaticGameObject> GetISGO() {
		//	return isgoObjects;
		//}

		public List<IStaticGameObject> getISGOs() {
			return isgoObjectList;
		}

		//public List<IMovableGameObject> GetIMGO() {
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
					imgo.JumpNextLocation(addVect);
				} else {
					collision.Add(imgo.Position, imgo);
				}
			}
		}
	}
}
