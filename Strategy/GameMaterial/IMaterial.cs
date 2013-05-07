using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    public interface IMaterial {
  
        double DisplayChangees();
        int GetQuantityOfMaterial();
        bool TryBuild(int wantedQuantity);
        void AddQuantity(double quantity);

        int State {  get; }
        string Name { get; }
    }
}
