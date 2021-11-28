using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  [SerializeField]
  private GameObject menuWrapper;
  [SerializeField]
  private GameObject pauseMenu;
  [SerializeField]
  private GameObject settingsMenu;
  [SerializeField]
  private GameObject settingsMenuBackground;

  private bool isPaused = false;

  private void Start() {
    settingsMenu.SetActive(false);
    settingsMenuBackground.SetActive(false);
    isPaused = true;
    ToggleMenu();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      ToggleMenu();
    }
  }

  private void ToggleMenu() {
    isPaused = !isPaused;
    menuWrapper.SetActive(isPaused);
    Time.timeScale = isPaused ? 0 : 1;
  }

  public void QuitToMenu() {
    Time.timeScale = 1;
    SceneManager.LoadScene(1);
  }

  public void Resume() {
    ToggleMenu();
  }

  public void ShowSettings() {
    pauseMenu.SetActive(false);
    settingsMenu.SetActive(true);
    settingsMenuBackground.SetActive(true);
  }

  public void HideSettings() {
    pauseMenu.SetActive(true);
    settingsMenu.SetActive(false);
    settingsMenuBackground.SetActive(false);
  }
}
