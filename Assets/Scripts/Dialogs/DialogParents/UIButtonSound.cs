public class UIButtonSound : UIButton {
	protected override void OnClick() {
		SoundController.Instance.IsSoundEnabled = !SoundController.Instance.IsSoundEnabled;
	}

	private void Start() {
		var toggle = GetComponent<UICustomToggle>();
		if (toggle != null) {
			toggle.IsChecked = SoundController.Instance.IsSoundEnabled;
		}
	}
}
