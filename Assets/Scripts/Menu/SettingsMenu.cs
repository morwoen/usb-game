using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class SettingsMenu : MonoBehaviour
{
  [Header("Music")]
  [SerializeField]
  private Slider musicSlider;
  [SerializeField]
  private TextMeshProUGUI musicText;


  [Header("Sound")]
  [SerializeField]
  private Slider soundSlider;
  [SerializeField]
  private TextMeshProUGUI soundText;

  private StudioEventEmitter backgroundMusic;

  private void Awake() {
    backgroundMusic = FindObjectOfType<StudioEventEmitter>();
    float musicVolume = DataManager.GetMusicVolume();
    float soundVolume = DataManager.GetSoundVolume();

    musicSlider.value = musicVolume;
    soundSlider.value = soundVolume;

    musicText.text = VolumeToText(musicSlider.value);
    soundText.text = VolumeToText(soundSlider.value);

    backgroundMusic.SetParameter("MusicVolume", musicSlider.value);

    // TODO: Update fmod
  }

  private string VolumeToText(float value) {
    return ((int)(value * 100)).ToString();
  }

  public void OnMusicChange() {
    musicText.text = VolumeToText(musicSlider.value);
    DataManager.SetMusicVolume(musicSlider.value);

    backgroundMusic.SetParameter("MusicVolume", musicSlider.value);
  }

  public void OnSoundChange() {
    soundText.text = VolumeToText(soundSlider.value);
    DataManager.SetSoundVolume(soundSlider.value);

    // TODO: Update fmod
  }

  public void Save() {
    DataManager.Save();
  }
}
