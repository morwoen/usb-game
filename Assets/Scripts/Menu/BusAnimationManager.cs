using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using FMODUnity;

public class BusAnimationManager : MonoBehaviour
{
  [Header("Tyre animation")]
  [SerializeField]
  private Sprite tyreSprite1;
  [SerializeField]
  private Sprite tyreSprite2;
  [SerializeField]
  private SpriteRenderer tyre1;
  [SerializeField]
  private SpriteRenderer tyre2;
  [SerializeField]
  private float spriteSwapSpeed = 1;

  [Header("Door animation")]
  [SerializeField]
  private Transform doorLeft;
  [SerializeField]
  private Transform doorRight;
  [SerializeField]
  private float doorSpeed = 1;
  private float doorOffset = 16;

  private RectTransform rectTransform;
  private Sequence oscillation;
  private Tween intro;

  private DynamicSoundEventManager soundEventManager;

  private void Start() {
    rectTransform = GetComponent<RectTransform>();
    soundEventManager = GetComponent<DynamicSoundEventManager>();

    InvokeRepeating("SwapTyres", spriteSwapSpeed, spriteSwapSpeed);
  }

  private void SwapTyres() {
    if (tyre1.sprite == tyreSprite1) {
      tyre1.sprite = tyreSprite2;
      tyre2.sprite = tyreSprite2;
    } else {
      tyre1.sprite = tyreSprite1;
      tyre2.sprite = tyreSprite1;
    }
  }

  public void MenuIntro() {
    soundEventManager.PlayEvent("event:/bus-leave");
    intro = rectTransform.DOAnchorPos(Vector2.zero, 4)
      .OnComplete(() => {
        oscillation = DOTween.Sequence()
          .Append(rectTransform.DOAnchorPos3DY(-10, 2))
          .Append(rectTransform.DOAnchorPos3DY(0, 2))
          .SetLoops(-1);
      });
  }

  public void MenuOnPlayTransition(Action onComplete) {
    oscillation?.Kill();
    intro?.Kill();
    CameraManager camManager = FindObjectOfType<CameraManager>();
    MenuPlayerController player = FindObjectOfType<MenuPlayerController>();

    // Disable the direction indicator
    player.GetComponentInChildren<SpriteRenderer>().enabled = false;
    soundEventManager.PlayEvent("event:/bus-leave");

    DOTween.Sequence()
      // Exit the screen to the right
      .Append(transform.DOMove(new Vector3(camManager.SizeOfCamera / 2 + 20, transform.position.y), 1))
      // Teleport to the left of the screen
      .AppendCallback(() => transform.position = new Vector3(-camManager.SizeOfCamera / 2 - 20, transform.position.y))
      // Go to the player location
      .Append(transform.DOMove(player.transform.position, 1))
      // Open doors
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-leave");
        soundEventManager.PlayEvent("event:/bus-door");
      })
      .Append(doorLeft.DOLocalMoveX(doorLeft.localPosition.x - doorOffset, doorSpeed))
      .Join(doorRight.DOLocalMoveX(doorRight.localPosition.x + doorOffset, doorSpeed))
      // Hide player
      .AppendCallback(() => player.Hide())
      .AppendInterval(0.5f)
      // Close doors
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-door");
        soundEventManager.PlayEvent("event:/bus-door");
      })
      .Append(doorLeft.DOLocalMoveX(doorLeft.localPosition.x, doorSpeed))
      .Join(doorRight.DOLocalMoveX(doorRight.localPosition.x, doorSpeed))
      // Exit the screen
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-door");
        soundEventManager.PlayEvent("event:/bus-leave");
      })
      .Append(transform.DOMove(new Vector3(camManager.SizeOfCamera / 2 + 20, transform.position.y), 1))
      // Transition scene
      .OnComplete(() => onComplete());
  }

  public void GameIntroSetup() {
    CameraManager camManager = FindObjectOfType<CameraManager>();
    PlayerController player = FindObjectOfType<PlayerController>();
    player.Hide();
    transform.position = new Vector3(-camManager.SizeOfCamera / 2 - 20, transform.position.y);

    DynamicSoundEventManager soundEventManager = GetComponent<DynamicSoundEventManager>();
    if (!soundEventManager.IsPlaying("event:/bus-leave")) {
      soundEventManager.PlayEvent("event:/bus-leave");
    }
  }

  public Sequence GameIntro() {
    CameraManager camManager = FindObjectOfType<CameraManager>();
    PlayerController player = FindObjectOfType<PlayerController>();

    return DOTween.Sequence()
      // Go to the player location
      .Append(transform.DOMove(player.transform.position, 2))
      // Open doors
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-leave");
        soundEventManager.PlayEvent("event:/bus-door");
      })
      .Append(doorLeft.DOLocalMoveX(doorLeft.localPosition.x - doorOffset, doorSpeed))
      .Join(doorRight.DOLocalMoveX(doorRight.localPosition.x + doorOffset, doorSpeed))
      // Hide player
      .AppendCallback(() => player.Show())
      // Close doors
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-door");
        soundEventManager.PlayEvent("event:/bus-door");
      })
      .Append(doorLeft.DOLocalMoveX(doorLeft.localPosition.x, doorSpeed))
      .Join(doorRight.DOLocalMoveX(doorRight.localPosition.x, doorSpeed))
      // Leave the sceen
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-door");
        soundEventManager.PlayEvent("event:/bus-leave");
      })
      .Append(transform.DOMove(new Vector3(camManager.SizeOfCamera / 2 + 20, transform.position.y), 2))
      // Destory the bus as we don't use it past this point
      .AppendCallback(() => {
        soundEventManager.StopEvent("event:/bus-leave");
        Destroy(gameObject);
      });
  }
}
