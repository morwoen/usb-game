using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : NodeHolder
{
  [SerializeField]
  private SpriteRenderer sprite;

  private void OnTriggerEnter2D(Collider2D collision) {
    //sprite.enabled = false;
  }

  private void OnTriggerExit2D(Collider2D collision) {
    //sprite.enabled = Node.isKnown;
  }
}
