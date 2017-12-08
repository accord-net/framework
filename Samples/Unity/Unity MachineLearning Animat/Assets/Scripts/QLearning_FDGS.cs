using Accord.Compat;
using Accord.MachineLearning;
using System;
using System.Linq;
using UnityEngine;

public class QLearning_FDGS {

    private int _mapWidth;
    private int _mapHeight;

    private IExplorationPolicy _explorationPolicy;
    private double _learningRate = 0.25f;
    private double _discountFactor = 0.95f;

    private double _maxQValueForNextStateActions;

    private double[][] _rewardTable;
    private double[][] _qLearningTable;

    public IExplorationPolicy ExplorationPolicy {
        get {
            return _explorationPolicy;
        }
        set {
            _explorationPolicy = value;
        }
    }

    public double LearningRate {
        get {
            return _learningRate;
        }
        set {
            _learningRate = Math.Max(0.0, Math.Min(1.0, value));
        }
    }

    public double DiscountFactor {
        get {
            return _discountFactor;
        }
        set {
            _discountFactor = Math.Max(0.0, Math.Min(1.0, value));
        }
    }

    public QLearning_FDGS(int actions, int goalX, int goalY, int[,] map, IExplorationPolicy explorationPolicy) {

        _mapWidth = map.GetLength(1);
        _mapHeight = map.GetLength(0);

        _explorationPolicy = explorationPolicy;

        _rewardTable = new double[_mapWidth * _mapHeight][];
        _qLearningTable = new double[_mapWidth * _mapHeight][];

        for (int i = 0; i < _qLearningTable.Length; i++) {
            _rewardTable[i] = new double[actions];
            _qLearningTable[i] = new double[actions];
        }


        InitRewardMatrix();
        InitQLearningMatrix();

        for (int i = 1; i < (_mapWidth - 1); i++) {
            for (int j = 1; j < (_mapHeight - 1); j++) {

                var state = GetStateFromCoordinates(i, j);

                if (state == 67) {
                    Debug.Log("check state values...");
                }

                // set reward according to map (walls, no walls)
                _rewardTable[state][0] = (map[j + 1, i] == 0) && (_rewardTable[state][0] != 1) ? 0 : _rewardTable[state][0];
                _rewardTable[state][1] = (map[j, i + 1] == 0) && (_rewardTable[state][1] != 1) ? 0 : _rewardTable[state][1];
                _rewardTable[state][2] = (map[j - 1, i] == 0) && (_rewardTable[state][2] != 1) ? 0 : _rewardTable[state][2];
                _rewardTable[state][3] = (map[j, i - 1] == 0) && (_rewardTable[state][3] != 1) ? 0 : _rewardTable[state][3];

                // check for goal -> every state before reaching the goal will be rewarded
                if ((i == goalX) && (j == goalY)) {
                    // below goal and moving up
                    _rewardTable[GetStateFromCoordinates(i, j - 1)][0] = map[j - 1, i] == -1 ? -1 : 1;
                    // right from goal and moving left
                    _rewardTable[GetStateFromCoordinates(i + 1, j)][3] = map[j, i + 1] == -1 ? -1 : 1;
                    // above goal and moving down
                    _rewardTable[GetStateFromCoordinates(i, j + 1)][2] = map[j + 1, i] == -1 ? -1 : 1;
                    // left from goal and moving right
                    _rewardTable[GetStateFromCoordinates(i - 1, j)][1] = map[j, i - 1] == -1 ? -1 : 1;
                }
            }
        }
    }

    private void InitRewardMatrix() {
        for (int i = 0; i < _rewardTable.Length; i++) {
            for (int j = 0; j < _rewardTable[i].Length; j++) {
                _rewardTable[i][j] = -1;
            }
        }
    }

    private void InitQLearningMatrix() {
        for (int i = 0; i < _qLearningTable.Length; i++) {
            for (int j = 0; j < _qLearningTable[i].Length; j++) {
                _qLearningTable[i][j] = 0;
            }
        }
    }

    public void LearnStep(int currentState, int currentAction, int nextState) {
        // get maximum of all possible actions, for the next state
        _maxQValueForNextStateActions = GetMaxQValue(nextState);

        // calculate qValues according to
        // Q(s, a) = (1 - alpha) * Q(s, a) + alpha * (R(s, a) + gamma * max(Q(s', a')))
        _qLearningTable[currentState][currentAction] = (1 - _learningRate) * _qLearningTable[currentState][currentAction];
        _qLearningTable[currentState][currentAction] += _learningRate * (_rewardTable[currentState][currentAction] + _discountFactor * _maxQValueForNextStateActions);
    }

    public int GetStateFromCoordinates(int x, int y) {
        return x + (y * _mapWidth);
    }

    public Tuple<int, int> GetCoordinatesFromState(int state) {
        int x = state % _mapWidth;
        int y = state / _mapWidth;
        return Tuple.Create(x, y);
    }

    public int GetAction(int state) {
        return _explorationPolicy.ChooseAction(_rewardTable[state]);
    }

    public int GetLearnedAction(int state) {
        var maxValue = _qLearningTable[state].Max();
        return Array.IndexOf(_qLearningTable[state], maxValue);
    }

    public double GetMaxQValue(int state) {
        return _qLearningTable[state].Max();
    }
}
