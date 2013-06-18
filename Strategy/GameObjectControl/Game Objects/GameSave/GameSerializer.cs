using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Strategy.GameObjectControl.Game_Objects.GameTargets;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MissionControl;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.GameSave {
	public class GameSerializer {
		XElement usedObjectNode;


		public GameSerializer() { }

		public void Initialize(string missionFilePath) {

			XDocument xml = XDocument.Load(missionFilePath);

			usedObjectNode = xml.Descendants("usedObjects").First();
		}

		public void Save(string saveName) {
			var document = new XDocument();
			var rootElement = new XElement("mission", new XAttribute("name", saveName.Substring(0, saveName.Length - 5)));
			document.Add(rootElement);
			rootElement.Add(usedObjectNode);

			// Saves all SolarSystems
			SerializeSolarSystems(rootElement, Game.SolarSystemManager.GetSolarSystems());

			// Saves teams materials
			SerializeMaterials(rootElement, Game.TeamManager.GetTeams());

			// Saves mission
			SerializeMission(rootElement, Game.Mission);

			document.Save(Game.SavesGamePath + '/' + saveName);
		}

		#region Serializators

		/// <summary>
		/// Serializes the current Mission (all current ITargets).
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="mission">The serializing Mission with all current targets.</param>
		private void SerializeMission(XElement rootElement, Mission mission) {
			var element = new XElement("missionTargets");
			rootElement.Add(element);
			foreach (var target in mission.GetTargets()) {
				SerializeITarget(element, target);
			}
		}

		/// <summary>
		/// Serializes ITarget (name and .
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="target">The serializing ITarget.</param>
		private void SerializeITarget(XElement rootElement, ITarget target) {
			var element = new XElement("target", new XAttribute("name", target.GetType().ToString().Split('.').Last()));
			rootElement.Add(element);
		}

		/// <summary>
		/// Serializes materials of the all Teams in the game.
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="teamDict">The dictionary with all Teams.</param>
		private void SerializeMaterials(XElement rootElement, Dictionary<string, Team> teamDict) {
			var element = new XElement("materials");
			rootElement.Add(element);
			foreach (var team in teamDict) {
				SerializeTeamMaterials(element, team.Value);
			}
		}

		/// <summary>
		/// Serializes materials of the given Team (name, team and quantity).
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="team">The serializing materials of the given Team.</param>
		private void SerializeTeamMaterials(XElement rootElement, Team team) {
			foreach (var material in team.GetMaterials()) {
				var element = new XElement("material",
					new XAttribute("name", material.Value.Name),
					new XAttribute("team", team.Name));
				SerializeArgument(element, material.Value.State.ToString());
				rootElement.Add(element);
			}
		}

		/// <summary>
		/// Serializes the given argument.
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="argument">The serializing value.</param>
		private void SerializeArgument(XElement rootElement, string argument) {
			var element = new XElement("argument",argument);
			rootElement.Add(element);
		}

		/// <summary>
		/// Serializes all SolarSystems members.
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="solarSystemDict">The serializing SolarSystems.</param>
		private void SerializeSolarSystems(XElement rootElement, Dictionary<int, SolarSystem> solarSystemDict) {

			var element = new XElement("solarSystems");
			rootElement.Add(element);

			foreach (var solarSystem in solarSystemDict) {
				SerializeSolarSystem(element, solarSystem.Value);
			}
		}

		/// <summary>
		/// Serializes the given SolarSystem (name,position and if has gate).
		/// Different serialization for the sun and others. (Sun doesn't have position).
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="solarSystem">The serializing SolarSystem.</param>
		private void SerializeSolarSystem(XElement rootElement, SolarSystem solarSystem) {

			var element = new XElement("solarSystem", new XAttribute("name", solarSystem.Name),
				new XAttribute("position", CreateSerializableVector3(solarSystem.Position)));
			if (solarSystem.HasGate) {
				element.Add(new XAttribute("gate", solarSystem.HasGate));
			}

			// Saves Sun
			if (solarSystem.Sun != null) {
				SerializeSun(element, solarSystem.Sun);
			}

			// Saves IStaticGameObjects
			foreach (var isgo in solarSystem.GetISGOs()) {
				if (!(isgo.Value is Gate)) {
					SerializeIGameObject(element, isgo.Value);
				}
			}

			// Saves IMovableGameObjects
			foreach (var imgo in solarSystem.GetIMGOs()) {
				SerializeIGameObject(element, imgo.Value);
			}
			rootElement.Add(element);
		}

		/// <summary>
		/// Serializes the given Sun (name and type).
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="sun">The serializing Sun.</param>
		private void SerializeSun(XElement rootElement, IStaticGameObject sun) {
			var element = new XElement("isgo",
				new XAttribute("name", sun.Name),
				new XAttribute("type", "Sun"));
			rootElement.Add(element);
		}

		/// <summary>
		/// Serializes the given IGameObject (name, type, team).
		/// </summary>
		/// <param name="rootElement">The parent element.</param>
		/// <param name="gameObject">The serializing IGameObject.</param>
		private void SerializeIGameObject(XElement rootElement, IGameObject gameObject) {
			var element = new XElement("isgo",
				new XAttribute("name", gameObject.Name),
				new XAttribute("type", gameObject.GetType().ToString().Split('.').Last()),
				new XAttribute("team", gameObject.Team)
				);
			rootElement.Add(element);
			Console.WriteLine("Saving " + gameObject.Name);
		}

		/// <summary>
		/// Converts the vector into serializable form.
		/// </summary>
		/// <param name="vector">The converting Vector3.</param>
		/// <returns>The converted vector.</returns>
		private string CreateSerializableVector3(Mogre.Vector3 vector) {
			return vector.x.ToString() + ';' + vector.y + ';' + vector.z;
		}

		#endregion


	}
}
