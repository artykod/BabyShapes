using UnityEngine;

public class UIButtonLanguage : UIButton {
	[SerializeField]
	private Language language = Language.English;

	public Language Language {
		get {
			return language;
		}
	}

	public bool IsChecked {
		get {
			return GetComponent<UICustomToggle>().IsChecked;
		}
		set {
			GetComponent<UICustomToggle>().IsChecked = value;
		}
	}

	protected override void OnClick() {
		LanguageController.Instance.CurrentLanguage = language;
		var allTexts = FindObjectsOfType<UILocalizedText>();
		foreach (var t in allTexts) {
			t.Refresh();
		}
		Resources.UnloadUnusedAssets();
	}
}
