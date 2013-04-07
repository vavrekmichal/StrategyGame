using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.FightControl;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.GroupControl.RuntimeProperty;
using Strategy.MoveControl;
using Strategy.GameMaterial;
using Strategy.TeamControl;
using Strategy.GameGUI;


namespace Strategy.GroupControl {
	class GroupManager {
		protected ObjectCreator objectCreator;
		protected GUIControler guiControler;
		protected IMoveControler moveControler;
		protected PropertyManager propertyMgr;
		protected TeamManager teamMgr;
		protected HitTest hitTest;

		protected Dictionary<int, SolarSystem> solarSystemBetter;
		protected int lastSolarSystem = 0;

		public bool activeMGroup; //active is movable group

		private GroupMovables selectedGroupM; //not implemented ...will be actual selected group - need rectangular select
		private GroupStatics selectedGroupS;

		private int activeSolarSystem = 0; //now active solarSystem


		#region singlton and constructor
		private static GroupManager instance;
		/// <summary>
		/// Singleton constructor
		/// </summary>
		/// <param name="manager">Mogre SceneManager</param>
		/// <returns>instance of GroupManager</returns>
		public static GroupManager getInstance(Mogre.SceneManager manager) {
			if (instance == null) {
				instance = new GroupManager(manager);
			}
			return instance;
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		/// <param name="manager">Mogre SceneManager</param>
		private GroupManager(Mogre.SceneManager manager) {
			teamMgr = TeamManager.getInstance();
			objectCreator = ObjectCreator.getInstance(manager);
			solarSystemBetter = new Dictionary<int, SolarSystem>();
			moveControler = MoveControler.getInstance();
			propertyMgr = new PropertyManager("StartMission");
			hitTest = new HitTest();
		}
		#endregion

		public void setGUI(GUIControler gui) {
			guiControler = gui;
		}

		/// <summary>
		/// Get SolarSystem from ObjectCreator as List and creates Dictionary. 
		/// Also initializes HitTest
		/// </summary>
		private void createSolarSystems() {
			//inicialization
			List<SolarSystem> sSyst = objectCreator.getInicializedSolarSystems();

			foreach (SolarSystem solarSyst in sSyst) {
				solarSystemBetter.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
			hitTest.createHitTestMap(sSyst);
		}

		/// <summary>
		/// Called on frame update
		/// </summary>
		/// <param name="delay">delay between frames</param>
		public void update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemBetter) {
				solarSys.Value.update(delay);
			}
			Gate.updateTravelers(delay);
			//teamManager.update();
			moveControler.update(delay);
		}

		#region solarSyst


		/// <summary>
		/// Show given solar system and hide actual
		/// </summary>
		/// <param name="newSolarSystem">integer of showing solar system</param>
		public void changeSolarSystem(int newSolarSystem) {
			//better system
			deselectGroup();
			solarSystemBetter[activeSolarSystem].hideSolarSystem();
			solarSystemBetter[newSolarSystem].showSolarSystem();
			//end of it

			activeSolarSystem = newSolarSystem; //set new active solar system   
			guiControler.setSolarSystemName(getSolarSystemName(activeSolarSystem));
		}

