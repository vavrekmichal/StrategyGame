using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.GameSave {

	public enum AttributeType {
		Vector3,
		Basic,
		Property,
		PropertyVector3
	}

	public class ConstructorFieldAttribute : Attribute {
		public int Order { get; set; }
		public AttributeType Type { get; set; }


		public ConstructorFieldAttribute(int order, AttributeType type) {
			this.Type = type;
			this.Order = order;
		}
	}
}
