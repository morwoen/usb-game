using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
  private static TransitionManager instance;

  [SerializeField]
  private Image background;
  [SerializeField]
  private TextMeshProUGUI text;

  private bool isTransitioning;
  private bool initial;
  private Tween currentAnimation;
  private Color textColor = new Color(1, 0.337f, 0.968f);

  private void Awake() {
    instance = this;
    isTransitioning = true;
    initial = true;
    background.color = Color.black;
    text.color = textColor;

    MissionManager.ProgressReport progressReport = MissionManager.GetProgressReport();
    string txt = $"There are {progressReport.totalMissions} ways a virus can terminate a company\n";
    if (progressReport.completedMissions == 0) {
      txt += "Can you find them all?";
    } else {
      txt += $"You have found {progressReport.completedMissions} of them";
    }


    text.text = txt;

    // Fade the transition
    currentAnimation = DOTween.Sequence()
      .AppendInterval(5)
      .Append(DOTween.ToAlpha(() => textColor, (c) => text.color = c, 0, 1))
      .Append(DOTween.ToAlpha(() => Color.black, (c) => background.color = c, 0, 1))
      .OnComplete(() => {
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
    text.text = "You got caught\nBe more careful next time";
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
