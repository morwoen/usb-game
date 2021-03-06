using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using FMODUnity;

public class TransitionManager : MonoBehaviour
{
  private static TransitionManager instance;

  [SerializeField]
  private Image background;
  [SerializeField]
  private TextMeshProUGUI text;
  [SerializeField]
  private BusAnimationManager bus;

  private bool isTransitioning;
  private bool initial;
  private Tween currentAnimation;
  private Color textColor = new Color(1, 0.337f, 0.968f);
  private DynamicSoundEventManager soundManager;

  private void Awake() {
    instance = this;
    isTransitioning = true;
    initial = true;
    background.color = Color.black;
    text.color = textColor;
    soundManager = GetComponent<DynamicSoundEventManager>();

    MissionManager.ProgressReport progressReport = MissionManager.GetProgressReport();
    string txt = $"There are {progressReport.totalMissions} ways a virus can terminate a company\n";
    if (progressReport.completedMissions == 0) {
      txt += "Can you find them all?";
    } else {
      txt += $"You have found {progressReport.completedMissions} of them";
    }

    text.text = txt;

    bus.GameIntroSetup();
    Invoke("PlayIntro", 0.1f);
  }

  private void PlayIntro() {
    ParticleSystemRenderer playerRenderer = FindObjectOfType<PlayerController>().GetComponentInChildren<ParticleSystemRenderer>();

    playerRenderer.sortingLayerName = "BusLayer";
    // Fade the transition
    currentAnimation = DOTween.Sequence()
      .Append(bus.GameIntro())
      .AppendInterval(3)
      .Append(DOTween.ToAlpha(() => textColor, (c) => text.color = c, 0, 1))
      .Append(DOTween.ToAlpha(() => Color.black, (c) => background.color = c, 0, 1))
      .OnComplete(() => {
        playerRenderer.sortingLayerName = "Player";
        isTransitioning = false;
        initial = false;
        currentAnimation = null;
      });
  }

  private void Update() {
    if (initial && currentAnimation != null && Input.GetMouseButtonDown(0)) {
      currentAnimation.Kill();
      currentAnimation = null;
      initial = false;
      isTransitioning = false;
      background.color = new Color(0, 0, 0, 0);
      text.color = new Color(textColor.r, textColor.g, textColor.b, 0);

      PlayerController player = FindObjectOfType<PlayerController>();
      ParticleSystemRenderer playerRenderer = player.GetComponentInChildren<ParticleSystemRenderer>();
      playerRenderer.sortingLayerName = "Player";
      player.Show();
      if (bus) {
        Destroy(bus.gameObject);
      }
    }
  }

  private void ShowTransitionAndReload() {
    isTransitioning = true;
    Sequence seq = DOTween.Sequence()
      .Append(DOTween.ToAlpha(() => Color.black, (c) => background.color = c, 1, 1))
      .Append(DOTween.ToAlpha(() => text.color, (c) => text.color = c, 1, 1))
      .AppendInterval(5)
      .OnComplete(() => {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      });
  }

  private void WinGame(Mission mission) {
    text.text = $"You got them!\n{mission.description}";
    ShowTransitionAndReload();
  }

  private void LoseGame() {
    soundManager.PlayEvent("event:/death");
    text.text = "You got caught by the Antivirus\nBe more careful next time";
    ShowTransitionAndReload();
  }

  public static void Win(Mission mission) {
    instance?.WinGame(mission);
  }

  public static void Lose() {
    instance?.LoseGame();
  }

  public static bool IsTransitioning() {
    return instance?.isTransitioning == true;
  }
}
