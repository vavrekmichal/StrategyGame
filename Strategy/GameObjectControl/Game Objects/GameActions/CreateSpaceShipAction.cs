using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.Exceptions;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class CreateSpaceShipAction : IGameAction {

		private string creatingObject;
		IGameObject gameObject;
		Dictionary<string, int> neededMaterials;

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

		public void Update(float delay) { }

		public string OnMouseClick() {
			if (gameObject.Team.CheckMaterials(neededMaterials)) {
				gameObject.Team.UseMaterials(neededMaterials);
				var args = new List<object>();
				args.Add(Game.IGameObjectCreator.GetUnusedName("Wolen"));
				args.Add(gameObject.Team);

				string objectPosToString = gameObject.Position.x.ToString() + ';' + gameObject.Position.z.ToString();
				args.Add(new object[] { objectPosToString });

				var solSyst = Game.GroupManager.GetSolarSystem(gameObject);
				var createdGameObject = Game.IGameObjectCreator.CreateImgo(creatingObject, args.ToArray(), solSyst);

				var position = new Mogre.Vector3(100, 0, 100) + gameObject.Position;
				Game.IMoveManager.GoToLocation(createdGameObject, position);
				gameObject.Team.AddIMGO(createdGameObject);

				return creatingObject + " created.";
			} else {
				return creatingObject + " cannot be created, you don't have enough materials.";
			}
		}

		public string IconPath() {
			return "../../media/icons/ship.png";
		}
	}
}
