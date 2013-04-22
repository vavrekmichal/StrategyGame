using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public enum PropertyEnum {
		Attack,
		Deffence,
		Hp,
		Speed,
		Rotate,
		PickUp,
		Team,
		AttackBonus = Attack + 50,
		DeffenceBonus = Deffence + 50,
		SpeedBonus = Speed + 50,

		UserDefined1,
		UserDefined2,
		UserDefined3
	}
}
