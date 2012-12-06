﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.FightControl;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.MoveControl;
using Strategy.GameMaterial;
using Strategy.TeamControl;

namespace Strategy.GroupControl {
    class GroupManager {
        protected ObjectCreator objectCreator;
        //TODO: complete IMGO
        //TODO: send ISGO to TeamMgr
        //TODO: Solar System Class or not

        protected Dictionary<int, SolarSystem> solarSystemBetter;
        protected int lastSolarSystem = 0;

        

        private GroupMovables selectedGroup; //not implemented ...will be actual selected group - need rectangular select
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
            
            selectedGroup = null;
            solarSystemBetter = new Dictionary<int, SolarSystem>();
        }
        #endregion

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

        /// <summary>
        /// Show given solar system and hide actual
        /// </summary>
        /// <param name="newSolarSystem">integer of showing solar system</param>
        public void changeSolarSystem(int newSolarSystem) {

            //better system
            solarSystemBetter[activeSolarSystem].hideSolarSystem();
            solarSystemBetter[newSolarSystem].showSolarSystem();
            //end of it

            activeSolarSystem = newSolarSystem; //set new active solar system     
        }


        /// <summary>
        /// inicializetion of world
        /// </summary>
        public void inicializeWorld() { 
            //
            objectCreator.initializeWorld("nameOfMission");
            createSolarSystems();
        }

        public string getSolarSystemName(int numberOfSolarSystem) {
            return solarSystemBetter[numberOfSolarSystem].getName();
        }

        public Dictionary<string,Team> getTeams() {            
            return objectCreator.getTeams();
        }
    }


}