		public List<string> getSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemBetter) {
				list.Add(ss.Value.Name);
			}
			return list;
		}


		public string getSolarSystemName(int numberOfSolarSystem) {
			return solarSystemBetter[numberOfSolarSystem].Name;
		}
		#endregion

		/// <summary>
		/// inicializetion of world
		/// </summary>
		public void inicializeWorld(string missionName) {
			objectCreator.initializeWorld(missionName, propertyMgr);
			createSolarSystems();
			teamMgr.setGUI(guiControler);
			teamMgr.inicialization(objectCreator.getTeams(), objectCreator.getTeamsRelations());
			guiControler.inicialization(teamMgr.playerTeam.getMaterials());
		}


		/// <summary>
		/// Set all select group as new empty
		/// </summary>
		public void deselectGroup() {
			GroupMovables groupM = new GroupMovables();
			GroupStatics groupS = new GroupStatics();
			activeMGroup = false;
			guiControler.showTargeted(groupS);

		}

		/// <summary>
		/// Select targeted object and check if is players or not. Objects are selected by type
		/// (imgo/isgo) and teams. Movable object has bigger prioryty. Fuctions result is selected
		/// movable or static group of object.
		/// </summary>
		/// <param name="movableList">targeted objects</param>
		public void selectGroup(List<Mogre.MovableObject> movableList) {
			//first check if is moveble or not
			GroupMovables groupM;
			Dictionary<string, GroupMovables> selectedIMGOs = new Dictionary<string, GroupMovables>();
			GroupStatics groupS = new GroupStatics();
			string targetedTeam = "";
			if (movableList.Count == 0) {
				activeMGroup = false;
				guiControler.showTargeted(groupS);
				return;
			}
			foreach (var mobleItem in movableList) {
				if (hitTest.isObjectMovable(mobleItem.Name)) {
					IMovableGameObject imgo = hitTest.getIMGO(mobleItem.Name);
					if (selectedIMGOs.ContainsKey(imgo.Team.Name)) {
						selectedIMGOs[imgo.Team.Name].insertMemeber(imgo);
					} else {
						var group = new GroupMovables(imgo.Team);
						group.insertMemeber(imgo);
						selectedIMGOs.Add(imgo.Team.Name, group);
						targetedTeam = imgo.Team.Name;
					}
				} else {
					groupS.insertMemeber(hitTest.getISGO(mobleItem.Name));
				}
			}
			if (selectedIMGOs.Count == 0) {
				activeMGroup = false;
				selectedGroupS = groupS;
				guiControler.showTargeted(groupS);
			} else {
				if (selectedIMGOs.ContainsKey(Game.playerName)) {
					groupM = selectedIMGOs[Game.playerName];
				} else {
					groupM = selectedIMGOs[targetedTeam];
				}
				activeMGroup = true;
				guiControler.showTargeted(groupM);
				selectedGroupM = groupM;
			}
		}

		/// <summary>
		/// Called from GUI when a left mouse button click or a rectangular select in game area 
		/// </summary>
		/// <param name="selectedObjects">objects in clicked area</param>
		public void leftClick(List<Mogre.MovableObject> selectedObjects) {
			selectGroup(selectedObjects);
		}

		/// <summary>
		/// Called from GUI when right mouse button click in game area
		/// </summary>
		/// <param name="clickedPoint">mouse position</param>
		/// <param name="selectedObjects">objects in clicked area</param>
		public void rightClick(Mogre.Vector3 clickedPoint, List<Mogre.MovableObject> selectedObjects) {
			if (activeMGroup) {//TODO rewrite (notebook)
				if (selectedGroupM.OwnerTeam.Name == Game.playerName) {
					Mogre.MovableObject hitObject;
					bool isFriendly = true;
					if (selectedObjects.Count == 0) {
						hitObject = null;
					} else {
						hitObject = selectedObjects[0];
						Team targetTeam;
						if (hitTest.isObjectMovable(hitObject.Name)) {
							targetTeam = hitTest.getIMGO(hitObject.Name).Team;
							
						} else {
							targetTeam = hitTest.getISGO(hitObject.Name).Team;
						}
						isFriendly = teamMgr.areFriendly(selectedGroupM.OwnerTeam, targetTeam);
						
					}
					var answer = selectedGroupM.onMouseAction(ActionReason.onRightButtonClick, clickedPoint, hitObject, isFriendly);
					switch (answer) {
						case ActionAnswer.None:
							break;
						case ActionAnswer.Attack:
							break;
						case ActionAnswer.Move:
							moveControler.goToLocation(selectedGroupM, clickedPoint);
							break;
						case ActionAnswer.MoveTo:
							moveControler.goToTarget(selectedGroupM, hitTest.getISGO(hitObject.Name));
							break;
						case ActionAnswer.RunAway:
							break;
						default:
							break;
					}

				}
			}
		}
	}


}
