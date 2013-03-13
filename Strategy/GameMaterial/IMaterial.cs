using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    public interface IMaterial {
  
        double displayChangees();
        int getQuantityOfMaterial();
        bool tryBuild(int wantedQuantity);
        void addQuantity(double quantity);

        int state {  get; }
        string name { get; }
    }
}
