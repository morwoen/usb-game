using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
  [SerializeField]
  private GameObject tutorial1;
  [SerializeField]
  private GameObject tutorial2;
  [SerializeField]
  private GameObject tutorial3;
  [SerializeField]
  private GameObject tutorial4;

  private bool nodeWithMissionPresented = false;
  private bool clickToMovePresented = false;
  private bool iotPresented = false;

  private PlayerController player;

  private void Awake() {
    player = FindObjectOfType<PlayerController>();

    tutorial1.SetActive(true);
    tutorial2.SetActive(false);
    tutorial3.SetActive(false);
    tutorial4.SetActive(false);
  }

  private void Update() {
    transform.position = new Vector3(transform.position.x, player.transform.position.y + 2);
  }

  public void OnInteract() {
    // On reception desk
    // Scan network to reveal computers on the network
    if (tutorial1.activeSelf) {
      tutorial1.SetActive(false);
    }

    if (tutorial3.activeSelf) {
      tutorial3.SetActive(false);
    }

    if (tutorial4.activeSelf) {
      tutorial4.SetActive(false);
    }
  }

  public void OnAfterInteract() {
    // After that
    // Click to move towards other computers
    if (!clickToMovePresented) {
      clickToMovePresented = true;
      tutorial2.SetActive(true);
    }
  }

  public void OnMove() {
    if (tutorial2.activeSelf) {
      tutorial2.SetActive(false);
    }

    if (tutorial3.activeSelf) {
      tutorial3.SetActive(false);
    }

    if (tutorial4.activeSelf) {
      tutorial4.SetActive(false);
    }
  }

  public void OnNodeWithMissions() {
    // On computer with mission
    // some computers have special actions that progress your mission
    if (!nodeWithMissionPresented && !tutorial1.activeSelf && !tutorial2.activeSelf && !tutorial4.activeSelf) {
      nodeWithMissionPresented = true;
      tutorial3.SetActive(true);
    }
  }

  public void OnIotDevice() {
    // First time on Roomba/Water/Coffee/Door
    // Antivirus software can't reach IOT devices such as Roombas, Water Dispensers, Coffee Dispensers or Doors
    if (!iotPresented && !tutorial1.activeSelf && !tutorial2.activeSelf && !tutorial3.activeSelf) {
      iotPresented = true;
      tutorial4.SetActive(true);
    }
  }
}
