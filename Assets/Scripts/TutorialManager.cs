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

  private void Awake() {
    tutorial1.SetActive(false);
    tutorial2.SetActive(false);
    tutorial3.SetActive(false);
    tutorial4.SetActive(false);
  }

  // On reception desk
  // Scan network to reveal computers on the network

  // After that
  // Click to move towards other computers

  // On computer with mission
  // some computers have special actions that progress your mission

  // First time on Roomba/Water/Coffee/Door
  // Antivirus software can't reach IOT devices such as Roombas, Water Dispensers, Coffee Dispensers or Doors
}
