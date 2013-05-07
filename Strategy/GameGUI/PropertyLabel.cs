using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameGUI {
	class PropertyLabel<T> : Label {

		protected Property<T> property;
		protected string text;

		public PropertyLabel(Property<T> property, string text)
			: base() {
			this.text = text + " ";
			this.property = property;
		}

		protected override void UpdateCore() {
			Text = text + property.Value.ToString();
			base.UpdateCore();
		}

		public void Set(T value) {
			property.Value = value;
		}
	}
}
