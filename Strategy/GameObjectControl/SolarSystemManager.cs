using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.GameObjectControl {
	public class SolarSystemManager {

		protected Dictionary<int, SolarSystem> solarSystemDict;
		protected int lastSolarSystem = 0;
		private int activeSolarSystem = 0; // Now active solarSystem

		public SolarSystemManager() {
			solarSystemDict = new Dictionary<int, SolarSystem>();
		}

		/// <summary>
		/// Updates all solar systems in solarSys dictionary.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemDict) {
				solarSys.Value.Update(delay);
			}
			Gate.UpdateTravelers(delay);
		}

		/// <summary>
		/// Stores given solar systems to solarSystemDict.
		/// </summary>
		public void CreateSolarSystems(List<SolarSystem> solSystList) {
			foreach (SolarSystem solarSyst in solSystList) {
				solarSystemDict.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
		}

		/// <summary>
		/// Finds objects SolarSystem and removes it from the SolarSystem.
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
		/// Finds objects SolarSystem and removes it from the SolarSystem.
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


		public int GetSolarSystemsNumber(IGameObject igo) {
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


		/// <summary>
		/// Show given solar system and hide actual
		/// </summary>
		/// <param Name="newSolarSystem">Integer of showing solar system</param>
		public void ChangeSolarSystem(int newSolarSystem) {

			solarSystemDict[activeSolarSystem].HideSolarSystem();
			solarSystemDict[newSolarSystem].ShowSolarSystem();

			activeSolarSystem = newSolarSystem; // Set new active solar system  
			Game.GroupManager.DeselectGroup();

		}

		public List<string> GetAllSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemDict) {
				list.Add(ss.Value.Name);
			}
			return list;
		}


		public string GetSolarSystemName(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem].Name;
		}

		public SolarSystem GetActiveSolarSystem() {
			return solarSystemDict[activeSolarSystem];
		}

		public SolarSystem GetSolarSystem(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem];
		}

		public SolarSystem GetSolarSystem(IGameObject igo) {
			var number = GetSolarSystemsNumber(igo);
			if (number == -1) {
				return null;
			}
			return solarSystemDict[number];
		}


		public void CreateTraveler(int solarSystemNumberTo, object imgo) {
			Gate.CreateTraveler(solarSystemDict[activeSolarSystem], GetSolarSystem(solarSystemNumberTo), imgo);
		}


		public List<Traveler> GetTravelers() {
			return Gate.GetTravelers();
		}

	}
}
