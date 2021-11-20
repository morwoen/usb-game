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

  private bool isPaused = false;

  private void Awake() {
    settingsMenu.SetActive(false);
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
    SceneManager.LoadScene(0);
  }

  public void Resume() {
    ToggleMenu();
  }

  public void ShowSettings() {
    pauseMenu.SetActive(false);
    settingsMenu.SetActive(true);
  }

  public void HideSettings() {
    pauseMenu.SetActive(true);
    settingsMenu.SetActive(false);
  }
}
