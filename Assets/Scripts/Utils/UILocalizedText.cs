using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UILocalizedText : MonoBehaviour {
	[SerializeField]
	private string localizationKey = "";

	private Text text = null;

	private void Awake() {
		text = GetComponent<Text>();
		text.text = LanguageController.Localize(localizationKey);
	}
}
