using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : NodeHolder
{
  [SerializeField]
  private Sprite spriteOpen;
  [SerializeField]
  private Sprite spriteClosed;

  private void OnTriggerEnter2D(Collider2D collision) {
    spriteRenderer.sprite = spriteOpen;
    spriteRenderer.transform.localPosition = new Vector3(-0.5f, 0.5f);
  }

  private void OnTriggerExit2D(Collider2D collision) {
    spriteRenderer.sprite = spriteClosed;
    spriteRenderer.transform.localPosition = new Vector3(0, 0.5f);
  }
}
