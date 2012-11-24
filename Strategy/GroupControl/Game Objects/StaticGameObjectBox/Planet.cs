using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
    class Planet : StaticGameObject {

        protected double mDistance = 0.0f; //distance to positoin
        protected Mogre.Vector3 mDirection = Mogre.Vector3.ZERO;   // The direction the object is moving
        protected Mogre.Vector3 mDestination = Mogre.Vector3.ZERO; // The destination the object is moving towards
        protected bool mFlying = false; //bool to detect if object walking or stay
        protected double mFlySpeed = 200f; //speed of planet

        public Planet(string name, string mesh, int team, int planetSystem, Mogre.SceneManager manager, double distanceFromCenter, int circularNum = 20) {
            this.name = name;
            this.mesh = mesh;
            this.team = team;
            this.planetSystem = planetSystem;
            
            //prepare list of positions
            circularPositions = calculatePositions(circularNum, distanceFromCenter);
            randomizeStartPosition(circularNum); // randomize start position

            //Mogre inicialization of object
            entity = manager.CreateEntity(name, mesh);
            sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", circularPositions.First());

            //circularPositions[random.Next(circularNum) ]
            sceneNode.Pitch(new Mogre.Degree(-90f));
            sceneNode.AttachObject(entity);

        }

        public override void rotate(float f) {
            sceneNode.Roll(new Mogre.Degree((float)(mFlySpeed * 0.5 *f)));
            //here is place to make moves
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
                double move = mFlySpeed * f;
                mDistance -= move;
                if (mDistance <= .0f) { //reach destination
                    sceneNode.Position = mDestination;
                    mDirection = Mogre.Vector3.ZERO;
                    mFlying = false;
                } else {
                    sceneNode.Translate(mDirection * (float)move);
                }
            }
        }

        public override void produce(float f) {
            throw new NotImplementedException();
        }


        //own functions 

        private void randomizeStartPosition(int max) {
            Random random = new Random();
            int r = random.Next(max);
            for (int i = 0; i < r; i++) {
                prepareNextPosition();
            }
        }

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
        private LinkedList<Mogre.Vector3> calculatePositions(int circularNum, double distanceFromCenter) {
            var list = new LinkedList<Mogre.Vector3>();
            for (int i = 0; i < circularNum; i++) {
                double x = Math.Cos(i * 2 * Math.PI / circularNum) * distanceFromCenter;
                double y = Math.Sin(i * 2 * Math.PI / circularNum) * distanceFromCenter;
                list.AddFirst(new Mogre.Vector3((float)x, 0, (float)y));
            }

            return list;
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

    }
}
