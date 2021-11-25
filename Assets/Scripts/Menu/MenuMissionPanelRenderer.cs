using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuMissionPanelRenderer : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI spritePanel;
  [SerializeField]
  private TextMeshProUGUI body;
  [SerializeField]
  private Color completedColor;

  public void Render(Mission mission) {
    bool completed = DataManager.IsCompleted(mission);
    if (completed) {
      body.text = mission.description;
      spritePanel.text = "O";
      spritePanel.color = completedColor;
    } else {
      body.text = "Explore the game to complete this mission";
    }
  }
}
