using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;
using Strategy.GroupControl.RuntimeProperty;

namespace Strategy.GameGUI {
	class PropertyLabel<T> : Label {

		protected Property<T> property;

		public PropertyLabel(Property<T> property)
			: base() {
			this.property = property;
		}

		protected override void UpdateCore() {
			Text = property.Value.ToString();
			base.UpdateCore();
		}

		public void set(T value) {
			property.Value = value;
		}
	}
}
