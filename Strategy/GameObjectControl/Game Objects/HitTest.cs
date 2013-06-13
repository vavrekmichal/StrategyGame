using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects {
	/// <summary>
	/// Has information about all game objects. Could return information about a object by name 
	/// (because name must be unique), information if the object is movable, static or uncotrollable.
	/// </summary>
	public class HitTest {

		protected Dictionary<string, bool> objectIsMovable;
		protected Dictionary<string, IStaticGameObject> isgoDict;
		protected Dictionary<string, IMovableGameObject> imgoDict;

		/// <summary>
		/// Initializes dictionaries.
		/// </summary>
		public HitTest() {
			objectIsMovable = new Dictionary<string, bool>();
			isgoDict = new Dictionary<string, IStaticGameObject>();
			imgoDict = new Dictionary<string, IMovableGameObject>();
		}

		#region Public
		/// <summary>
		/// Checks if the object is contollable (the object is in objectIsMovable dictionary).
		/// </summary>
		/// <param name="name">The name of the controlling objet.</param>
		/// <returns>Returns if the object is controllable.</returns>
		public bool IsObjectControllable(string name) {
			return objectIsMovable.ContainsKey(name);
		}

		/// <summary>
		/// Checks if the object is movable (the object value in objectIsMovable is true).
		/// </summary>
		/// <param name="name">Returns if the object is movable.</param>
		/// <returns></returns>
		public bool IsObjectMovable(string name) {
			return objectIsMovable[name];
		}

		/// <summary>
		/// Finds a game object by given name. Hittest has reference to all movable and static 
		/// objects in imgoDict and isgoDict.
		/// </summary>
		/// <param name="name">The name of the finding object.</param>
		/// <returns>Returns founded object by the name.</returns>
		public IGameObject GetGameObject(string name) {
			if (imgoDict.ContainsKey(name)) {
				return imgoDict[name];
			} else {
				return isgoDict[name];
			}
		}

		/// <summary>
		/// Returns IMovableGameObject from a imgoDict dictionary.
		/// </summary>
		/// <param name="name">The name of searched object.</param>
		/// <returns>Returns founded IMovableGameObject.</returns>
		public IMovableGameObject GetIMGO(string name) {
			return imgoDict[name];
		}

		/// <summary>
		/// Returns IStaticGameObject from a isgoDict dictionary.
		/// </summary>
		/// <param name="name">The name of searched object.</param>
		/// <returns>Returns founded IStaticGameObject.</returns>
		public IStaticGameObject GetISGO(string name) {
			return isgoDict[name];
		}

		/// <summary>
		/// Initializas the HitTest. Sorts game objects to ISGO and IMGO dictionaries (isgoDict, imgoDict).
		/// </summary>
		/// <param Name="solarSystems">The List with all solarSystems.</param>
		public void CreateHitTestMap(List<SolarSystem> solarSystems) {
			objectIsMovable = new Dictionary<string, bool>();
			isgoDict = new Dictionary<string, IStaticGameObject>();
			imgoDict = new Dictionary<string, IMovableGameObject>();
			foreach (SolarSystem ss in solarSystems) {
				IStaticGameObject s = ss.Sun;
				if (s != null) {
					objectIsMovable.Add(s.Name, false);
					isgoDict.Add(s.Name, s);
				}

				foreach (var isgoPair in ss.GetISGOs()) {
					objectIsMovable.Add(isgoPair.Key, false);
					isgoDict.Add(isgoPair.Key, isgoPair.Value);
				}

				foreach (var imgoPair in ss.GetIMGOs()) {
					objectIsMovable.Add(imgoPair.Key, true);
					imgoDict.Add(imgoPair.Key, imgoPair.Value);
				}
			}
		}

		/// <summary>
		/// Adds new object to isgoDict and also adds to objectIsMovable as non-movable (false).
		/// </summary>
		/// <param name="isgo">The inserting object.</param>
		public void RegisterISGO(IStaticGameObject isgo) {
			objectIsMovable.Add(isgo.Name, false);
			isgoDict.Add(isgo.Name, isgo);
		}

		/// <summary>
		/// Adds new object to imgoDict and also adds to objectIsMovable as movable (true).
		/// </summary>
		/// <param name="imgo">The inserting object.</param>
		public void RegisterIMGO(IMovableGameObject imgo) {
			objectIsMovable.Add(imgo.Name, true);
			imgoDict.Add(imgo.Name, imgo);
		}
		#endregion
	}
}
