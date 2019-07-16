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

    private Color _disabledColor = Color.gray;
    private Color _enabledColor = Color.black;

    public void OnLevelDropdown_ValueChanged() {
        for (int i = 0; i < Levels.Length; i++) {
            Levels[i].SetActive(false);
        }
        Levels[LevelSelection.value].SetActive(true);
    }

    public void EnableControls(bool enable) {
        LevelSelection.enabled = enable;        
        LearningAlgorithm.enabled = enable;

        LevelSelection.GetComponentInChildren<Text>().color = GetColor(enable);
        LearningAlgorithm.GetComponentInChildren<Text>().color = GetColor(enable);

        InitialExplorationRate.enabled = enable;
        InitialLearningRate.enabled = enable;
        LearningIterations.enabled = enable;

        InitialExplorationRate.textComponent.color = GetColor(enable);
        InitialLearningRate.textComponent.color = GetColor(enable);
        LearningIterations.textComponent.color = GetColor(enable);

        StartTraining.enabled = enable;
        ShowSolution.enabled = enable;
        Stop.enabled = !enable;

        StartTraining.GetComponentInChildren<Text>().color = GetColor(enable);
        ShowSolution.GetComponentInChildren<Text>().color = GetColor(enable);
        Stop.GetComponentInChildren<Text>().color = GetColor(!enable);
    }

    private Color GetColor(bool enable) {
        return enable ? _enabledColor : _disabledColor;
    }
}
