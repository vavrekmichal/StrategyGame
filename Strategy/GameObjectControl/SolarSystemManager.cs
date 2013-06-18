using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl {
	/// <summary>
	/// Controls and updates game solar systems (SolarSystem).
	/// </summary>
	public class SolarSystemManager {

		protected Dictionary<int, SolarSystem> solarSystemDict;
		protected int lastSolarSystem = 0;
		private int activeSolarSystem = 0; // Now active solarSystem

		/// <summary>
		/// Initialiaze SolarSystemManager (not initialize SolarSystems).
		/// </summary>
		public SolarSystemManager() {
			solarSystemDict = new Dictionary<int, SolarSystem>();
		}

		/// <summary>
		/// Updates all solar systems in solarSys dictionary and travelers between SolarSystems.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemDict) {
				solarSys.Value.Update(delay);
			}
			Gate.UpdateTravelers(delay);
		}

		/// <summary>
		/// Stores given solar systems to SolarSystemManager (solarSystemDict).
		/// </summary>
		public void CreateSolarSystems(List<SolarSystem> solSystList) {
			foreach (SolarSystem solarSyst in solSystList) {
				solarSystemDict.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
		}

		/// <summary>
		/// Finds IStaticGameObject at SolarSystems and removes it from the one.
		/// </summary>
		/// <param name="isgo">The removing object.</param>
		public void RemoveObjectFromSolarSystem(IStaticGameObject isgo) {
			foreach (var solSysPair in solarSystemDict) {
				if (solSysPair.Value.HasISGO(isgo)) {
					solSysPair.Value.RemoveISGO(isgo);
					break;
				}
			}
		}

		/// <summary>
		/// Finds IMovableGameObject at SolarSystems and removes it from the one.
		/// </summary>
		/// <param name="imgo">The removing object.</param>
		public void RemoveObjectFromSolarSystem(IMovableGameObject imgo) {
			foreach (var solSysPair in solarSystemDict) {
				if (solSysPair.Value.HasIMGO(imgo)) {
					solSysPair.Value.RemoveIMGO(imgo);
					break;
				}
			}
		}

		/// <summary>
		/// Shows SolarSystem by the given number and hide the current one.
		/// </summary>
		/// <param Name="newSolarSystem">The number of showing SolarSystem.</param>
		public void ChangeSolarSystem(int newSolarSystem) {

			solarSystemDict[activeSolarSystem].HideSolarSystem();
			solarSystemDict[newSolarSystem].ShowSolarSystem();
			// Set new active solar system  
			activeSolarSystem = newSolarSystem;
			// Untarget the targeted group.
			Game.GroupManager.UntargetGroup();

		}

		/// <summary>
		/// Returns the list with the names of all SolarSystems.
		/// </summary>
		/// <returns>Returns list with the names of all SolarSystems.</returns>
		public List<string> GetAllSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemDict) {
				list.Add(ss.Value.Name);
			}
			return list;
		}

		/// <summary>
		/// Returns the name of a SolarSystem by given number.
		/// </summary>
		/// <param name="numberOfSolarSystem">The number of a SolarSystem.</param>
		/// <returns>Returns the name of a SolarSystem by given number.</returns>
		public string GetSolarSystemName(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem].Name;
		}

		/// <summary>
		/// Returns the active SolarSystem.
		/// </summary>
		/// <returns>Returns the active SolarSystem.</returns>
		public SolarSystem GetActiveSolarSystem() {
			return solarSystemDict[activeSolarSystem];
		}

		/// <summary>
		/// Returns a SolarSystem by the given number.
		/// </summary>
		/// <param name="numberOfSolarSystem">The number of the SolarSystem.</param>
		/// <returns>Returns a SolarSystem by tge given number.</returns>
		public SolarSystem GetSolarSystem(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem];
		}

		/// <summary>
		/// Returns a SolarSystem by the given game object. If the SolarSystem is not 
		/// found so returns null.
		/// </summary>
		/// <param name="igo">The game object according to which searches.</param>
		/// <returns>Returns a SolarSystem by the given game object.</returns>
		public SolarSystem GetSolarSystem(IGameObject igo) {
			var number = GetSolarSystemsNumber(igo);
			if (number == -1) {
				return null;
			}
			return solarSystemDict[number];
		}

		/// <summary>
		/// Returns all currently created SolarSystems.
		/// </summary>
		/// <returns>Returns the dictionary with all SolarSystem.</returns>
		public Dictionary<int, SolarSystem> GetSolarSystems() {
			return new Dictionary<int,SolarSystem>(solarSystemDict);
		}

		/// <summary>
		/// Creates a travler between SolarSystems.
		/// </summary>
		/// <param name="solarSystemNumberTo">The number of the SolarSystem into which travels.</param>
		/// <param name="imgo">The object which travels.</param>
		public void CreateTraveler(int solarSystemNumberTo, object imgo) {
			Gate.CreateTraveler(solarSystemDict[activeSolarSystem], GetSolarSystem(solarSystemNumberTo), imgo);
		}

		/// <summary>
		/// Returns all current travelers.
		/// </summary>
		/// <returns>Return the List with travelers.</returns>
		public List<Traveler> GetTravelers() {
			return Gate.GetTravelers();
		}

		/// <summary>
		/// Returns the number of a SolarSystem which contains the game object.
		/// </summary>
		/// <param name="igo">The game object according to which searches.</param>
		/// <returns>Returns the number of the SolarSystem which contains the object or -1.</returns>
		private int GetSolarSystemsNumber(IGameObject igo) {
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				for (int i = 0; i < solarSystemDict.Count; i++) {
					if (solarSystemDict[i].HasIMGO(imgo)) {
						return i;
					}
				}
			} else {
				var isgo = igo as IStaticGameObject;
				for (int i = 0; i < solarSystemDict.Count; i++) {
					if (solarSystemDict[i].HasISGO(isgo)) {
						return i;
					}
				}
			}

			return -1;
		}

	}
}
