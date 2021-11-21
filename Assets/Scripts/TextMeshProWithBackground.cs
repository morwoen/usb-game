using UnityEngine;
using TMPro;

public class TextMeshProWithBackground : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI text;
  [SerializeField]
  private RectTransform background;
  [SerializeField]
  private bool autoInit = false;
  [SerializeField]
  private bool centered = false;

  private void OnEnable() {
    if (autoInit) {
      SetText(text.text);
    }
  }

  public void SetText(string txt) {
    text.SetText(txt);
    text.ForceMeshUpdate();
    background.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    if (centered) {
      background.anchoredPosition = new Vector2(0, 0);
    } else {
      background.anchoredPosition = new Vector2(text.preferredWidth / 2, -text.preferredHeight / 2);
    }
  }
}
