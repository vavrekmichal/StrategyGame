using System;
using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.GameObjectControl {
	/// <summary>
	/// Represents a solar system in the game. Updates all objects in the SolarSystem and
	/// sets visibility of the objects.
	/// </summary>
	public class SolarSystem {

		protected IStaticGameObject sun;
		protected Dictionary<string, IStaticGameObject> isgoObjectDict;
		protected Dictionary<string, IMovableGameObject> imgoObjectDict;
		protected Dictionary<string, IBullet> bulletDict;

		protected bool active = false;
		protected bool hasGate = false;
		// Absolute position at Space used for calculate distance between SolarSystems.
		protected Mogre.Vector3 position;
		protected string name;

		private const int randConst = 40;

		/// <summary>
		/// Initializes dictionaries and sets the name and the position at Space of the SolarSystem.
		/// </summary>
		/// <param name="name">The name of the SolarSystem.</param>
		/// <param name="position">The position of the SolarSystem.</param>
		public SolarSystem(string name, Mogre.Vector3 position) {
			this.name = name;
			this.position = position;
			isgoObjectDict = new Dictionary<string, IStaticGameObject>();
			imgoObjectDict = new Dictionary<string, IMovableGameObject>();
			bulletDict = new Dictionary<string, IBullet>();
		}

		#region Public
		/// <summary>
		/// Sets or gets SolarSystem's Sun. The Sun is max one at each SolarSystem.
		/// </summary>
		public IStaticGameObject Sun {
			get { return sun; }
			set { sun = value; }
		}

		/// <summary>
		/// Inserts the bullet to the SolarSystem.
		/// </summary>
		/// <param name="bullet">The inserting bullet.</param>
		public void AddIBullet(IBullet bullet) {
			if (!bulletDict.ContainsKey(bullet.Name)) {
				bulletDict.Add(bullet.Name, bullet);
				bullet.ChangeVisible(active);
			}
		}

		/// <summary>
		/// Removes the bullet from the SolarSystem.
		/// </summary>
		/// <param name="bullet">The removing SolarSystem.</param>
		public void RemoveIBullet(IBullet bullet) {
			if (bulletDict.ContainsKey(bullet.Name)) {
				bulletDict.Remove(bullet.Name);
			}
		}

		/// <summary>
		/// Check if SolarSystem contains the IStaticGameObject.
		/// </summary>
		/// <param name="isgo">The checking object.</param>
		/// <returns>Returns if the SolarSystem contains the object.</returns>
		public bool HasISGO(IStaticGameObject isgo) {
			return isgoObjectDict.ContainsKey(isgo.Name);
		}

		/// <summary>
		/// Check if SolarSystem contains the IMovableGameObject.
		/// </summary>
		/// <param name="imgo">The checking object.</param>
		/// <returns>Returns if the SolarSystem contains the object.</returns>
		public bool HasIMGO(IMovableGameObject imgo) {
			return imgoObjectDict.ContainsKey(imgo.Name);
		}

		/// <summary>
		/// Inserts the IStaticGameObject to the SolarSystem and sets SolarSystem visibility
		/// to the object. Also checks if the inserting item is Gate, if the object is so sets
		/// the indicator;
		/// </summary>
		/// <param name="isgo">The inserting object.</param>
		public void AddISGO(IStaticGameObject isgo) {
			if (!isgoObjectDict.ContainsKey(isgo.Name)) {
				var gate = isgo as Gate;
				if (gate != null) {
					hasGate = true;
				}
				isgoObjectDict.Add(isgo.Name, isgo);
				isgo.ChangeVisible(active);
			}
		}

		/// <summary>
		/// Inserts the IMovableGameObject to the SolarSystem and sets SolarSystem visibility
		/// to the object. 
		/// </summary>
		/// <param name="imgo">The inserting object.</param>
		public void AddIMGO(IMovableGameObject imgo) {
			if (!imgoObjectDict.ContainsKey(imgo.Name)) {
				imgoObjectDict.Add(imgo.Name, imgo);
				imgo.ChangeVisible(active);
			}
		}

		/// <summary>
		/// Inserts the IStaticGameObjects to the SolarSystem and sets SolarSystem visibility
		/// to the objects.  
		/// </summary>
		/// <param name="listOfISGO">The inserting List of IStaticGameObjects</param>
		public void AddISGO(List<IStaticGameObject> listOfISGO) {
			foreach (IStaticGameObject isgo in listOfISGO) {
				AddISGO(isgo);
				isgo.ChangeVisible(active);
			}
		}

		/// <summary>
		/// Inserts the IMovableGameObject to the SolarSystem and sets SolarSystem visibility
		/// to the objects.  
		/// </summary>
		/// <param name="listOfIMGO">The inserting List of IMovableGameObject</param>
		public void AddIMGO(List<IMovableGameObject> listOfIMGO) {
			foreach (IMovableGameObject imgo in listOfIMGO) {
				AddIMGO(imgo);
				imgo.ChangeVisible(active);
			}
		}

		/// <summary>
		/// Removes the IStaticGameObject from the SolarSystem.
		/// </summary>
		/// <param name="isgo">The removing object.</param>
		public void RemoveISGO(IStaticGameObject isgo) {
			if (isgoObjectDict.ContainsKey(isgo.Name)) {
				isgoObjectDict.Remove(isgo.Name);
			}
		}

		/// <summary>
		/// Removes the IMovableGameObject from the SolarSystem. 
		/// </summary>
		/// <param name="imgo">The removing object.</param>
		public void RemoveIMGO(IMovableGameObject imgo) {
			if (imgoObjectDict.ContainsKey(imgo.Name)) {
				imgoObjectDict.Remove(imgo.Name);
			}
		}

		/// <summary>
		/// Returns all IStaticGameObjects in the SolarSystem.
		/// </summary>
		/// <returns>Returns Dictionary with all IStaticGameObjects in the SolarSystem.</returns>
		public Dictionary<string, IStaticGameObject> GetISGOs() {
			return isgoObjectDict;
		}

		/// <summary>
		/// Returns all IMovableGameObject in the SolarSystem.
		/// </summary>
		/// <returns>Returns Dictionary with all IMovableGameObject in the SolarSystem.</returns>
		public Dictionary<string, IMovableGameObject> GetIMGOs() {
			return imgoObjectDict;
		}

		/// <summary>
		/// Sets the SolarSystem to invisible mode and changes visibility (to false) for each 
		/// object in the SolarSystem.
		/// </summary>
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

		/// <summary>
		/// Sets the SolarSystem to visible mode and changes visibility (to true) for each 
		/// object in the SolarSystem and checks invisible collisions. Also prints the name of current action SolarSystem to 
		/// GUI.
		/// </summary>
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

		/// <summary>
		/// Destroys the SolarSystem and all its members.
		/// </summary>
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
		/// Updates all objects in SolarSystem in visible or invisible mode by the SolarSystem visibility.
		/// Uses shallow copies for updating (object could be removed).
		/// </summary>
		/// <param name="delay">The deley between last two frames</param>
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

		/// <summary>
		/// Represents the name of the SolarSystem. The name is unique.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Represents the position in Space. The property is used for calculation
		/// of distance between SolarSystems.
		/// </summary>
		public Mogre.Vector3 Position {
			get { return position; }
		}

		/// <summary>
		/// Indicates if the SolarSystem has a Gate.
		/// </summary>
		public bool HasGate {
			get { return hasGate; }
		}

		#endregion

		

		private static Random r = new Random();

		/// <summary>
		/// Randomizes the vector for the setted constant (randConst).
		/// </summary>
		/// <param name="v">The randomizing vector.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Sets new positions to members in the collision group.
		/// </summary>
		/// <param name="groupInCollistion">The group with members in collision.</param>
		private void RepairHidenCollision(Dictionary<string, IMovableGameObject> groupInCollistion) {
			Dictionary<Mogre.Vector3, IMovableGameObject> collision = new Dictionary<Mogre.Vector3, IMovableGameObject>();
			// Checks all members in collision and sets them new position.
			foreach (var imgoPair in groupInCollistion) {
				if (collision.ContainsKey(imgoPair.Value.Position)) {
					bool isTaken = true;
					Mogre.Vector3 addVect = imgoPair.Value.Position;
					// Find new empty position.
					while (isTaken) {
						if (!collision.ContainsKey(addVect)) {
							collision.Add(addVect, imgoPair.Value);
							isTaken = false;
						} else {
							addVect = RandomizeVector(addVect);
						}
					}
					imgoPair.Value.JumpToLocation(addVect);
				} else {
					collision.Add(imgoPair.Value.Position, imgoPair.Value);
				}
			}
		}
	}
}
