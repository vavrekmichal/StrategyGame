using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public class Property<T> : ICloneable{
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
					throw new Exception("Unknown operator " + op);
			}
		}

		public Property<T2> simpleMath<T2>(Operator op, Property<T2> p2) {
			var newValue = doTheOperation(op, Value, p2.Value);
			T2 pok = (T2)Convert.ChangeType(newValue, typeof(T2));
			return new Property<T2>(pok);
		}

		public object pokus(Operator op, object p1, object p2) {
			if (p1.GetType() == typeof(Property<>) && p2.GetType() == typeof(Property<>)) {
				Type t = typeof(T);
				// createPropertyLabelAsLabel is private function
				MethodInfo method = typeof(Property<>).GetMethod("simpleMath", BindingFlags.NonPublic | BindingFlags.Instance); 
				MethodInfo generic = method.MakeGenericMethod(t);
				List<object> args = new List<object>();
				args.Add(op);
				args.Add(p1);
				args.Add(p2);
				var o = generic.Invoke(null, args.ToArray());
				return o;
			} else {
				throw new ArgumentException("Arguments must be Property.");
			}

		}

		/// <summary>
		/// Determines if a type is numeric.  Nullable numeric types are considered numeric.
		/// </summary>
		/// <remarks>
		/// Boolean is not considered numeric.
		/// </remarks>
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
