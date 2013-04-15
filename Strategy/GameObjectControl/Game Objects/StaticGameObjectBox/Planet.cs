﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;



namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	class Planet : StaticGameObject {

		protected double mDistance = 0.0f; //distance to positoin
		protected Mogre.Vector3 mDirection = Mogre.Vector3.ZERO;   // The direction the object is moving

		protected bool mFlying = false; //bool to detect if object walking or stay


		protected double travelledInvisible;


		protected LinkedList<Mogre.Vector3> circularPositions;

		private static Random random = new Random();


		public Planet(string name, string mesh, Team myTeam, Mogre.SceneManager manager, double distanceFromCenter,
			Vector3 center, PropertyManager propMgr, int circularNum = 30) {
			this.name = name;
			this.mesh = mesh;
			planetTeam = myTeam;
			this.manager = manager;
			base.setProperty("Speed", propMgr.getProperty<float>("speed3"));
			base.setProperty("Rotate", propMgr.getProperty<float>("planetRotateSpeed"));
			base.setProperty("PickUp", propMgr.getProperty<float>("planetPickUpDistance"));

			//prepare list of positions
			circularPositions = calculatePositions(circularNum, distanceFromCenter, center);
			randomizeStartPosition(circularNum); // randomize start position
			mDestination = circularPositions.First();

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Rotating in visible mood, it means when planet is in active solar system
		/// </summary>
		/// <param name="f">delay between frames</param>
		public override void rotate(float f) {
			tryExecute("Produce");
			sceneNode.Roll(new Mogre.Degree((float)(getProperty<float>("Speed").Value * getProperty<float>("Rotate").Value * f)));
			//position in LinkedList now moving
			if (!mFlying) {
				if (nextLocation()) {
					mFlying = true;
					mDestination = circularPositions.First.Value; //get the next destination.
					prepareNextPosition();
					//update the direction and the distance
					mDirection = mDestination - sceneNode.Position;
					mDistance = mDirection.Normalise();
				} else {
				}//nothing to do so stay in position    
			} else {
				double move = getProperty<float>("Speed").Value * f;
				mDistance -= move;
				if (mDistance <= .0f) { //reach destination
					travelledInvisible = 0;
					sceneNode.Position = mDestination;
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
		public override void nonActiveRotate(float f) {
			tryExecute("Produce");
			if (!mFlying) {
				if (nextLocation()) {
					mFlying = true;
					mDestination = circularPositions.First.Value; //get the next destination.
					prepareNextPosition();
					mDistance = mDirection.Normalise();
				} else {
				}
			} else {
				double move = getProperty<float>("Speed").Value * f;
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
		private void randomizeStartPosition(int max) {
			for (int i = 0; i < getRandomNumber(max); i++) {
				prepareNextPosition();
			}
		}

		private static int getRandomNumber(int max) {
			return random.Next(max);
		}

		/// <summary>
		/// Cyclic remove from LinkedList and add on the end
		/// </summary>
		private void prepareNextPosition() {
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
		private LinkedList<Mogre.Vector3> calculatePositions(int circularNum, double distanceFromCenter, Vector3 center) {
			var list = new LinkedList<Mogre.Vector3>();
			for (int i = 0; i < circularNum; i++) {
				double x = System.Math.Cos(i * 2 * System.Math.PI / circularNum) * distanceFromCenter;
				double y = System.Math.Sin(i * 2 * System.Math.PI / circularNum) * distanceFromCenter;
				list.AddFirst(new Mogre.Vector3((float)x + center.x, 0, (float)y) + center.y);
			}

			return list;
		}


		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public override void changeVisible(bool visible) {   //now creating
			if (visible) {
				if (sceneNode == null) {
					if (entity == null) { //control if the entity is inicialized
						entity = manager.CreateEntity(name, mesh);
					}

					if (mDestination == null) { //control inicialization
						mDestination = circularPositions.Last();
					}

					sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", mDestination);

					sceneNode.Pitch(new Degree(-90f));
					sceneNode.AttachObject(entity);
					onDisplayed();
				}
			} else {
				if (sceneNode != null) {
					manager.DestroySceneNode(sceneNode);
					sceneNode = null;
				}
			}
		}


		/// <summary>
		/// The NextLocation() check if exist next location to move
		/// </summary>
		/// <returns>true ->exist / false -> not exist</returns>
		private bool nextLocation() {
			if (circularPositions.Count == 0) {
				return false;
			}
			return true;
		}

		public override Dictionary<string, object> getPropertyToDisplay() {
			var propToDisp = new Dictionary<string, object>(propertyDict);
			return propToDisp;
		}


		/// <summary>
		/// Called when object is displayed (invisible to visible)
		/// </summary>
		protected override void onDisplayed() {
			sceneNode.Position = mDestination;
			mFlying = false; //jump correction
		}

		/// <summary>
		/// PickUpDistance is setted by runtime property
		/// </summary>
		public override float PickUpDistance {
			get { return getProperty<float>("PickUp").Value; ; }
		}

	}
}