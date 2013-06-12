using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.GameObjectControl {
	public class SolarSystem {

		protected IStaticGameObject sun;
		protected Dictionary<string, IStaticGameObject> isgoObjectDict;
		protected Dictionary<string, IMovableGameObject> imgoObjectDict;
		protected Dictionary<string, IBullet> bulletDict;
		protected bool active = false;
		protected Mogre.Vector3 position;


		private string name;
		private const int randConst = 40;

		public SolarSystem(string name, Mogre.Vector3 position) {
			this.name = name;
			this.position = position;
			isgoObjectDict = new Dictionary<string, IStaticGameObject>();
			imgoObjectDict = new Dictionary<string, IMovableGameObject>();
			bulletDict = new Dictionary<string, IBullet>();
		}

		public IStaticGameObject Sun {
			get { return sun; }
			set { sun = value; }
		}


		public void AddIBullet(IBullet bullet) {
			if (!bulletDict.ContainsKey(bullet.Name)) {
				bulletDict.Add(bullet.Name, bullet);
				bullet.ChangeVisible(active);
			}
		}

		public void RemoveIBullet(IBullet bullet) {
			if (bulletDict.ContainsKey(bullet.Name)) {
				bulletDict.Remove(bullet.Name);
			}
		}

		public bool HasISGO(IStaticGameObject isgo) {
			return isgoObjectDict.ContainsKey(isgo.Name);
		}


		public void AddISGO(IStaticGameObject isgo) {
			if (!isgoObjectDict.ContainsKey(isgo.Name)) {
				isgoObjectDict.Add(isgo.Name, isgo);
				isgo.ChangeVisible(active);
			}
		}

		public void AddISGO(List<IStaticGameObject> listOfISGO) {
			foreach (IStaticGameObject isgo in listOfISGO) {
				AddISGO(isgo);
				isgo.ChangeVisible(active);
			}
		}


		public bool HasIMGO(IMovableGameObject imgo) {
			return imgoObjectDict.ContainsKey(imgo.Name);
		}

		public void RemoveISGO(IStaticGameObject isgo) {
			if (isgoObjectDict.ContainsKey(isgo.Name)) {
				isgoObjectDict.Remove(isgo.Name);
			}
		}

		public void AddIMGO(IMovableGameObject imgo) {
			if (!imgoObjectDict.ContainsKey(imgo.Name)) {
				imgoObjectDict.Add(imgo.Name, imgo);
				imgo.ChangeVisible(active);
			}
		}

		public void AddIMGO(List<IMovableGameObject> listOfIMGO) {
			foreach (IMovableGameObject imgo in listOfIMGO) {
				AddIMGO(imgo);
				imgo.ChangeVisible(active);
			}
		}

		public void RemoveIMGO(IMovableGameObject imgo) {
			if (imgoObjectDict.ContainsKey(imgo.Name)) {
				imgoObjectDict.Remove(imgo.Name);
			}
		}

		public void HideSolarSystem() {
			if (active) {
				foreach (var isgoPair in isgoObjectDict) {
					isgoPair.Value.ChangeVisible(false);
				}
				foreach (var imgoPair in imgoObjectDict) {
					imgoPair.Value.ChangeVisible(false);
				}
				foreach (var bullet in bulletDict) {
					bullet.Value.ChangeVisible(false);
				}
				if (sun != null) {
					sun.ChangeVisible(false);
				}
				active = false;
			}
		}

		public void ShowSolarSystem() {
			if (!active) {

				Game.IGameGUI.SetSolarSystemName(name);

				foreach (var isgoPair in isgoObjectDict) {
					isgoPair.Value.ChangeVisible(true);
				}
				foreach (var imgoPair in imgoObjectDict) {
					imgoPair.Value.ChangeVisible(true);
				}
				foreach (var bullet in bulletDict) {
					bullet.Value.ChangeVisible(true);
				}

				// Check nonActive collisions
				RepairHidenCollision(imgoObjectDict);
				if (sun != null) {
					sun.ChangeVisible(true);
				}

				active = true;
			}
		}

		public void Destroy() {
			if (sun != null) {
				sun.Destroy();
			}

			foreach (var isgoPair in isgoObjectDict) {
				isgoPair.Value.Destroy();
			}
			foreach (var imgoPair in imgoObjectDict) {
				imgoPair.Value.Destroy();
			}
			foreach (var bullet in bulletDict) {
				bullet.Value.Destroy();
			}

		}

		/// <summary>
		/// Function update all objects in SolarSystem in visible or invisible mode.
		/// The function is using shallow copies for removing from original Dictionaries.
		/// </summary>
		/// <param name="delay">Deley between last two frames</param>
		public void Update(float delay) {
			if (active) {
				foreach (var isgoPair in new Dictionary<string, IStaticGameObject>(isgoObjectDict)) {
					isgoPair.Value.Rotate(delay);
				}
				foreach (var imgoPair in new Dictionary<string, IMovableGameObject>(imgoObjectDict)) {
					imgoPair.Value.Move(delay);
				}
				foreach (var bullet in new Dictionary<string, IBullet>(bulletDict)) {
					bullet.Value.Update(delay);
				}
				if (sun != null) {
					sun.Rotate(delay);
				}
			} else {
				foreach (var isgoPair in new Dictionary<string, IStaticGameObject>(isgoObjectDict)) {
					isgoPair.Value.NonActiveRotate(delay);
				}
				foreach (var imgoPair in new Dictionary<string, IMovableGameObject>(imgoObjectDict)) {
					imgoPair.Value.NonActiveMove(delay);
				}
				foreach (var bullet in new Dictionary<string, IBullet>(bulletDict)) {
					bullet.Value.HiddenUpdate(delay);
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

		public Dictionary<string, IStaticGameObject> GetISGOs() {
			return isgoObjectDict;
		}

		public Dictionary<string, IMovableGameObject> GetIMGOs() {
			return imgoObjectDict;
		}

		private static Random r = new Random();
		private Mogre.Vector3 RandomizeVector(Mogre.Vector3 v) {
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

		private void RepairHidenCollision(Dictionary<string, IMovableGameObject> groupInCollistion) {
			Dictionary<Mogre.Vector3, IMovableGameObject> collision = new Dictionary<Mogre.Vector3, IMovableGameObject>();
			foreach (var imgoPair in groupInCollistion) {
				if (collision.ContainsKey(imgoPair.Value.Position)) {
					bool isTaken = true;
					Mogre.Vector3 addVect = imgoPair.Value.Position;

					while (isTaken) {
						if (!collision.ContainsKey(addVect)) {
							collision.Add(addVect, imgoPair.Value);
							isTaken = false;
						} else {
							addVect = RandomizeVector(addVect);

						}
					}
					imgoPair.Value.JumpNextLocation(addVect);
				} else {
					collision.Add(imgoPair.Value.Position, imgoPair.Value);
				}
			}
		}
	}
}
