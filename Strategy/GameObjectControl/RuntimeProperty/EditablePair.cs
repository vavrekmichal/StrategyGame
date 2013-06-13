
namespace Strategy.GameObjectControl.RuntimeProperty {
	/// <summary>
	/// Contains two generics values. The class is editable.
	/// </summary>
	/// <typeparam name="T1">The type of a first value.</typeparam>
	/// <typeparam name="T2">The type of a second value.</typeparam>
	public class EditablePair<T1, T2>  
		where T2 : struct {

		T1 item1;
		T2 item2;

		/// <summary>
		/// Creates new instance and sets values.
		/// </summary>
		/// <param name="i1">The first value.</param>
		/// <param name="i2">The second value.</param>
		public EditablePair(T1 i1, T2 i2) {
			item1 = i1;
			item2 = i2;
		}

		/// <summary>
		/// Gets or sets first value.
		/// </summary>
		public T1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}

		/// <summary>
		/// Gets or sets second value.
		/// </summary>
		public T2 Item2 {
			get { return item2; }
			set { item2 = value; }
		}

	}
}
