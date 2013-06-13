using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	/// <summary>
	/// Represents basic type of a static game object. The planet circling around the given center and around vertical axis.
	/// </summary>
	class Planet : StaticGameObject {

		protected double mDistance = 0.0f; //distance to positoin
		protected Mogre.Vector3 mDirection = Mogre.Vector3.ZERO;   // The direction the object is moving

		// Detect if object is moving or stay
		protected bool mFlying = false;

		Vector3 nextPostion;

		protected LinkedList<Mogre.Vector3> circularPositions;

		private static Random random = new Random();
		private static int circularNum = 30;

		/// <summary>
		/// Initializes Mogre properties, runtime Properties (Speed, Rotate - speed of rotation,
		/// PickUp - pick up distance). Needs 3 arguments and fourth is optional (name of a mesh,
		/// string with position - Vector2 converted to Vector3, distance from center and health).
		/// </summary>
		/// <param name="name">The name of the planet.</param>
		/// <param name="myTeam">The planet's team.</param>
		/// <param name="args">The array with arguments (3 required + 1 optional)</param>
		public Planet(string name, Team myTeam, object[] args) {
			this.name = name;
			this.mesh = (string)args[0];
			this.team = myTeam;

			this.position = new Property<Vector3>(Vector3.ZERO);

			SetProperty(PropertyEnum.Position, this.position);
			base.SetProperty(PropertyEnum.Speed, Game.PropertyManager.GetProperty<float>("speed3"));
			base.SetProperty(PropertyEnum.Position, position);
			base.SetProperty(PropertyEnum.Rotate, Game.PropertyManager.GetProperty<float>("planetRotateSpeed"));
			base.SetProperty(PropertyEnum.PickUp, Game.PropertyManager.GetProperty<float>("planetPickUpDistance"));

			if (args.Count() == 4) {
				setHp(Convert.ToInt32(args[3]));
			}

			var planetPosition = ParseStringToVector3((string)args[2]);
			var centerPosition = ParseStringToVector3((string)args[1]);

			// Prepare list of positions
			circularPositions = CalculatePositions(circularNum, CalculateDistance(planetPosition, centerPosition), centerPosition);

			// Sets start position
			SetStartPosition(circularNum, planetPosition);

			position.Value = circularPositions.First();

			// Mogre inicialization of object
			entity = Game.SceneManager.CreateEntity(name, mesh);
		}


		/// <summary>
		/// Moves in visible mood, it means when planet is in active solar system.
		/// </summary>
		/// <param name="delay">The delay between last two frames</param>
		public override void Rotate(float delay) {
			Update(delay);
			sceneNode.Roll(new Mogre.Degree((float)(GetProperty<float>(PropertyEnum.Speed).Value * 
				GetProperty<float>(PropertyEnum.Rotate).Value * delay)));

			if (!mFlying) {
				if (NextLocation()) {
					mFlying = true;
					nextPostion = circularPositions.First.Value; // Get the next destination.
					PrepareNextPosition();
					// Update the direction and the distance
					mDirection = nextPostion - position.Value;
					mDistance = mDirection.Normalise();
				} else {
				}// Nothing to do so stay in position    
			} else {
				double move = GetProperty<float>(PropertyEnum.Speed).Value * delay;
				mDistance -= move;
				if (mDistance <= .0f) { // Reached destination
					sceneNode.Position = nextPostion;
					position.Value = nextPostion;
					mDirection = Mogre.Vector3.ZERO;
					mFlying = false;
				} else {
					sceneNode.Translate(mDirection * (float)move);
					position.Value = sceneNode.Position;
				}
			}
		}

		/// <summary>
		/// Calculates move in invisible mode
		/// </summary>
		/// <param name="delay">The delay between last two frames</param>
		public override void NonActiveRotate(float delay) {
			Update(delay);

			if (!mFlying) {
				if (NextLocation()) {
					mFlying = true;
					nextPostion = circularPositions.First.Value; // Get the next destination.
					PrepareNextPosition();
					// Update the direction and the distance
					mDirection = nextPostion - position.Value;
					mDistance = mDirection.Normalise();
				} else {
				}// Nothing to do so stay in position    
			} else {
				double move = GetProperty<float>(PropertyEnum.Speed).Value * delay;
				mDistance -= move;
				if (mDistance <= .0f) { // Reached destination
					position.Value = nextPostion;
					mDirection = Mogre.Vector3.ZERO;
					mFlying = false;
				} else {
					position.Value += (mDirection * (float)move);
				}
			}
		}

		/// <summary>
		/// Returns value of PickUp Property.
		/// </summary>
		public override float PickUpDistance {
			get { return GetProperty<float>(PropertyEnum.PickUp).Value; }
		}

		/// <summary>
		/// Returns value of PickUp Property.
		/// </summary>
		public override float OccupyDistance {
			get { return GetProperty<float>(PropertyEnum.PickUp).Value * 2; }
		}

		/// <summary>
		/// Finds the nearest point in circularPositions and sets it as the start position.
		/// </summary>
		/// <param name="planetPosition">The loaded position of the planet.</param>
		private void SetStartPosition(int max, Vector3 planetPosition) {
			var sortedDistances = GetSortedDistances(planetPosition);

			var nearestPoint = sortedDistances.First().Value[0];

			while (circularPositions.First.Value != nearestPoint) {
				PrepareNextPosition();
			}
		}

		/// <summary>
		/// Calculates distance between all points and puts them into sorted dictionary.
		/// </summary>
		/// <param name="planetPosition">The position of planet.</param>
		/// <returns>Returns SortedDictionary with distances and vectors.</returns>
		private SortedDictionary<double, List<Vector3>> GetSortedDistances(Vector3 planetPosition) {
			var result = new SortedDictionary<double, List<Vector3>>();
			foreach (var item in circularPositions) {
				var distance = CalculateDistance(planetPosition, item);
				if (result.ContainsKey(distance)) {
					result[distance].Add(item);
				} else {
					result.Add(CalculateDistance(planetPosition, item), new List<Vector3>() { item });
				}
			}
			return result;
		}

		/// <summary>
		/// Cyclic removes from LinkedList and adds on the end.
		/// </summary>
		private void PrepareNextPosition() {
			var tmp = circularPositions.First; //save the node that held it
			circularPositions.RemoveFirst(); //remove that node from the front of the list
			circularPositions.AddLast(tmp);  //add it to the back of the list.
		}

		/// <summary>
		/// Calculates posistions on circle represent as n-gon
		/// </summary>
		/// <param name="circularNum">The number of positions on circle</param>
		/// <param name="distanceFromCenter">The radius of the circle</param>
		/// <returns>The LinkedList with positions on n-gon (circle)</returns>
		private LinkedList<Mogre.Vector3> CalculatePositions(int circularNum, double distanceFromCenter, Vector3 center) {
			var list = new LinkedList<Mogre.Vector3>();
			for (int i = 0; i < circularNum; i++) {
				double x = System.Math.Cos(i * 2 * System.Math.PI / circularNum) * distanceFromCenter;
				double y = System.Math.Sin(i * 2 * System.Math.PI / circularNum) * distanceFromCenter;
				list.AddFirst(new Mogre.Vector3((float)x + center.x, 0, (float)y) + center.y);
			}
			return list;
		}

		/// <summary>
		/// Checks if exist the next location to move.
		/// </summary>
		/// <returns>Returns if the next location exists.</returns>
		private bool NextLocation() {
			if (circularPositions.Count == 0) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Calculates distance between two points.
		/// </summary>
		/// <param name="vector1">The firtst vector.</param>
		/// <param name="vector2">The second vecotr.</param>
		/// <returns>Returns distance between two given points.</returns>
		private double CalculateDistance(Vector3 vector1, Vector3 vector2) {
			var xd = vector1.x - vector2.x;
			var yd = vector1.z - vector2.z;
			return System.Math.Sqrt(xd * xd + yd * yd);
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		protected override void OnDisplayed() {	}

	}
}
