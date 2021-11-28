using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressMenuPanel : MonoBehaviour
{
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private MissionDatabase database;

  private void Start() {
    foreach (Mission mission in database.missions) {
      MenuMissionPanelRenderer renderer = Instantiate(prefab, transform).GetComponent<MenuMissionPanelRenderer>();
      renderer.Render(mission);
    }
  }
}
