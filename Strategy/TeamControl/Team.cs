using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameMaterial;

namespace Strategy.TeamControl {
    public class Team {

        protected Dictionary<string, IMaterial> materialsStates;

        protected List<IMovableGameObject> imgoObjects;
        protected List<IStaticGameObject> isgoObjects;
        protected string name;


        public Team(string name) {
            this.name = name;
            materialsStates = new Dictionary<string, IMaterial>();
            imgoObjects = new List<IMovableGameObject>();
            isgoObjects = new List<IStaticGameObject>();
        }

        public Team(string name, List<IMaterial> materialList) {
            this.name = name;
            materialsStates = new Dictionary<string, IMaterial>();
            imgoObjects = new List<IMovableGameObject>();
            isgoObjects = new List<IStaticGameObject>();
            SetMaterials(materialList);
        }

        // Isgo
        public void AddISGO(IStaticGameObject isgo) {
            if (!isgoObjects.Contains(isgo)) {
                isgoObjects.Add(isgo);
				isgo.Team = this;
            }        
        }

        public void RemoveISGO(IStaticGameObject isgo) {
            if (isgoObjects.Contains(isgo)) {
                isgoObjects.Remove(isgo);
            }   
        }

        // Imgo
        public void AddIMGO(IMovableGameObject imgo) {
            if (!imgoObjects.Contains(imgo)) {
                imgoObjects.Add(imgo);
				imgo.Team = this;
            }
        }

        public void RemoveIMGO(IMovableGameObject imgo) {
            if (imgoObjects.Contains(imgo)) {
                imgoObjects.Remove(imgo);
            }
        }

        // Others
        public string Name {
			get { return name; }
        }

        public string Print() {
            StringBuilder s = new StringBuilder();
            s.Append(name+"\n");
            s.Append("\t ISGO" + "\n");
            foreach (IStaticGameObject isgo in isgoObjects) {
                s.Append("\t\t"+isgo.Name+"\n");
            }

            s.Append("\t IMGO" + "\n");
            foreach (IMovableGameObject imgo in imgoObjects) {
				s.Append("\t\t" + imgo.Name + "\n");
            }

            return s.ToString();
        }

		public override string ToString() {
			return Name;
		}

        private void SetMaterials(List<IMaterial> materials) {
            foreach (IMaterial mat in materials) {
                materialsStates.Add(mat.Name, mat);
            }
        }

		private void AddMaterial(string materialName, double materialQuantity) { //TODO divne
			materialsStates[materialName].AddQuantity(materialQuantity);
		}

		public void Produce(string material, double quantity) {
			if (materialsStates.ContainsKey(material)) {
				materialsStates[material].AddQuantity(quantity);
			} else {
				materialsStates.Add(material, new GameMaterial.Matrial(material));
				if (name==Game.playerName) {
					Game.IGameGUI.UpdatePlayerMaterialDict(materialsStates);
				}
			}
		}

        public Dictionary<string, IMaterial> GetMaterials() {
            return materialsStates;
        }
    }
}
