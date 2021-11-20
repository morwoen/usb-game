using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/Mission")]
public class Mission : ScriptableObject
{
  public int currentStep { get; set; }
  public Sprite sprite;
  public string description;
  public List<Step> steps;
  [NonSerialized]
  public bool impossible = false;

  private void Awake() {
    currentStep = 0;
    steps.ForEach(s => s.goals.ForEach(g => g.completed = false));
    impossible = false;
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
      Hack,
    }

    public GoalType type;
    [Header("Use {targets} to be replaced with the number of remaining targets")]
    public string message;
    [Header("Will be populated with random nodes of the given type at runtime")]
    public Map.Node.NodeType targetType;
    public int numberOfTargets;

    [NonSerialized]
    public bool completed = false;
    [NonSerialized]
    public List<Map.Node> targets;
  }
}
