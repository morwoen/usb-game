using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSoundLoader : MonoBehaviour
{
  [FMODUnity.BankRef]
  public List<string> Banks;

  private void Start() {
    StartCoroutine(LoadGameAsync());
  }

  IEnumerator LoadGameAsync() {
    // Start an asynchronous operation to load the scene
    AsyncOperation async = SceneManager.LoadSceneAsync(1);

    // Don't lead the scene start until all Studio Banks have finished loading
    async.allowSceneActivation = false;

    // Iterate all the Studio Banks and start them loading in the background
    // including the audio sample data
    foreach (var bank in Banks) {
      FMODUnity.RuntimeManager.LoadBank(bank, true);
    }

    // Keep yielding the co-routine until all the Bank loading is done
    while (FMODUnity.RuntimeManager.AnyBankLoading()) {
      yield return null;
    }

    // Allow the scene to be activated. This means that any OnActivated() or Start()
    // methods will be guaranteed that all FMOD Studio loading will be completed and
    // there will be no delay in starting events
    async.allowSceneActivation = true;

    // Keep yielding the co-routine until scene loading and activation is done.
    while (!async.isDone) {
      yield return null;
    }

  }
}
