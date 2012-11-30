using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    interface IMaterial {
        void registerProducer(IStaticGameObject isgo);
        void produce(float delay);
        double displayChangees();
        int getQuantityOfMaterial();
        bool tryBuild(int wantedQuantity);
     

        string name { get; }
    }
}
