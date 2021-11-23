using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DontDestroyOnLoad : MonoBehaviour
{
  private StudioEventEmitter studioEvent;

  private void Awake() {
    DontDestroyOnLoad(gameObject);
    studioEvent = GetComponent<StudioEventEmitter>();
    if (!studioEvent.IsPlaying()) {
      studioEvent.Play();
    }
  }
}
