using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerNodeRenderer : MonoBehaviour
{
  private void Awake() {
    GetComponent<NodeHolder>().spriteRenderer = transform.GetChild(Random.Range(0, transform.childCount)).gameObject.GetComponent<SpriteRenderer>();
  }
}
