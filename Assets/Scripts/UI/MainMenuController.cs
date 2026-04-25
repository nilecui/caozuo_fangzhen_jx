using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private static readonly string[] EquipmentConfigMap =
    {
        "sample_training.json",
        "sample_training.json",
        "sample_training.json",
        "sample_training.json",
    };

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var nameField = root.Q<TextField>("field-name");
        var idField   = root.Q<TextField>("field-id");
        var dropdown  = root.Q<DropdownField>("dropdown-equipment");

        root.Q<Button>("btn-structure").RegisterCallback<ClickEvent>(_ =>
            Navigate("StructureDisplay", nameField.value, idField.value, dropdown.index));
        root.Q<Button>("btn-principle").RegisterCallback<ClickEvent>(_ =>
            Navigate("PrincipleDemo", nameField.value, idField.value, dropdown.index));
        root.Q<Button>("btn-training").RegisterCallback<ClickEvent>(_ =>
            StartMode("OperationTraining", nameField.value, idField.value, dropdown.index, TrainingMode.Training));
        root.Q<Button>("btn-assessment").RegisterCallback<ClickEvent>(_ =>
            StartMode("OperationTraining", nameField.value, idField.value, dropdown.index, TrainingMode.Assessment));
        root.Q<Button>("btn-scores").RegisterCallback<ClickEvent>(_ =>
            SceneLoader.Instance.LoadScene("ScoreManagement"));
    }

    private void Navigate(string scene, string name, string id, int equipIdx)
    {
        SaveSession(name, id, equipIdx);
        SceneLoader.Instance.LoadScene(scene);
    }

    private void StartMode(string scene, string name, string id, int equipIdx, TrainingMode mode)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id))
        {
            Debug.LogWarning("[MainMenu] 请先填写学员姓名和工号");
            return;
        }
        SaveSession(name, id, equipIdx);
        AppManager.Instance.Session.SelectedEquipmentType = EquipmentConfigMap[equipIdx];
        SceneLoader.Instance.LoadScene(scene);
    }

    private void SaveSession(string name, string id, int equipIdx)
    {
        AppManager.Instance.Session.TraineeName = name;
        AppManager.Instance.Session.TraineeId   = id;
    }
}
