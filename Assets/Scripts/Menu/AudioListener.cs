using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioListener : MonoBehaviour
{
  private StudioEventEmitter studioEvent;

  private void Awake() {
    if (FindObjectsOfType<AudioListener>().Length > 1) {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);
    studioEvent = GetComponent<StudioEventEmitter>();
    if (!studioEvent.IsPlaying()) {
      studioEvent.Play();
    }
  }
}
