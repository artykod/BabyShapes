using UnityEngine;

public class UIDialogParents : UIDialogBase {
	[SerializeField]
	private UIButtonLanguage[] languageButtons = null;

	private static UIDialogParents currentDialog = null;

	public static void Open(bool animated = true) {
		if (currentDialog != null) {
			Destroy(currentDialog.gameObject);
			currentDialog = null;
		}

		var prefab = Resources.Load<UIDialogParents>("UI/UIDialogParents");
		if (prefab != null) {
			currentDialog = Instantiate(prefab);
			if (animated) {
				currentDialog.Show();
			}
		}
	}

	private void Awake() {
		var currentLanguage = LanguageController.Instance.CurrentLanguage;
		foreach (var i in languageButtons) {
			if (currentLanguage == i.Language) {
				i.IsChecked = true;
			}
		}
	}
}
