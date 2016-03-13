using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UILocalizedText : MonoBehaviour {
	[SerializeField]
	private string localizationKey = "";

	public System.Action<UILocalizedText> OnRefresh = null;

	public Text SelfText {
		get;
		private set;
	}

	public void Refresh() {
		SelfText.text = LanguageController.Localize(localizationKey);
		if (OnRefresh != null) {
			OnRefresh(this);
		}
	}

	private void Awake() {
		SelfText = GetComponent<Text>();
		Refresh();
	}
}
