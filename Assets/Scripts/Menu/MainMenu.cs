using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using DG.Tweening;

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
  private bool isTransitioning = false;

  private void Awake() {
    player = FindObjectOfType<MenuPlayerController>();
    backgroundMusic = FindObjectOfType<StudioEventEmitter>();
    backgroundMusic.SetParameter("inGame", 0);

#if UNITY_WEBGL
    Destroy(quitButton);
    // The player fails to detect when the object is being destroyed, so instead we trigger this again to reset the map
    Invoke("SwitchToMain", 0.1f);
#endif

    SwitchToMain();

    StartCoroutine(PlayIntro());
  }

  private IEnumerator PlayIntro() {
    BusAnimationManager bus = FindObjectOfType<BusAnimationManager>();
    bus.transform.position = new Vector3(-FindObjectOfType<CameraManager>().SizeOfCamera / 2, bus.transform.position.y + 5);
    yield return new WaitForEndOfFrame();
    bus.MenuIntro();
  }

  public void GoToNode(MenuNode node) {
    if (isTransitioning) return;
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
    if (isTransitioning) {
      return;
    }

    isTransitioning = true;
    FindObjectOfType<BusAnimationManager>().MenuOnPlayTransition(() => {
      DOTween.KillAll();
      backgroundMusic.SetParameter("inGame", 1);
      SceneManager.LoadScene(2);
    });
  }

  public void Quit() {
    Application.Quit();
  }
}
