using System;
using System.Collections.Generic;
using System.Linq;
using Strategy.Exceptions;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Tries to make a new IMovableGameObject. Needs odd number of arguments (creating type, 
	/// [name of the material, quantity of the material]).
	/// </summary>
	class CreateSpaceShipAction : IGameAction {

		private string creatingObject;
		IGameObject gameObject;
		Dictionary<string, int> neededMaterials;

		/// <summary>
		/// Initializes game action. Parses given argument (if are invalid so throw exception)
		/// </summary>
		/// <param name="gameObject">The IGameAction owner.</param>
		/// <param name="args">The arguments (creating type, [name of the material, quantity of the material])</param>
		public CreateSpaceShipAction(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
			neededMaterials = new Dictionary<string, int>();
			creatingObject = (string)args[0];
			if (args.Count() % 2 == 0) {
				throw new XmlLoadException("Wrong number of parameters for IGameAction CreateSpaceShipAction");
			}
			for (int i = 1; i < args.Count(); i = i + 2) {
				neededMaterials.Add((string)args[i], Int32.Parse((string)args[i + 1]));
			}

		}

		/// <summary>
		/// Does nothing on Update.
		/// </summary>
		/// <param name="delay">The delay between last to frames.</param>
		public void Update(float delay) { }

		/// <summary>
		/// Checks if owner team has enough of the materials. If team has enough, so the game action
		/// creates new object and send it to a position (Vector3(100, 0, 100) + gameObject.Position) and returns
		/// text that the creation was successful. Else returns text that the creation failed.
		/// </summary>
		/// <returns>Returns text about the creation.</returns>
		public string OnMouseClick() {
			if (gameObject.Team.CheckMaterials(neededMaterials)) {
				// Removes team material
				gameObject.Team.UseMaterials(neededMaterials);

				// Creates arguments of the creation
				var args = new List<object>();
				args.Add(Game.IGameObjectCreator.GetUnusedName("Wolen"));
				args.Add(gameObject.Team);

				string objectPosToString = gameObject.Position.x.ToString() + ';' + gameObject.Position.z.ToString();
				args.Add(new object[] { objectPosToString });

				var solSyst = Game.GroupManager.GetSolarSystem(gameObject);
				
				// Creates new object.
				var createdGameObject = Game.IGameObjectCreator.CreateImgo(creatingObject, args.ToArray(), solSyst);

				// Sends new object to its position.
				var position = new Mogre.Vector3(100, 0, 100) + gameObject.Position;
				Game.IMoveManager.GoToLocation(createdGameObject, position);
				gameObject.Team.AddIMGO(createdGameObject);

				return creatingObject + " created.";
			} else {
				return creatingObject + " cannot be created, you don't have enough materials.";
			}
		}


		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/ship.png";
		}
	}
}
