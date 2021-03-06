using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MissionGenerator : MonoBehaviour
{
  public static void PopulateMissions(Map map, List<Mission> missions) {
    foreach (Mission mission in missions) {
      foreach (Mission.Step step in mission.steps) {
        foreach (Mission.Goal goal in step.goals) {
          List<Map.Node> possibleTargets = map.nodes.Where(n => n.Type == goal.targetType).ToList();

          mission.impossible = possibleTargets.Count == 0;
          if (mission.impossible) {
            break;
          }

          if (goal.numberOfTargets == 0) {
            goal.targets = possibleTargets;
          } else {
            goal.targets = GenerateRandom(goal.numberOfTargets, possibleTargets.Count).Select(ind => possibleTargets[ind]).ToList();
          }
        }
        
        if (mission.impossible) {
          break;
        }
      }
    }
  }

  private static List<int> GenerateRandom(int count, int max) {
    HashSet<int> candidates = new HashSet<int>();

    for (int top = max - count; top < max; top++) {
      if (!candidates.Add(Random.Range(0, top + 1))) {
        candidates.Add(top);
      }
    }

    List<int> result = candidates.ToList();

    for (int i = result.Count - 1; i > 0; i--) {
      int k = Random.Range(0, i + 1);
      int tmp = result[k];
      result[k] = result[i];
      result[i] = tmp;
    }
    return result;
  }
}
