using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UILocalizedText : MonoBehaviour {
	[SerializeField]
	private string localizationKey = "";

	private Text text = null;

	public void Refresh() {
		text.text = LanguageController.Localize(localizationKey);
	}

	private void Awake() {
		text = GetComponent<Text>();
		Refresh();
	}
}
