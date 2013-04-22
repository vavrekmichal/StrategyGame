﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public class EditablePair<T1, T2> 
		where T1 : struct 
		where T2 : struct {

		T1 item1;
		T2 item2;

		public EditablePair(T1 i1, T2 i2) {
			item1 = i1;
			item2 = i2;
		}

		public T1 Item1 {
			get { return item1; }
			set { item1 = value; }
		}

		public T2 Item2 {
			get { return item2; }
			set { item2 = value; }
		}

	}
}
