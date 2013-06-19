using Miyagi.UI.Controls;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the Label which loads text from given Property.
	/// </summary>
	/// <typeparam name="T">The type of the Property</typeparam>
	class PropertyLabel<T> : Label {

		protected Property<T> property;
		protected string text;

		/// <summary>
		/// Creates instance of the PropertyLabel which stores the text and the property.
		/// </summary>
		/// <param name="property">The porperty witch value to display.</param>
		/// <param name="text">The text which will be printed before the property value.</param>
		public PropertyLabel(Property<T> property, string text)
			: base() {
			this.text = text + " ";
			this.property = property;
		}

		/// <summary>
		/// Overrides function because needs to reload the property value.
		/// </summary>
		protected override void UpdateCore() {
			Text = text + property.ToString();
			base.UpdateCore();
		}

		/// <summary>
		/// Sets new property value.
		/// </summary>
		/// <param name="value">The property value.</param>
		public void Set(T value) {
			property.Value = value;
		}
	}
}
