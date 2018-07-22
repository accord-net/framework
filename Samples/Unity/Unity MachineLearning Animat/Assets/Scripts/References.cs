using UnityEngine;
using UnityEngine.UI;

public class References : MonoBehaviour {
    public Dropdown LevelSelection;
    public Dropdown LearningAlgorithm;
    public Text WorldSize;
    public InputField InitialExplorationRate;
    public InputField InitialLearningRate;
    public InputField LearningIterations;
    public Text CurrentIteration;

    public Button StartTraining;
    public Button Stop;
    public Button ShowSolution;

    public GameObject[] Levels;

    public void OnLevelDropdown_ValueChanged() {
        for (int i = 0; i < Levels.Length; i++) {
            Levels[i].SetActive(false);
        }
        Levels[LevelSelection.value].SetActive(true);
    }

    public void EnableControls(bool enable) {
        LevelSelection.enabled = enable;
        LearningAlgorithm.enabled = enable;
        InitialExplorationRate.enabled = enable;
        InitialLearningRate.enabled = enable;
        LearningIterations.enabled = enable;

        StartTraining.enabled = enable;
        ShowSolution.enabled = enable;
        Stop.enabled = !enable;
    }
}
