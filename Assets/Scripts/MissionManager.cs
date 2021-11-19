using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
  private static MissionManager instance;

  [SerializeField]
  private MissionDatabase database;

  private void OnEnable() {
    instance = this;
  }

  private void OnWin(Mission mission) {
    DataManager.CompleteMission(mission);
    // TODO: win screen
  }

  private static void Load() {
    if (!instance) {
      instance = FindObjectOfType<MissionManager>();
    }
  }

  public static List<Mission.Goal> GetInteractions(Map.Node node) {
    Load();

    List<Mission.Goal> goals = new List<Mission.Goal>();
    foreach (Mission mission in instance.database.missions) {
      foreach (Mission.Goal goal in mission.steps[mission.currentStep].goals) {
        if (goal.targets.Contains(node) && !goal.completed) {
          goals.Add(goal);
        }
      }
    }

    return goals;
  }

  public static void Interact(Mission.Goal goal, Map.Node node) {
    Load();

    Mission mission = instance.database.missions.FirstOrDefault(miss => miss.steps[miss.currentStep].goals.Contains(goal));
    if (mission == null) return;

    goal.targets.Remove(node);
    if (goal.targets.Count == 0) {
      goal.completed = true;
    } else {
      return;
    }

    if (mission.steps[mission.currentStep].goals.All(g => g.completed)) {
      if (mission.currentStep < mission.steps.Count - 1) {
        mission.currentStep += 1;
      } else {
        instance.OnWin(mission);
      }
    }
  }

  public static void PopulateMissions(Map map) {
    Load();

    MissionGenerator.PopulateMissions(map, instance.database.missions.ToList());
  }
}
