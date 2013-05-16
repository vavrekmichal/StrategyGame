using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.GroupMgr;



namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	class Planet : StaticGameObject {

		protected double mDistance = 0.0f; //distance to positoin
		protected Mogre.Vector3 mDirection = Mogre.Vector3.ZERO;   // The direction the object is moving

		protected bool mFlying = false; //bool to detect if object walking or stay


		protected double travelledInvisible;


		protected LinkedList<Mogre.Vector3> circularPositions;

		private static Random random = new Random();


		public Planet(string name, string mesh, Team myTeam, double distanceFromCenter,
			Vector3 center, int circularNum = 30) {
			this.name = name;
			this.mesh = mesh;
			this.team = myTeam;
			base.SetProperty(PropertyEnum.Speed, Game.PropertyManager.GetProperty<float>("speed3"));
			base.SetProperty(PropertyEnum.Rotate, Game.PropertyManager.GetProperty<float>("planetRotateSpeed"));
			base.SetProperty(PropertyEnum.PickUp, Game.PropertyManager.GetProperty<float>("planetPickUpDistance"));

			//prepare list of positions
			circularPositions = CalculatePositions(circularNum, distanceFromCenter, center);
			RandomizeStartPosition(circularNum); // randomize start position
			position = circularPositions.First();

			//Mogre inicialization of object
			entity = Game.SceneManager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Rotating in visible mood, it means when planet is in active solar system
		/// </summary>
		/// <param name="f">delay between frames</param>
		public override void Rotate(float f) {
			TryExecute("Produce");
			sceneNode.Roll(new Mogre.Degree((float)(GetProperty<float>(PropertyEnum.Speed).Value * GetProperty<float>(PropertyEnum.Rotate).Value * f)));
			//position in LinkedList now moving
			if (!mFlying) {
				if (NextLocation()) {
					mFlying = true;
					position = circularPositions.First.Value; //get the next destination.
					PrepareNextPosition();
					//update the direction and the distance
					mDirection = position - sceneNode.Position;
					mDistance = mDirection.Normalise();
				} else {
				}//nothing to do so stay in position    
			} else {
				double move = GetProperty<float>(PropertyEnum.Speed).Value * f;
				mDistance -= move;
				if (mDistance <= .0f) { //reach destination
					travelledInvisible = 0;
					sceneNode.Position = position;
					mDirection = Mogre.Vector3.ZERO;
					mFlying = false;
				} else {
					sceneNode.Translate(mDirection * (float)move);
				}
			}
		}

		/// <summary>
		/// Function calculate moves in invisible mode
		/// </summary>
		/// <param name="f">delay between frames</param>
		public override void NonActiveRotate(float f) {
			TryExecute("Produce");
			if (!mFlying) {
				if (NextLocation()) {
					mFlying = true;
					position = circularPositions.First.Value; //get the next destination.
					PrepareNextPosition();
					mDistance = mDirection.Normalise();
				} else {
				}
			} else {
				double move = GetProperty<float>(PropertyEnum.Speed).Value * f;
				mDistance -= move;
				if (mDistance <= .0f) { //reach destination
					travelledInvisible = 0;
					mDirection = Mogre.Vector3.ZERO;
					mFlying = false;

				} else {
					travelledInvisible += move;
				}
			}
		}


		//own functions 

		/// <summary>
		/// Randomize starting position of planet
		/// </summary>
		/// <param name="max">max of rotates</param>
		private void RandomizeStartPosition(int max) {
			for (int i = 0; i < GetRandomNumber(max); i++) {
				PrepareNextPosition();
			}
		}

		private static int GetRandomNumber(int max) {
			return random.Next(max);
		}

		/// <summary>
		/// Cyclic remove from LinkedList and add on the end
		/// </summary>
		private void PrepareNextPosition() {
			var tmp = circularPositions.First; //save the node that held it
			circularPositions.RemoveFirst(); //remove that node from the front of the list
			circularPositions.AddLast(tmp);  //add it to the back of the list.
		}

		/// <summary>
		/// Calculate posistion on circle represent as ngon
		/// </summary>
		/// <param name="circularNum">Number of positions on circle</param>
		/// <param name="distanceFromCenter">radius on circle</param>
		/// <returns>linkedList with position on ngon (circle)</returns>
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
		/// The NextLocation() check if exist next location to move
		/// </summary>
		/// <returns>true ->exist / false -> not exist</returns>
		private bool NextLocation() {
			if (circularPositions.Count == 0) {
				return false;
			}
			return true;
		}

		//public override Dictionary<string, object> getPropertyToDisplay() {
		//	var propToDisp = new Dictionary<string, object>(propertyDict);
		//	return propToDisp;
		//}


		/// <summary>
		/// Called when object is displayed (invisible to visible)
		/// </summary>
		protected override void OnDisplayed() {
			sceneNode.Position = position;
			mFlying = false; //jump correction
		}

		/// <summary>
		/// PickUpDistance is setted by runtime property
		/// </summary>
		public override float PickUpDistance {
			get { return GetProperty<float>(PropertyEnum.PickUp).Value; }
		}

		/// <summary>
		/// OccupyDistance is overriden and now can be really occupied (value is not 0)
		/// </summary>
		public override float OccupyDistance {
			get { return GetProperty<float>(PropertyEnum.PickUp).Value * 5; }
		}

	}
}
