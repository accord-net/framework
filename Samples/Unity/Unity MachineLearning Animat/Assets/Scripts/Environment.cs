using Accord.MachineLearning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Environment : MonoBehaviour {

    public Transform PlayerObject;
    public Transform GoalObject;

    public References References;

    [SerializeField]
    private int learningIterations = 100;
    [SerializeField]
    private float explorationRate = 0.5f;
    [SerializeField]
    private float learningRate = 0.5f;

    [SerializeField]
    private int actions = 4;
    [SerializeField]
    private int mapWidth = 6;
    [SerializeField]
    private int mapHeight = 6;

    private double moveReward = 0;
    private double wallReward = -1;
    private double goalReward = 1;

    // agent's start and stop position
    private int _agentStartX;
    private int _agentStartY;
    private int _agentStopX;
    private int _agentStopY;

    // agent's current position
    private int _agentCurrX;
    private int _agentCurrY;

    // temp next state coordinates of the agent
    private int _agentNextX;
    private int _agentNextY;

    private bool _needToStop;
    private GameObject[] _wallObjects;
    private int[,] _map;

    // Q-Learning algorithm
    private QLearning _qLearning = null;
    // Sarsa algorithm
    private Sarsa _sarsa = null;
    // self implemented Q-Learning
    private QLearning_FDGS _qLearning_FDGS = null;

    private bool _showSolution;
    private bool _initShowSolution;
    private bool _enableControls;

    private float _timeStep;

    // worker thread
    private Thread _workerThread = null;

    private int _currentIteration;

    void Start () {
        // find walls
        _wallObjects = GameObject.FindGameObjectsWithTag("Wall");

        // init map
        _map = new int[mapHeight, mapWidth];

        // generate temporary map
        for (int i = 1; i < (mapWidth - 1); i++) {
            for (int j = 1; j < (mapHeight - 1); j++) {
                _map[j, i] = 0;
            }
        }

        // set values for walls
        for (int i = 0; i < _wallObjects.Length; i++) {
            _map[(int)_wallObjects[i].transform.localPosition.z, (int)_wallObjects[i].transform.localPosition.x] = -1;
        }

        _agentStartX = (int)PlayerObject.localPosition.x;
        _agentStartY = (int)PlayerObject.localPosition.z;

        _agentStopX = (int)GoalObject.localPosition.x;
        _agentStopY = (int)GoalObject.localPosition.z;

        // init textbox values
        References.WorldSize.text = string.Format("{0} x {1}", mapWidth, mapHeight);
        References.InitialExplorationRate.text = explorationRate.ToString();
        References.InitialLearningRate.text = learningRate.ToString();
        References.LearningIterations.text = learningIterations.ToString();

        // add button listener
        References.StartTraining.onClick.AddListener(StartLearning_OnClick);
        References.Stop.onClick.AddListener(Stop_OnClick);
        References.ShowSolution.onClick.AddListener(ShowSolution_OnClick);
    }
    
    void FixedUpdate () {
        if (_showSolution) {
            // move every 0.5 seconds
            if ((_timeStep + 0.25f) < Time.time) {

                _timeStep = Time.time;

                if ((_agentCurrX == _agentStopX) && (_agentCurrY == _agentStopY)) {

                    PlayerObject.localPosition = new Vector3(_agentStartX, 0, _agentStartY);

                    _agentCurrX = _agentStartX;
                    _agentCurrY = _agentStartY;
                } else {
                    if (_initShowSolution) {
                        _initShowSolution = false;

                        // set exploration rate to 0, so agent uses only what he learnt
                        TabuSearchExploration tabuPolicy = null;
                        EpsilonGreedyExploration exploratioPolicy = null;

                        if (_qLearning != null)
                            tabuPolicy = (TabuSearchExploration)_qLearning.ExplorationPolicy;
                        else if (_sarsa != null)
                            tabuPolicy = (TabuSearchExploration)_sarsa.ExplorationPolicy;
                        else
                            tabuPolicy = (TabuSearchExploration)_qLearning_FDGS.ExplorationPolicy;

                        exploratioPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

                        exploratioPolicy.Epsilon = 0;
                        tabuPolicy.ResetTabuList();

                        PlayerObject.localPosition = new Vector3(_agentStartX, 0, _agentStartY);

                        // current coordinates of the agent
                        _agentCurrX = (int)PlayerObject.localPosition.x;
                        _agentCurrY = (int)PlayerObject.localPosition.z;
                    }

                    if ((_qLearning != null) || (_sarsa != null)) {
                        // get agent's current state
                        int currentState = GetStateNumber(_agentCurrX, _agentCurrY);
                        // get the action for this state
                        int action = (_qLearning != null) ? _qLearning.GetAction(currentState) : _sarsa.GetAction(currentState);
                        // update agent's current position and get his reward
                        UpdateAgentPosition(action);
                    } else {
                        // get agent's current state
                        int currentState = _qLearning_FDGS.GetStateFromCoordinates(_agentCurrX, _agentCurrY);
                        // get the action for this state
                        int action = _qLearning_FDGS.GetLearnedAction(currentState);
                        // update agent's current position
                        UpdateAgentPosition(currentState, action);
                    }

                    // set player object position
                    PlayerObject.localPosition = new Vector3(_agentCurrX, 0, _agentCurrY);
                }
            }
        } else {
            if (!_needToStop) {
                // show current iteration
                References.CurrentIteration.text = _currentIteration.ToString();
            }

            if (_enableControls) {
                _enableControls = false;

                // enable settings controls
                References.EnableControls(true);
            }
        }
    }

    void OnApplicationQuit() {
        // check if worker thread is running
        if ((_workerThread != null) && (_workerThread.IsAlive)) {
            _needToStop = true;
            while (!_workerThread.Join(100)) { }
        }
    }

    private void StartLearning_OnClick() {

        // reset learning class values
        _qLearning = null;
        _sarsa = null;
        _qLearning_FDGS = null;

        if (References.LearningAlgorithm.value == 0) {
            // create new QLearning algorithm's instance
            _qLearning = new QLearning(256, 4, new TabuSearchExploration(4, new EpsilonGreedyExploration(explorationRate)));
            _workerThread = new Thread(new ThreadStart(QLearningThread));
        } else if (References.LearningAlgorithm.value == 1) {
            // create new Sarsa algorithm's instance
            _sarsa = new Sarsa(256, 4, new TabuSearchExploration(4, new EpsilonGreedyExploration(explorationRate)));
            _workerThread = new Thread(new ThreadStart(SarsaThread));
        } else {
            // init QLearn
            _qLearning_FDGS = new QLearning_FDGS(actions, _agentStopX, _agentStopY, _map, new TabuSearchExploration(actions, new EpsilonGreedyExploration(Convert.ToDouble(explorationRate))));
            _workerThread = new Thread(new ThreadStart(QLearningThread_FDGS));
        }

        // disable all settings controls except "Stop" button
        References.EnableControls(false);

        // run worker thread
        _needToStop = false;
        _workerThread.Start();

        Debug.Log("Learning started. Please wait until training is finished.");
    }

    private void Stop_OnClick() {
        if (_workerThread != null) {
            // stop worker thread
            _needToStop = true;
            while (!_workerThread.Join(100)) { }
            _workerThread = null;
        }

        // also stop showing the solution
        _showSolution = false;

        // current coordinates of the agent to stop position for reset
        _agentCurrX = _agentStopX;
        _agentCurrY = _agentStopY;

        _enableControls = true;

        Debug.Log("Everything stopped.");
    }

    private void ShowSolution_OnClick() {
        // check if learning algorithm was run before
        if ((_qLearning == null) && (_sarsa == null) && (_qLearning_FDGS == null)) {
            return;
        }

        // disable all settings controls except "Stop" button
        References.EnableControls(false);

        // run solution in update
        _needToStop = false;

        _showSolution = true;
        _initShowSolution = true;

        Debug.Log("Showing the learned solution.");
    }

    // Q-Learning thread
    private void QLearningThread() {
        _currentIteration = 0;

        // exploration policy
        TabuSearchExploration tabuPolicy = (TabuSearchExploration)_qLearning.ExplorationPolicy;
        EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

        // loop
        while ((!_needToStop) && (_currentIteration < learningIterations)) {
            // set exploration rate for this iteration
            explorationPolicy.Epsilon = explorationRate - ((double)_currentIteration / learningIterations) * explorationRate;
            // set learning rate for this iteration
            _qLearning.LearningRate = learningRate - ((double)_currentIteration / learningIterations) * learningRate;
            // clear tabu list
            tabuPolicy.ResetTabuList();

            // reset agent's coordinates to the starting position
            _agentCurrX = _agentStartX;
            _agentCurrY = _agentStartY;

            // steps performed by agent to get to the goal
            int steps = 0;

            while ((!_needToStop) && ((_agentCurrX != _agentStopX) || (_agentCurrY != _agentStopY))) {
                steps++;
                // get agent's current state
                int currentState = GetStateNumber(_agentCurrX, _agentCurrY);
                // get the action for this state
                int action = _qLearning.GetAction(currentState);
                // update agent's current position and get his reward
                double reward = UpdateAgentPosition(action);
                // get agent's next state
                int nextState = GetStateNumber(_agentCurrX, _agentCurrY);
                // do learning of the agent - update his Q-function
                _qLearning.UpdateState(currentState, action, reward, nextState);

                // set tabu action
                tabuPolicy.SetTabuAction((action + 2) % 4, 1);
            }

            _currentIteration++;
            Debug.Log(string.Format("{0} steps needed for iteration {1}.", steps, _currentIteration));
        }

        _enableControls = true;
        Debug.Log("QLearning training finished. Try to execute the solution.");
    }

    // Sarsa thread
    private void SarsaThread() {
        int iteration = 0;

        // exploration policy
        TabuSearchExploration tabuPolicy = (TabuSearchExploration)_sarsa.ExplorationPolicy;
        EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

        // loop
        while ((!_needToStop) && (iteration < learningIterations)) {
            // set exploration rate for this iteration
            explorationPolicy.Epsilon = explorationRate - ((double)iteration / learningIterations) * explorationRate;
            // set learning rate for this iteration
            _sarsa.LearningRate = learningRate - ((double)iteration / learningIterations) * learningRate;
            // clear tabu list
            tabuPolicy.ResetTabuList();

            // reset agent's coordinates to the starting position
            _agentCurrX = _agentStartX;
            _agentCurrY = _agentStartY;

            // steps performed by agent to get to the goal
            int steps = 1;
            // previous state and action
            int previousState = GetStateNumber(_agentCurrX, _agentCurrY);
            int previousAction = _sarsa.GetAction(previousState);
            // update agent's current position and get his reward
            double reward = UpdateAgentPosition(previousAction);

            while ((!_needToStop) && ((_agentCurrX != _agentStopX) || (_agentCurrY != _agentStopY))) {
                steps++;

                // set tabu action
                tabuPolicy.SetTabuAction((previousAction + 2) % 4, 1);

                // get agent's next state
                int nextState = GetStateNumber(_agentCurrX, _agentCurrY);
                // get agent's next action
                int nextAction = _sarsa.GetAction(nextState);
                // do learning of the agent - update his Q-function
                _sarsa.UpdateState(previousState, previousAction, reward, nextState, nextAction);

                // update agent's new position and get his reward
                reward = UpdateAgentPosition(nextAction);

                previousState = nextState;
                previousAction = nextAction;
            }

            if (!_needToStop) {
                // update Q-function if terminal state was reached
                _sarsa.UpdateState(previousState, previousAction, reward);
            }

            iteration++;
            Debug.Log(string.Format("{0} steps needed for iteration {1}.", steps, iteration));
        }

        _enableControls = true;
        Debug.Log("SARSA training finished. Try to execute the solution.");
    }

    // self implemented Q-Learning thread
    private void QLearningThread_FDGS() {
        int iteration = 0;

        // exploration policy
        TabuSearchExploration tabuPolicy = (TabuSearchExploration)_qLearning_FDGS.ExplorationPolicy;
        EpsilonGreedyExploration explorationPolicy = (EpsilonGreedyExploration)tabuPolicy.BasePolicy;

        // loop
        while ((!_needToStop) && (iteration < learningIterations)) {
            // set exploration rate for this iteration
            explorationPolicy.Epsilon = explorationRate - ((double)iteration / learningIterations) * explorationRate;
            // set learning rate for this iteration
            _qLearning_FDGS.LearningRate = learningRate - ((double)iteration / learningIterations) * learningRate;
            // clear tabu list
            tabuPolicy.ResetTabuList();

            // reset agent's coordinates to the starting position
            _agentCurrX = _agentStartX;
            _agentCurrY = _agentStartY;

            // steps performed by agent to get to the goal
            int steps = 0;

            while ((!_needToStop) && ((_agentCurrX != _agentStopX) || (_agentCurrY != _agentStopY))) {
                steps++;
                // get agent's current state
                int currentState = _qLearning_FDGS.GetStateFromCoordinates(_agentCurrX, _agentCurrY);
                // get the action for this state
                int action = _qLearning_FDGS.GetAction(currentState);
                // update agent and get next state
                int nextState = UpdateAgentPosition(currentState, action);
                // do learning of the agent - update his Q-function
                _qLearning_FDGS.LearnStep(currentState, action, nextState);

                // set tabu action
                tabuPolicy.SetTabuAction((action + 2) % 4, 1);
            }

            iteration++;
            Debug.Log(string.Format("{0} steps needed for iteration {1}.", steps, iteration));
        }

        _enableControls = true;
        Debug.Log("QL_FDGS training finished. Try to execute the solution.");
    }

    // Update agent position without reward calculation (will be done during learning step)
    private int UpdateAgentPosition(int state, int action) {
        // moving direction
        int dx = 0, dy = 0;

        switch (action) {
            case 0:         // go to north (up)
                dy = 1;
                break;
            case 1:         // go to east (right)
                dx = 1;
                break;
            case 2:         // go to south (down)
                dy = -1;
                break;
            case 3:         // go to west (left)
                dx = -1;
                break;
        }

        var currentCoordinates = _qLearning_FDGS.GetCoordinatesFromState(state);
        _agentNextX = currentCoordinates.Item1 + dx; // calc new X
        _agentNextY = currentCoordinates.Item2 + dy; // calc new Y

        // check new agent's coordinates and set if not hitting a wall
        // or going out of bounds
        if ((_map[_agentNextY, _agentNextX] != 0) ||
            (_agentNextX < 0) || (_agentNextX >= mapWidth) ||
            (_agentNextY < 0) || (_agentNextY >= mapHeight)) {

            return _qLearning_FDGS.GetStateFromCoordinates(currentCoordinates.Item1, currentCoordinates.Item2);
        }

        _agentCurrX = _agentNextX;
        _agentCurrY = _agentNextY;

        return _qLearning_FDGS.GetStateFromCoordinates(_agentNextX, _agentNextY);
    }

    // Update agent position and return reward for the move
    private double UpdateAgentPosition(int action) {
        // default reward is equal to moving reward
        double reward = moveReward;
        // moving direction
        int dx = 0, dy = 0;

        switch (action) {
            case 0:         // go to north (up)
                dy = 1;
                break;
            case 1:         // go to east (right)
                dx = 1;
                break;
            case 2:         // go to south (down)
                dy = -1;
                break;
            case 3:         // go to west (left)
                dx = -1;
                break;
        }

        int agentNextX = _agentCurrX + dx;
        int agentNextY = _agentCurrY + dy;

        // check new agent's coordinates
        if (
            (_map[agentNextY, agentNextX] != 0) ||
            (agentNextX < 0) || (agentNextX >= mapWidth) ||
            (agentNextY < 0) || (agentNextY >= mapHeight)
            ) {
            // we found a wall or got outside of the world
            reward = wallReward;
        } else {
            _agentCurrX = agentNextX;
            _agentCurrY = agentNextY;

            // check if we found the goal
            if ((_agentCurrX == _agentStopX) && (_agentCurrY == _agentStopY))
                reward = goalReward;
        }

        return reward;
    }

    // Get state number from agent's receptors in the specified position
    private int GetStateNumber(int x, int y) {
        int c1 = (_map[y - 1, x - 1] != 0) ? 1 : 0;
        int c2 = (_map[y - 1, x] != 0) ? 1 : 0;
        int c3 = (_map[y - 1, x + 1] != 0) ? 1 : 0;
        int c4 = (_map[y, x + 1] != 0) ? 1 : 0;
        int c5 = (_map[y + 1, x + 1] != 0) ? 1 : 0;
        int c6 = (_map[y + 1, x] != 0) ? 1 : 0;
        int c7 = (_map[y + 1, x - 1] != 0) ? 1 : 0;
        int c8 = (_map[y, x - 1] != 0) ? 1 : 0;

        return c1 |
            (c2 << 1) |
            (c3 << 2) |
            (c4 << 3) |
            (c5 << 4) |
            (c6 << 5) |
            (c7 << 6) |
            (c8 << 7);
    }
}
