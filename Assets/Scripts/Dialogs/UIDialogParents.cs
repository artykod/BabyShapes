public class UIDialogParents : UIDialogGeneric<UIDialogParents> {
	private UIButtonLanguage[] languageButtons = null;

	private void Awake() {
		languageButtons = GetComponentsInChildren<UIButtonLanguage>();
		var currentLanguage = LanguageController.Instance.CurrentLanguage;
		foreach (var i in languageButtons) {
			if (currentLanguage == i.Language) {
				i.IsChecked = true;
			}
		}
	}
}
