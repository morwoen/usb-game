using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionsRenderer : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI text;
  [SerializeField]
  private RectTransform background;

  public void UpdateActions(Map map) {
    List<Mission.Goal> actions = MissionManager.GetInteractions(map.CurrentNode);
    bool networkUndiscovered = map.CurrentNode.links.FirstOrDefault(node => !node.isKnown) != null;

    background.gameObject.SetActive(actions.Count > 0 || networkUndiscovered);
    if (actions.Count > 0 || networkUndiscovered) {
      string txt = "";
      int index = 1;
      if (networkUndiscovered) {
        txt += $"{index++} • Scan the network\n";
      }
      actions.ForEach(goal => {
        txt += $"{index++} • {goal.message}\n";
      });

      text.SetText(txt);
      text.ForceMeshUpdate();
      background.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
      background.anchoredPosition = new Vector2(text.preferredWidth / 2, - text.preferredHeight / 2);
    }
  }

  public void HideActions() {
    background.gameObject.SetActive(false);
  }
}
