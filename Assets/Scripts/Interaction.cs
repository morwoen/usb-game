using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
  private static Interaction instance;
  public static bool IsInteracting
  {
    get
    {
      return instance?.tween != null;
    }
  }

  [SerializeField]
  private Slider slider;
  [SerializeField]
  private Image sliderImage;

  private float fadeInTime = 1;

  private TweenerCore<float, float, FloatOptions> tween;
  private Action<bool> callback;

  private void Awake() {
    ResetSlider();
  }

  private void OnEnable() {
    instance = this;
  }

  private void InterruptCast() {
    tween?.Kill();
  }

  private void ResetSlider() {
    sliderImage.DOColor(new Color(sliderImage.color.r, sliderImage.color.g, sliderImage.color.b, 0), fadeInTime);
    tween = null;
    callback = null;
  }

  private void InteractCast(float time, Action<bool> callback) {
    tween?.Kill();

    this.callback = callback;

    sliderImage.DOColor(new Color(sliderImage.color.r, sliderImage.color.g, sliderImage.color.b, 1), fadeInTime * (1 - sliderImage.color.a));
    slider.value = 0;
    tween = slider.DOValue(1, time)
      .SetEase(Ease.Linear)
      .OnKill(OnTweenKilled)
      .OnComplete(OnTweenCompleted);
  }

  private void OnTweenCompleted() {
    if (callback != null) callback(true);
    ResetSlider();
  }

  private void OnTweenKilled() {
    if (callback != null) callback(false);
    ResetSlider();
  }

  public static void Interact(float time, Action<bool> callback) {
    instance?.InteractCast(time, callback);
  }

  public static void Interrupt() {
    instance?.InterruptCast();
  }
}
