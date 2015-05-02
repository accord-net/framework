// Animat sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using AForge.MachineLearning;

namespace Animat
{
    public partial class MainForm : Form
    {
        // map and its dimension
        private int[,] map = null;
        private int[,] mapToDisplay = null;
        private int mapWidth;
        private int mapHeight;

        // agent' start and stop position
        private int agentStartX;
        private int agentStartY;
        private int agentStopX;
        private int agentStopY;

        // flag to stop background job
        private volatile bool needToStop = false;

        // worker thread
        private Thread workerThread = null;

        // learning settings
        int learningIterations = 100;
        private double explorationRate = 0.5;
        private double learningRate = 0.5;

        private double moveReward = 0;
        private double wallReward = -1;
        private double goalReward = 1;

        // Q-Learning algorithm
        private QLearning qLearning = null;
        // Sarsa algorithm
        private Sarsa sarsa = null;

        // Form constructor
        public MainForm( )
        {
            InitializeComponent( );

            // set world colors
            cellWorld.Coloring = new Color[] { Color.White, Color.Green, Color.Black, Color.Red };

            // show settings
            ShowSettings( );
            algorithmCombo.SelectedIndex = 0;
        }

        // Form is closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            // check if worker thread is running
            if ( ( workerThread != null ) && ( workerThread.IsAlive ) )
            {
                needToStop = true;
                while ( !workerThread.Join( 100 ) )
                    Application.DoEvents( );
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback( System.Windows.Forms.Control control, string text );

        // Thread safe updating of control's text property
        private void SetText( System.Windows.Forms.Control control, string text )
        {
            if ( control.InvokeRequired )
            {
                SetTextCallback d = new SetTextCallback( SetText );
                Invoke( d, new object[] { control, text } );
            }
            else
            {
                control.Text = text;
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback( bool enable );

        // Enable/disale controls (safe for threading)
        private void EnableControls( bool enable )
        {
            if ( InvokeRequired )
            {
                EnableCallback d = new EnableCallback( EnableControls );
                Invoke( d, new object[] { enable } );
            }
            else
            {
                loadButton.Enabled = enable;

                algorithmCombo.Enabled = enable;
                explorationRateBox.Enabled = enable;
                learningRateBox.Enabled = enable;
                iterationsBox.Enabled = enable;

                moveRewardBox.Enabled = enable;
                wallRewardBox.Enabled = enable;
                goalRewardBox.Enabled = enable;

                startLearningButton.Enabled = enable;
                showSolutionButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // Show settings
        private void ShowSettings( )
        {
            explorationRateBox.Text = explorationRate.ToString( );
            learningRateBox.Text = learningRate.ToString( );
            iterationsBox.Text = learningIterations.ToString( );

            moveRewardBox.Text = moveReward.ToString( );
            wallRewardBox.Text = wallReward.ToString( );
            goalRewardBox.Text = goalReward.ToString( );
        }

        // Get settings
        private void GetSettings( )
        {
            // explortion rate
            try
            {
                explorationRate = Math.Max( 0.0, Math.Min( 1.0, double.Parse( explorationRateBox.Text ) ) );
            }
            catch
            {
                explorationRate = 0.5;
            }
            // learning rate
            try
            {
                learningRate = Math.Max( 0.0, Math.Min( 1.0, double.Parse( learningRateBox.Text ) ) );
            }
            catch
            {
                learningRate = 0.5;
            }
            // learning iterations
            try
            {
                learningIterations = Math.Max( 10, Math.Min( 100000, int.Parse( iterationsBox.Text ) ) );
            }
            catch
            {
                learningIterations = 100;
            }

            // move reward
            try
            {
                moveReward = Math.Max( -100, Math.Min( 100, double.Parse( moveRewardBox.Text ) ) );
            }
            catch
            {
                moveReward = 0;
            }
            // wall reward
            try
            {
                wallReward = Math.Max( -100, Math.Min( 100, double.Parse( wallRewardBox.Text ) ) );
            }
            catch
            {
                wallReward = -1;
            }
            // goal reward
            try
            {
                goalReward = Math.Max( -100, Math.Min( 100, double.Parse( goalRewardBox.Text ) ) );
            }
            catch
            {
                goalReward = 1;
            }
        }

        // On "Load" button click
        private void loadButton_Click( object sender, EventArgs e )
        {
			// show file selection dialog
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                StreamReader reader = null;

                try
                {
                    // open selected file
                    reader = File.OpenText( openFileDialog.FileName );
                    string str = null;
                    // line counter
                    int lines = 0;
                    int j = 0;

                    // read the file
                    while ( ( str = reader.ReadLine( ) ) != null )
                    {
                        str = str.Trim( );

                        // skip comments and empty lines
                        if ( ( str == string.Empty ) || ( str[0] == ';' ) || ( str[0] == '\0' ) )
                            continue;

                        // split the string
                        string[] strs = str.Split( ' ' );

                        // check the line
                        if ( lines == 0 )
                        {
                            // get world size
                            mapWidth = int.Parse( strs[0] );
                            mapHeight = int.Parse( strs[1] );
                            map = new int[mapHeight, mapWidth];
                        }
                        else if ( lines == 1 )
                        {
                            // get agents count
                            if ( int.Parse( strs[0] ) != 1 )
                            {
                                MessageBox.Show( "The application supports only 1 agent in the worlds", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                break;
                            }
                        }
                        else if ( lines == 2 )
                        {
                            // agent position
                            agentStartX = int.Parse( strs[0] );
                            agentStartY = int.Parse( strs[1] );
                            agentStopX = int.Parse( strs[2] );
                            agentStopY = int.Parse( strs[3] );

                            // check position
                            if (
                                ( agentStartX < 0 ) || ( agentStartX >= mapWidth ) ||
                                ( agentStartY < 0 ) || ( agentStartY >= mapHeight ) ||
                                ( agentStopX < 0 ) || ( agentStopX >= mapWidth ) ||
                                ( agentStopY < 0 ) || ( agentStopY >= mapHeight )
                                )
                            {
                                MessageBox.Show( "Agent's start and stop coordinates should be inside the world area ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                                break;
                            }
                        }
                        else if ( lines > 2 )
                        {
                            // map lines
                            if ( j < mapHeight )
                            {
                                for ( int i = 0; i < mapWidth; i++ )
                                {
                                    map[j, i] = int.Parse( strs[i] );
                                    if ( map[j, i] > 1 )
                                        map[j, i] = 1;
                                }
                                j++;
                            }
                        }
                        lines++;
                    }

                    // set world's map
                    mapToDisplay = (int[,]) map.Clone( );
                    mapToDisplay[agentStartY, agentStartX] = 2;
                    mapToDisplay[agentStopY, agentStopX] = 3;
                    cellWorld.Map = mapToDisplay;

                    // show world's size
                    worldSizeBox.Text = string.Format( "{0} x {1}", mapWidth, mapHeight );

                    // enable learning button
                    startLearningButton.Enabled = true;
                }
                catch ( Exception )
                {
                    MessageBox.Show( "Failed reading the map file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }
                finally
                {
                    // close file
                    if ( reader != null )
                        reader.Close( );
                }
            }
        }

        // On "Start" learning button click
        private void startLearningButton_Click( object sender, EventArgs e )
        {
            // get settings
            GetSettings( );
            ShowSettings( );

            iterationBox.Text = string.Empty;

            // destroy algorithms
            qLearning = null;
            sarsa = null;

            if ( algorithmCombo.SelectedIndex == 0 )
            {
                // create new QLearning algorithm's instance
                qLearning = new QLearning( 256, 4, new TabuSearchExploration( 4, new EpsilonGreedyExploration( explorationRate ) ) );
                workerThread = new Thread( new ThreadStart( QLearningThread ) );
            }
            else
            {
                // create new Sarsa algorithm's instance
                sarsa = new Sarsa( 256, 4, new TabuSearchExploration( 4, new EpsilonGreedyExploration( explorationRate ) ) );
                workerThread = new Thread( new ThreadStart( SarsaThread ) );
            }

            // disable all settings controls except "Stop" button
            EnableControls( false );

            // run worker thread
            needToStop = false;
            workerThread.Start( );
        }

        // On "Stop" button click
        private void stopButton_Click( object sender, EventArgs e )
        {
            if ( workerThread != null )
            {
                // stop worker thread
                needToStop = true;
                while ( !workerThread.Join( 100 ) )
                    Application.DoEvents( );
                workerThread = null;
            }
        }

        // On "Show Solution" button
        private void showSolutionButton_Click( object sender, EventArgs e )
        {
            // check if learning algorithm was run before
            if ( ( qLearning == null ) && ( sarsa == null ) )
                return;

            // disable all settings controls except "Stop" button
            EnableControls( false );

            // run worker thread
            needToStop = false;
            workerThread = new Thread( new ThreadStart( ShowSolutionThread ) );
            workerThread.Start( );
        }

        // Q-Learning thread
        private void QLearningThread( )
        {
            int iteration = 0;
            // curent coordinates of the agent
            int agentCurrentX, agentCurrentY;
            // exploration policy
            TabuSearchExploration tabuPolicy = (TabuSearchExploration) qLearning.ExplorationPolicy;
            EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration) tabuPolicy.BasePolicy;

			// loop
            while ( ( !needToStop ) && ( iteration < learningIterations ) )
            {
                // set exploration rate for this iteration
                explorationPolicy.Epsilon = explorationRate - ( (double) iteration / learningIterations ) * explorationRate;
                // set learning rate for this iteration
                qLearning.LearningRate = learningRate - ( (double) iteration / learningIterations ) * learningRate;
                // clear tabu list
                tabuPolicy.ResetTabuList( );

                // reset agent's coordinates to the starting position
                agentCurrentX = agentStartX;
                agentCurrentY = agentStartY;

                // steps performed by agent to get to the goal
                int steps = 0;

                while ( ( !needToStop ) && ( ( agentCurrentX != agentStopX ) || ( agentCurrentY != agentStopY ) ) )
                {
                    steps++;
                    // get agent's current state
                    int currentState = GetStateNumber( agentCurrentX, agentCurrentY );
                    // get the action for this state
                    int action = qLearning.GetAction( currentState );
                    // update agent's current position and get his reward
                    double reward = UpdateAgentPosition( ref agentCurrentX, ref agentCurrentY, action );
                    // get agent's next state
                    int nextState = GetStateNumber( agentCurrentX, agentCurrentY );
                    // do learning of the agent - update his Q-function
                    qLearning.UpdateState( currentState, action, reward, nextState );

                    // set tabu action
                    tabuPolicy.SetTabuAction( ( action + 2 ) % 4, 1 );
                }

                System.Diagnostics.Debug.WriteLine( steps );
                
                iteration++;

                // show current iteration
                SetText( iterationBox, iteration.ToString(  ) );
            }

            // enable settings controls
            EnableControls( true );
        }

        // Sarsa thread
        private void SarsaThread( )
        {
            int iteration = 0;
            // curent coordinates of the agent
            int agentCurrentX, agentCurrentY;
            // exploration policy
            TabuSearchExploration tabuPolicy = (TabuSearchExploration) sarsa.ExplorationPolicy;
            EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration) tabuPolicy.BasePolicy;

			// loop
            while ( ( !needToStop ) && ( iteration < learningIterations ) )
            {
                // set exploration rate for this iteration
                explorationPolicy.Epsilon = explorationRate - ( (double) iteration / learningIterations ) * explorationRate;
                // set learning rate for this iteration
                sarsa.LearningRate = learningRate - ( (double) iteration / learningIterations ) * learningRate;
                // clear tabu list
                tabuPolicy.ResetTabuList( );

                // reset agent's coordinates to the starting position
                agentCurrentX = agentStartX;
                agentCurrentY = agentStartY;

                // steps performed by agent to get to the goal
                int steps = 1;
                // previous state and action
                int previousState = GetStateNumber( agentCurrentX, agentCurrentY );
                int previousAction = sarsa.GetAction( previousState );
                // update agent's current position and get his reward
                double reward = UpdateAgentPosition( ref agentCurrentX, ref agentCurrentY, previousAction );

                while ( ( !needToStop ) && ( ( agentCurrentX != agentStopX ) || ( agentCurrentY != agentStopY ) ) )
                {
                    steps++;

                    // set tabu action
                    tabuPolicy.SetTabuAction( ( previousAction + 2 ) % 4, 1 );

                    // get agent's next state
                    int nextState = GetStateNumber( agentCurrentX, agentCurrentY );
                    // get agent's next action
                    int nextAction = sarsa.GetAction( nextState );
                    // do learning of the agent - update his Q-function
                    sarsa.UpdateState( previousState, previousAction, reward, nextState, nextAction );

                    // update agent's new position and get his reward
                    reward = UpdateAgentPosition( ref agentCurrentX, ref agentCurrentY, nextAction );

                    previousState = nextState;
                    previousAction = nextAction;
                }

                if ( !needToStop )
                {
                    // update Q-function if terminal state was reached
                    sarsa.UpdateState( previousState, previousAction, reward );
                }

                System.Diagnostics.Debug.WriteLine( steps );

                iteration++;

                // show current iteration
                SetText( iterationBox, iteration.ToString( ) );
            }

            // enable settings controls
            EnableControls( true );
        }

        // Show solution thread
        private void ShowSolutionThread( )
        {
            // set exploration rate to 0, so agent uses only what he learnt
            TabuSearchExploration tabuPolicy = null;
            EpsilonGreedyExploration exploratioPolicy = null;

            if ( qLearning != null )
                tabuPolicy = (TabuSearchExploration) qLearning.ExplorationPolicy;
            else
                tabuPolicy = (TabuSearchExploration) sarsa.ExplorationPolicy;

            exploratioPolicy = (EpsilonGreedyExploration) tabuPolicy.BasePolicy;

            exploratioPolicy.Epsilon = 0;
            tabuPolicy.ResetTabuList( );

            // curent coordinates of the agent
            int agentCurrentX = agentStartX, agentCurrentY = agentStartY;

            // pripate the map to display
            Array.Copy( map, mapToDisplay, mapWidth * mapHeight );
            mapToDisplay[agentStartY, agentStartX] = 2;
            mapToDisplay[agentStopY, agentStopX] = 3;

            while ( !needToStop )
            {
                // dispay the map
                cellWorld.Map = mapToDisplay;
                // sleep for a while
                Thread.Sleep( 200 );

                // check if we have reached the end point
                if ( ( agentCurrentX == agentStopX ) && ( agentCurrentY == agentStopY ) )
                {
                    // restore the map
                    mapToDisplay[agentStartY, agentStartX] = 2;
                    mapToDisplay[agentStopY, agentStopX] = 3;

                    agentCurrentX = agentStartX;
                    agentCurrentY = agentStartY;

                    cellWorld.Map = mapToDisplay;
                    Thread.Sleep( 200 );
                }

                // remove agent from current position
                mapToDisplay[agentCurrentY, agentCurrentX] = 0;

                // get agent's current state
                int currentState = GetStateNumber( agentCurrentX, agentCurrentY );
                // get the action for this state
                int action = ( qLearning != null ) ? qLearning.GetAction( currentState ) : sarsa.GetAction( currentState );
                // update agent's current position and get his reward
                double reward = UpdateAgentPosition( ref agentCurrentX, ref agentCurrentY, action );

                // put agent to the new position
                mapToDisplay[agentCurrentY, agentCurrentX] = 2;
            }

            // enable settings controls
            EnableControls( true );
        }

        // Update agent position and return reward for the move
        private double UpdateAgentPosition( ref int currentX, ref int currentY, int action )
        {
            // default reward is equal to moving reward
            double reward = moveReward;
            // moving direction
            int dx = 0, dy = 0;

            switch ( action )
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
            if (
                ( map[newY, newX] != 0 ) ||
                ( newX < 0 ) || ( newX >= mapWidth ) ||
                ( newY < 0 ) || ( newY >= mapHeight )
                )
            {
                // we found a wall or got outside of the world
                reward = wallReward;
            }
            else
            {
                currentX = newX;
                currentY = newY;

                // check if we found the goal
                if ( ( currentX == agentStopX ) && ( currentY == agentStopY ) )
                    reward = goalReward;
            }

            return reward;
        }

        // Get state number from agent's receptors in the specified position
        private int GetStateNumber( int x, int y )
        {
            int c1 = ( map[y - 1, x - 1] != 0 ) ? 1 : 0;
            int c2 = ( map[y - 1, x] != 0 ) ? 1 : 0;
            int c3 = ( map[y - 1, x + 1] != 0 ) ? 1 : 0;
            int c4 = ( map[y, x + 1] != 0 ) ? 1 : 0;
            int c5 = ( map[y + 1, x + 1] != 0 ) ? 1 : 0;
            int c6 = ( map[y + 1, x] != 0 ) ? 1 : 0;
            int c7 = ( map[y + 1, x - 1] != 0 ) ? 1 : 0;
            int c8 = ( map[y, x - 1] != 0 ) ? 1 : 0;

            return c1 |
                ( c2 << 1 ) |
                ( c3 << 2 ) |
                ( c4 << 3 ) |
                ( c5 << 4 ) |
                ( c6 << 5 ) |
                ( c7 << 6 ) |
                ( c8 << 7 );
        }
    }
}