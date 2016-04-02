public class UIButtonCloseDialog : UIButton {
	protected override void OnClick() {
		var dialog = GetComponentInParent<UIDialogBase>();
		if (dialog != null) {
			dialog.Close();
		}
	}
}
