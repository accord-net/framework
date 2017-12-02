// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.Geometry;
    using NUnit.Framework;
    using System;
    using Accord.Math.Geometry;
    using Accord.Math;
    using Accord.Statistics.Models.Regression.Linear;
    using Accord.MachineLearning;
    using System.Collections.Generic;

    [TestFixture]
    public class QLearningTest
    {

        [Test]
        public void learn_test()
        {
            #region doc_main
            // Fix the random number generator
            Accord.Math.Random.Generator.Seed = 0;

            // In this example, we will be using the QLearning algorithm
            // to make a robot learn how to navigate a map. The map is 
            // shown below, where a 1 denotes a wall and 0 denotes areas 
            // where the robot can navigate:
            //
            int[,] map =
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 0, 0, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 1, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 1, 1, 0, 0, 0, 1 },
                { 1, 1, 0, 1, 0, 0, 0, 0, 1 },
                { 1, 1, 0, 1, 0, 1, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            };

            // Now, we define the initial and target points from which the
            // robot will be spawn and where it should go, respectively:
            int agentStartX = 1;
            int agentStartY = 4;

            int agentStopX = 7;
            int agentStopY = 4;

            // The robot is able to sense the environment though 8 sensors
            // that capture whether the robot is near a wall or not. Based
            // on the robot's current location, the sensors will return an
            // integer number representing which sensors have detected walls

            Func<int, int, int> getState = (int x, int y) =>
            {
                int c1 = (map[y - 1, x - 1] != 0) ? 1 : 0;
                int c2 = (map[y - 1, x + 0] != 0) ? 1 : 0;
                int c3 = (map[y - 1, x + 1] != 0) ? 1 : 0;
                int c4 = (map[y + 0, x + 1] != 0) ? 1 : 0;
                int c5 = (map[y + 1, x + 1] != 0) ? 1 : 0;
                int c6 = (map[y + 1, x + 0] != 0) ? 1 : 0;
                int c7 = (map[y + 1, x - 1] != 0) ? 1 : 0;
                int c8 = (map[y + 0, x - 1] != 0) ? 1 : 0;

                return c1 | (c2 << 1) | (c3 << 2) | (c4 << 3) | (c5 << 4) | (c6 << 5) | (c7 << 6) | (c8 << 7);
            };

            // The actions are the possible directions the robot can go:
            //
            //   - case 0: go to north (up)
            //   - case 1: go to east (right)
            //   - case 2: go to south (down)
            //   - case 3: go to west (left)
            //

            int learningIterations = 1000;
            double explorationRate = 0.5;
            double learningRate = 0.5;

            double moveReward = 0;
            double wallReward = -1;
            double goalReward = 1;

            // The function below specifies how the robot should perform an action given its 
            // current position and an action number. This will cause the robot to update its 
            // current X and Y locations given the direction (above) it was instructed to go:
            Func<int, int, int, Tuple<double, int, int>> doAction = (int currentX, int currentY, int action) =>
            {
                // default reward is equal to moving reward
                double reward = moveReward;

                // moving direction
                int dx = 0, dy = 0;

                switch (action)
                {
                    case 0:         // go to north (up)
                        dy = -1;
                        break;
                    case 1:         // go to east (right)
                        dx = 1;
                        break;
                    case 2:         // go to south (down)
                        dy = 1;
                        break;
                    case 3:         // go to west (left)
                        dx = -1;
                        break;
                }

                int newX = currentX + dx;
                int newY = currentY + dy;

                // check new agent's coordinates
                if ((map[newY, newX] != 0) || (newX < 0) || (newX >= map.Columns()) || (newY < 0) || (newY >= map.Rows()))
                {
                    // we found a wall or got outside of the world
                    reward = wallReward;
                }
                else
                {
                    currentX = newX;
                    currentY = newY;

                    // check if we found the goal
                    if ((currentX == agentStopX) && (currentY == agentStopY))
                        reward = goalReward;
                }

                return Tuple.Create(reward, currentX, currentY);
            };


            // After defining all those functions, we create a new Sarsa algorithm:
            var explorationPolicy = new EpsilonGreedyExploration(explorationRate);
            var tabuPolicy = new TabuSearchExploration(4, explorationPolicy);
            var qLearning = new QLearning(256, 4, tabuPolicy);

            // curent coordinates of the agent
            int agentCurrentX = -1;
            int agentCurrentY = -1;

            bool needToStop = false;
            int iteration = 0;

            // loop
            while ((!needToStop) && (iteration < learningIterations))
            {
                // set exploration rate for this iteration
                explorationPolicy.Epsilon = explorationRate - ((double)iteration / learningIterations) * explorationRate;

                // set learning rate for this iteration
                qLearning.LearningRate = learningRate - ((double)iteration / learningIterations) * learningRate;

                // clear tabu list
                tabuPolicy.ResetTabuList();

                // reset agent's coordinates to the starting position
                agentCurrentX = agentStartX;
                agentCurrentY = agentStartY;

                // previous state and action
                int previousState = getState(agentCurrentX, agentCurrentY);
                int previousAction = qLearning.GetAction(previousState);

                // update agent's current position and get his reward
                var r = doAction(agentCurrentX, agentCurrentY, previousAction);
                double reward = r.Item1;
                agentCurrentX = r.Item2;
                agentCurrentY = r.Item3;

                // loop
                while ((!needToStop) && (iteration < learningIterations))
                {
                    // set exploration rate for this iteration
                    explorationPolicy.Epsilon = explorationRate - ((double)iteration / learningIterations) * explorationRate;
                    // set learning rate for this iteration
                    qLearning.LearningRate = learningRate - ((double)iteration / learningIterations) * learningRate;
                    // clear tabu list
                    tabuPolicy.ResetTabuList();

                    // reset agent's coordinates to the starting position
                    agentCurrentX = agentStartX;
                    agentCurrentY = agentStartY;

                    // steps performed by agent to get to the goal
                    int steps = 0;

                    while ((!needToStop) && ((agentCurrentX != agentStopX) || (agentCurrentY != agentStopY)))
                    {
                        steps++;
                        // get agent's current state
                        int currentState = getState(agentCurrentX, agentCurrentY);

                        // get the action for this state
                        int action = qLearning.GetAction(currentState);

                        // update agent's current position and get his reward
                        r = doAction(agentCurrentX, agentCurrentY, action);
                        reward = r.Item1;
                        agentCurrentX = r.Item2;
                        agentCurrentY = r.Item3;

                        // get agent's next state
                        int nextState = getState(agentCurrentX, agentCurrentY);

                        // do learning of the agent - update his Q-function
                        qLearning.UpdateState(currentState, action, reward, nextState);

                        // set tabu action
                        tabuPolicy.SetTabuAction((action + 2) % 4, 1);
                    }

                    System.Diagnostics.Debug.WriteLine(steps);

                    iteration++;
                }
            }

            // The end position for the robot will be (7, 4):
            int finalPosX = agentCurrentX; // 7
            int finalPosY = agentCurrentY; // 4;
            #endregion

            Assert.AreEqual(7, finalPosX);
            Assert.AreEqual(4, finalPosY);
        }

    }
}
