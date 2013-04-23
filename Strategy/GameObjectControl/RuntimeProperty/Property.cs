using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public class Property<T> : ICloneable {
		private T value;

		public Property(T value) {
			this.value = value;
		}

		public T Value {
			set { this.value = value; }
			get { return this.value; }
		}

		public object Clone() {
			return this.MemberwiseClone();
		}

		public enum Operator {
			Plus,
			Minus
		}

		private object doTheOperation(Operator op, dynamic a, dynamic b) {
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
		/// Generic function calls private doTheOperation with Properties values
		/// and convert to T2 type
		/// </summary>
		/// <typeparam name="T2">Result Property type</typeparam>
		/// <param name="op">Operator of operation</param>
		/// <param name="p2">Second argument of operation</param>
		/// <returns>Property with result of operation</returns>
		public Property<T> simpleMath(Operator op, Property<T> p2) {
			var newValue = doTheOperation(op, Value, p2.Value);
			T pok = (T)Convert.ChangeType(newValue, typeof(T));
			return new Property<T>(pok);
		}


		/// <summary>
		/// Determines if a type is numeric.  Nullable numeric types are considered numeric.
		/// </summary>
		/// <param name="type">Controling type</param>
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
