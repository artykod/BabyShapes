public class UIButtonParents : UIButton {
	protected override float HoldTime {
		get {
			return 0f;
		}
	}

	protected override void OnClick() {
		UIDialogParents.ShowSingle();
	}
}
