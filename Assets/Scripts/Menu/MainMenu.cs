using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MainMenu : MonoBehaviour
{
  [SerializeField]
  private GameObject mainMenu;
  [SerializeField]
  private GameObject creditsMenu;
  [SerializeField]
  private GameObject settingsMenu;
  [SerializeField]
  private GameObject progressMenu;

  [SerializeField]
  private GameObject quitButton;

  private StudioEventEmitter backgroundMusic;

  private MenuPlayerController player;

  private void Awake() {
    player = FindObjectOfType<MenuPlayerController>();
    backgroundMusic = FindObjectOfType<StudioEventEmitter>();
    backgroundMusic.SetParameter("inGame", 0);

    SwitchToMain();

#if UNITY_WEBGL
    Destroy(quitButton);
#endif
  }

  public void GoToNode(MenuNode node) {
    MenuNode origin = FindObjectsOfType<MenuNode>().First(n => n.gameObject.activeSelf && n.isRoot);
    player.NavigateBetween(origin.node, node.node);
  }

  public void SwitchToMain() {
    creditsMenu.SetActive(false);
    settingsMenu.SetActive(false);
    progressMenu.SetActive(false);
    mainMenu.SetActive(true);

    player.RegenerateMap();
  }

  public void SwitchToCredits() {
    creditsMenu.SetActive(true);
    settingsMenu.SetActive(false);
    progressMenu.SetActive(false);
    mainMenu.SetActive(false);

    player.RegenerateMap();
  }

  public void SwitchToSettings() {
    creditsMenu.SetActive(false);
    settingsMenu.SetActive(true);
    progressMenu.SetActive(false);
    mainMenu.SetActive(false);

    player.RegenerateMap();
  }

  public void SwitchToProgress() {
    creditsMenu.SetActive(false);
    settingsMenu.SetActive(false);
    progressMenu.SetActive(true);
    mainMenu.SetActive(false);

    player.RegenerateMap();
  }

  public void Play() {
    SceneManager.LoadScene(1);
    backgroundMusic.SetParameter("inGame", 1);
  }

  public void Quit() {
    Application.Quit();
  }
}
