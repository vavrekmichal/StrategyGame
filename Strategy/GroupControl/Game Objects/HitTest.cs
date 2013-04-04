﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GroupControl.Game_Objects {
	class HitTest {

		protected Dictionary<string, bool> objectIsMovable;
		protected Dictionary<string, IStaticGameObject> isgoDict;
		protected Dictionary<string, IMovableGameObject> imgoDict;

		public HitTest() {
			objectIsMovable = new Dictionary<string, bool>();
			isgoDict = new Dictionary<string, IStaticGameObject>();
			imgoDict = new Dictionary<string, IMovableGameObject>();
		}

		public bool isObjectMovable(string name) {
			return objectIsMovable[name];
		}

		public IMovableGameObject getIMGO(string name) {
			return imgoDict[name];
		}

		public IStaticGameObject getISGO(string name) {
			return isgoDict[name];
		}

		public void createHitTestMap(List<SolarSystem> solarSystems) {
			objectIsMovable = new Dictionary<string, bool>();
			isgoDict = new Dictionary<string, IStaticGameObject>();
			imgoDict = new Dictionary<string, IMovableGameObject>();
			foreach (SolarSystem ss in solarSystems) {
				IStaticGameObject s = ss.getSun();
				if (s != null) {
					objectIsMovable.Add(s.Name, false);
					isgoDict.Add(s.Name, s);
				}

				foreach (IStaticGameObject isgo in ss.getISGOs()) {
					objectIsMovable.Add(isgo.Name, false);
					isgoDict.Add(isgo.Name, isgo);
				}

				foreach (IMovableGameObject imgo in ss.getIMGOs()) {
					objectIsMovable.Add(imgo.Name, true);
					imgoDict.Add(imgo.Name, imgo);
				}
			}
		}
	}
}