using System.Collections.Generic;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameMaterial;
using Strategy.Exceptions;

namespace Strategy.TeamControl {
	/// <summary>
	/// Represents the team in the game and brings together game objects into teams. 
	/// Also ensures the production of materials.
	/// </summary>
	public class Team {

		protected Dictionary<string, IMaterial> materialsStates;

		protected List<IMovableGameObject> imgoObjects;
		protected List<IStaticGameObject> isgoObjects;
		protected string name;

		/// <summary>
		/// Creates the instance of the team with unique name (checks in XML file).
		/// </summary>
		/// <param name="name">The unique name of the team.</param>
		public Team(string name) {
			this.name = name;
			materialsStates = new Dictionary<string, IMaterial>();
			imgoObjects = new List<IMovableGameObject>();
			isgoObjects = new List<IStaticGameObject>();
		}

		/// <summary>
		/// Inserts the given static object to the team and sets its the team.
		/// </summary>
		/// <param name="isgo">The inserting static object.</param>
		public void AddISGO(IStaticGameObject isgo) {
			if (!isgoObjects.Contains(isgo)) {
				isgoObjects.Add(isgo);
				isgo.Team = this;
			}
		}

		/// <summary>
		/// Removes the given static object from the team.
		/// </summary>
		/// <param name="isgo">The removing static object.</param>
		public void RemoveISGO(IStaticGameObject isgo) {
			if (isgoObjects.Contains(isgo)) {
				isgoObjects.Remove(isgo);
			}
		}

		/// <summary>
		/// Inserts the given movable object to the team and sets its the team.
		/// </summary>
		/// <param name="imgo">The inserting movable object.</param>
		public void AddIMGO(IMovableGameObject imgo) {
			if (!imgoObjects.Contains(imgo)) {
				imgoObjects.Add(imgo);
				imgo.Team = this;
			}
		}

		/// <summary>
		/// Removes the given movable object from the team. 
		/// </summary>
		/// <param name="imgo">The removing movable object.</param>
		public void RemoveIMGO(IMovableGameObject imgo) {
			if (imgoObjects.Contains(imgo)) {
				imgoObjects.Remove(imgo);
			}
		}

		/// <summary>
		/// Returns the unique team name which represents team owner.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Returns the number of team members (moveble and static).
		/// </summary>
		public int Count {
			get {
				return imgoObjects.Count + isgoObjects.Count;
			}
		}

		/// <summary>
		/// Just returns the name of the Team.
		/// </summary>
		/// <returns>Returns the name of the Team.</returns>
		public override string ToString() {
			return Name;
		}

		/// <summary>
		/// Checks if the team has enough of the given materials. Controls all materials in the given dictionary
		/// and if any material has not enough, so returns false.
		/// </summary>
		/// <param name="materialNeededDict">The dictionary with required materials.</param>
		/// <returns>Returns if the team has enough materials.</returns>
		public bool CheckMaterials(Dictionary<string, int> materialNeededDict) {
			foreach (var materialPair in materialNeededDict) {
				if (materialPair.Value <= 0) {
					continue;
				}
				if (!materialsStates.ContainsKey(materialPair.Key) ||
					materialsStates[materialPair.Key].GetQuantityOfMaterial().Value < materialPair.Value) {
					// Team does not have the necessary quantity of material 
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Removes the given number of materials (in the dictionary). If the team has not enough, so throws the exception.
		/// </summary>
		/// <param name="materialNeededDict"></param>
		public void UseMaterials(Dictionary<string, int> materialNeededDict) {
			foreach (var materialPair in materialNeededDict) {
				if (materialPair.Value <= 0) {
					continue;
				}
				if (!materialsStates.ContainsKey(materialPair.Key) ||
					materialsStates[materialPair.Key].GetQuantityOfMaterial().Value < materialPair.Value) {
					// Team does not have the necessary quantity of material 
					throw new MissingMaterialException("You can not use " + materialsStates[materialPair.Key].Name + ", because you do not have enough material.");
				}
				materialsStates[materialPair.Key].GetQuantityOfMaterial().Value -= materialPair.Value;
			}
		}

		/// <summary>
		/// Creates the given quantity of the given material. If the team doesn't have the material, 
		/// so creates it and if it is the player's team so updates the GUI material box.
		/// </summary>
		/// <param name="material">The producing material.</param>
		/// <param name="quantity">The quantity of the material.</param>
		public void Produce(string material, double quantity) {
			if (!materialsStates.ContainsKey(material)) {
				materialsStates.Add(material, new GameMaterial.Matrial(material));
				if (name == Game.PlayerName) {
					Game.IGameGUI.UpdatePlayerMaterialDict(materialsStates);
				}
			}
			materialsStates[material].AddQuantity(quantity);

		}

		/// <summary>
		/// Returns the reference to the dictionary with all team materials.
		/// </summary>
		/// <returns>Returns the reference to the dictionary with all team materials.</returns>
		public Dictionary<string, IMaterial> GetMaterials() {
			return materialsStates;
		}
	}
}
