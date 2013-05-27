using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameMaterial {
    public interface IMaterial {
  
        double DisplayChangees();
        Property<int> GetQuantityOfMaterial();
        bool TryBuild(int wantedQuantity);
        void AddQuantity(double quantity);

        int State {  get; }
        string Name { get; }
    }
}
