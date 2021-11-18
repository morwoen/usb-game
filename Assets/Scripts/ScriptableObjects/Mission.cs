using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Mission")]
public class Mission : ScriptableObject
{
  public Step currentStep { get; private set; }
  public List<Step> steps;

  private void Awake() {
    currentStep = steps[0];
  }

  [Serializable]
  public class Step
  {
    public List<Goal> goals;
  }

  [Serializable]
  public class Goal
  {
    public enum GoalType
    {
      GoTo,
      GatherData,
      Hack,
    }

    public GoalType type;
    [Header("Use {targets} to be replaced with the number of remaining targets")]
    public string message;
    [Header("Will be populated with random nodes of the given type at runtime")]
    public List<Map.Node> targets;

    public Goal(GoalType type, List<Map.Node> targets) {
      this.type = type;
      this.targets = targets;
    }
  }
}
