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
		protected PropertyManager propertyManager;

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

			objectCreator = ObjectCreator.getInstance(manager);
            solarSystemBetter = new Dictionary<int, SolarSystem>();
			moveControler = MoveControler.getInstance();
			propertyManager = new PropertyManager("StartMission");
        }
        #endregion

		public void setGUI(GUIControler gui) {
            guiControler = gui;
        }

        //grupy planet / lodi dle teamu rozdelit
        private void createSolarSystems() {
            //just one solar system and one group
            //switch on team ISGO
            //shout on IMGO

            //inicialization
            List<SolarSystem> sSyst;

            objectCreator.getInicializedObjects(out sSyst);

            foreach(SolarSystem solarSyst in sSyst){
                solarSystemBetter.Add(lastSolarSystem, solarSyst);
                lastSolarSystem++;
            }
        }

        /// <summary>
        /// Called on frame update
        /// </summary>
        /// <param name="f">delay between frames</param>
        public void update(float f) {
            foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemBetter) {
                solarSys.Value.update(f);
            }
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
			
			objectCreator.initializeWorld(missionName, propertyManager);
            createSolarSystems();
        }



        public Dictionary<string,Team> getTeams() {            
            return objectCreator.getTeams();
        }

		public void deselectGroup() {
			GroupMovables groupM = new GroupMovables();
			GroupStatics groupS = new GroupStatics();
			activeMGroup = false;
			guiControler.showTargeted(groupS);
			
		}

		public void selectGroup(List<Mogre.MovableObject> movableList) {
			//first check if is moveble or not
			GroupMovables groupM = new GroupMovables();
			GroupStatics groupS = new GroupStatics();
			if (movableList.Count == 0) {
				activeMGroup = false;
				guiControler.showTargeted(groupS);
				return;
			}
			foreach (var mobleItem in movableList) {
				if (objectCreator.isObjectMovable(mobleItem.Name)) {
					groupM.insertMemeber(objectCreator.getIMGO(mobleItem.Name));
				} else {
					groupS.insertMemeber(objectCreator.getISGO(mobleItem.Name));	
				}
			}
			if (groupM.Count == 0) {
				activeMGroup = false;
				selectedGroupS = groupS;
				guiControler.showTargeted(groupS);
			} else {
				activeMGroup = true;
				guiControler.showTargeted(groupM);
				selectedGroupM = groupM;
			}
		}


		public void leftClick(List<Mogre.MovableObject> selectedObjects) {
			selectGroup(selectedObjects);
		}

		public void rightClick(Mogre.Vector3 clickedPoint) {
			if (activeMGroup) {
				//foreach (IMovableGameObject imgo in selectedGroupM) {
				//	moveControler.goToLocation(imgo, clickedPoint);
				//}
				moveControler.goToLocation(selectedGroupM, clickedPoint);
			}
		}
    }


}
