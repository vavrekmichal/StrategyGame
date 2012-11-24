using System;
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
		protected List<GroupStatics> groupListStatic;
		protected List<GroupMovables> groupListMovable;
		protected IMoveControler moveControler;
		protected IFightManager fightManager;

		private static GroupManager instance;

		private GroupMovables selectedGroup;

		public static GroupManager getInstance(Mogre.SceneManager manager){
			if (instance==null) {
				instance = new GroupManager(manager);
			}
			return instance;
		}

		private GroupManager(Mogre.SceneManager manager) {

			moveControler = MoveControler.getInstance();
			fightManager = FightManager.getInstance();
			objectCreator = ObjectCreator.getInstance(manager);

			selectedGroup = null;
			groupListStatic = new List<GroupStatics>();

			objectCreator.initializeWorld("");
			List<IMovableGameObject> listOfIMGO;
			List<IStaticGameObject> listOfISGO;
			objectCreator.getInicializedObjects(out listOfISGO, out listOfIMGO);
			makeGroups(listOfISGO, listOfIMGO, out groupListStatic, out groupListMovable);
		}

		//grupy planet / lodi dle teamu rozdelit
		private void makeGroups(List<IStaticGameObject> s, List<IMovableGameObject> m, out List<GroupStatics> gS, out List<GroupMovables> gM) {
			//just one solar system
            GroupMovables g = new GroupMovables();

			foreach (IMovableGameObject obj in m){
				g.insertMemeber(obj);
			}
			gM = new List<GroupMovables>();

			GroupStatics f = new GroupStatics();
			foreach (IStaticGameObject obj in s) {
				if (typeof(Sun) == obj.GetType()) {
					f.setSun((Sun)obj);
				} else {
					f.insertMemeber(obj);
				}
			}
			gS = new List<GroupStatics>();
			gS.Add(f);
		}

		public void update(float f) {
			foreach (GroupStatics group in groupListStatic) {
				group.rotate(f);
			}
			foreach (GroupMovables group in groupListMovable) {
				group.move(f);
			}
		}


	}


}
