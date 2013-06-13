using System;

namespace Strategy.GameObjectControl.RuntimeProperty {
	/// <summary>
	/// Stores the value and all object which has reference to this Property can
	/// see or edit same value.
	/// </summary>
	/// <typeparam name="T">The type of the Property.</typeparam>
	public class Property<T> : ICloneable {
		private T value;

		/// <summary>
		/// Initializes Property and sets the value.
		/// </summary>
		/// <param name="value">The value of the Property</param>
		public Property(T value) {
			this.value = value;
		}

		/// <summary>
		/// Gets or sets the value of the Property
		/// </summary>
		public T Value {
			set { this.value = value; }
			get { return this.value; }
		}

		/// <summary>
		/// Creates copy of this Property.
		/// </summary>
		/// <returns></returns>
		public object Clone() {
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Oparator enum.
		/// </summary>
		public enum Operator {
			Plus,
			Minus
		}

		/// <summary>
		/// Calculates the result of the operation.
		/// </summary>
		/// <param name="op">The operator.</param>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <returns>The result of the operation.</returns>
		private object ExecuteTheOperation(Operator op, dynamic a, dynamic b) {
			switch (op) {
				case Operator.Plus:
					return a + b;
				case Operator.Minus:
					return a - b;
				default:
					throw new ArgumentException("Unknown operator " + op);
			}
		}

		/// <summary>
		/// Calls ExecuteTheOperation with this Property value and with given Property value.
		/// Converts result to type of T.
		/// </summary>
		/// <typeparam Name="T">The type of the result.</typeparam>
		/// <param Name="op">The operator of the operation.</param>
		/// <param Name="p2">The second argument of the operation.</param>
		/// <returns>The Property with result of the operation.</returns>
		public Property<T> SimpleMath(Operator op, Property<T> p2) {
			var newValue = ExecuteTheOperation(op, Value, p2.Value);
			T pok = (T)Convert.ChangeType(newValue, typeof(T));
			return new Property<T>(pok);
		}

		/// <summary>
		/// Determines if a type is numeric. Nullable numeric types are considered numeric.
		/// </summary>
		/// <param Name="type">The controling type</param>
		/// <returns>If type is numeric (true) or not (false)</returns>
		private static bool IsNumericType(Type type) {
			if (type == null) {
				return false;
			}
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
				case TypeCode.Object:
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
						return IsNumericType(Nullable.GetUnderlyingType(type));
					}
					return false;
			}
			return false;
		}
	}
}
