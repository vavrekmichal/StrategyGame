using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameMaterial;

namespace Strategy.TeamControl {
    public class Team {

        //TODO: Production 
        //Ideas List<IMateria> Netreba jen jmeno...v sesite popsano.
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
            setMaterials(materialList);
        }

        //isgo
        public void addISGO(IStaticGameObject isgo) {
            if (!isgoObjects.Contains(isgo)) {
                isgoObjects.Add(isgo);   
            }        
        }

        public void removeISGO(IStaticGameObject isgo) {
            if (isgoObjects.Contains(isgo)) {
                isgoObjects.Remove(isgo);
            }   
        }

        //imgo
        public void addIMGO(IMovableGameObject imgo) {
            if (!imgoObjects.Contains(imgo)) {
                imgoObjects.Add(imgo);
            }
        }

        public void removeIMGO(IMovableGameObject imgo) {
            if (imgoObjects.Contains(imgo)) {
                imgoObjects.Remove(imgo);
            }
        }

        //others
        public string getName() {
            return name;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(name+"\n");
            s.Append("\t ISGO" + "\n");
            foreach (IStaticGameObject isgo in isgoObjects) {
                s.Append("\t\t"+isgo.getName()+"\n");
            }

            s.Append("\t IMGO" + "\n");
            foreach (IMovableGameObject imgo in imgoObjects) {
                s.Append("\t\t" + imgo.getName() + "\n");
            }

            return s.ToString();
        }

        public void setMaterials(List<IMaterial> materials) {
            foreach (IMaterial mat in materials) {
                materialsStates.Add(mat.name, mat);
            }
        }

        public void addMaterial(string materialName, double materialQuantity){
            materialsStates[materialName].addQuantity(materialQuantity);
        }


        public Dictionary<string, IMaterial> getMaterials() {
            return materialsStates;
        }
    }
}
