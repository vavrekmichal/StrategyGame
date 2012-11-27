﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.FightControl;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.MoveControl;

namespace Strategy.GroupControl {
	class GroupManager {
		protected ObjectCreator objectCreator;

		protected List<GroupMovables> groupListMovable; //group ->group, shout distance
        protected Dictionary<int,GroupStatics> ISGOGroups; //each team has own planets
        protected Dictionary<int,GroupStatics> solarSystem;
        protected List<Sun> suns; //Each SolarSystem has one Sun special type of GroupMovables

		protected IMoveControler moveControler;
		protected IFightManager fightManager;

		private static GroupManager instance;

		private GroupMovables selectedGroup; //not implemented ...will be actual selected group
        private int activeSolarSystem = 0; //now active solarSystem

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <param name="manager">Mogre SceneManager</param>
        /// <returns>instance of GroupManager</returns>
		public static GroupManager getInstance(Mogre.SceneManager manager){
			if (instance==null) {
				instance = new GroupManager(manager);
			}
			return instance;
		}

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="manager">Mogre SceneManager</param>
		private GroupManager(Mogre.SceneManager manager) {

			moveControler = MoveControler.getInstance();
			fightManager = FightManager.getInstance();
			objectCreator = ObjectCreator.getInstance(manager);

			selectedGroup = null;
            ISGOGroups = new Dictionary<int, GroupStatics>();
            solarSystem = new Dictionary<int, GroupStatics>();
            suns = new List<Sun>();

			objectCreator.initializeWorld("nameOfMission");
			
			makeGroups();
		}

		//grupy planet / lodi dle teamu rozdelit
		private void makeGroups() {
			//just one solar system and one group
            //switch on team ISGO
            //shout on IMGO
            
            //inicialization
            List<IMovableGameObject> listOfIMGO;
            List<IStaticGameObject> listOfISGO;
            objectCreator.getInicializedObjects(out listOfISGO, out listOfIMGO);

            GroupMovables g = new GroupMovables();
            GroupStatics f = new GroupStatics();
            //GroupOfSolarSystem
            //SolarSystem active will be 0

            foreach (IMovableGameObject obj in listOfIMGO) {//not implemented
				g.insertMemeber(obj);
                //not just insert but shout while is not empty
			}
			groupListMovable = new List<GroupMovables>();
            groupListMovable.Add(g);
			
            foreach (IStaticGameObject isgo in listOfISGO) {
                //must detect SolarSystem
				if (typeof(Sun) == isgo.GetType()) {
                    suns.Add((Sun)isgo);
				} else {
                    //try insert into st with group of this team
                    insertIntoStaticGroup(isgo);
				}
			}

		}


        private void insertIntoStaticGroup(IStaticGameObject isgo) {
            if (!solarSystem.ContainsKey(isgo.getSolarSystem)) {
                solarSystem.Add(isgo.getSolarSystem, new GroupStatics());
            }

            if (!ISGOGroups.ContainsKey(isgo.team)) {
                ISGOGroups.Add(isgo.team, new GroupStatics());
            }
            solarSystem[isgo.getSolarSystem].insertMemeber(isgo);
            ISGOGroups[isgo.team].insertMemeber(isgo);
        }


        /// <summary>
        /// Called on frame update
        /// </summary>
        /// <param name="f">delay between frames</param>
		public void update(float f) {
            foreach (Sun sun in suns) {
                sun.rotate(f);
            }
			foreach (GroupMovables group in groupListMovable) {
				group.move(f);
			}

        //developing dictionary
            foreach (KeyValuePair<int, GroupStatics> groupStaticPair in solarSystem) {
               
                groupStaticPair.Value.rotate(f,activeSolarSystem);
                
            }
		}


	}


}
