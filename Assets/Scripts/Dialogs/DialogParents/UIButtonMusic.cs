public class UIButtonMusic : UIButton {
	protected override void OnClick() {
		SoundController.Instance.IsMusicEnabled = !SoundController.Instance.IsMusicEnabled;
	}

	private void Start() {
		var toggle = GetComponent<UICustomToggle>();
		if (toggle != null) {
			toggle.IsChecked = SoundController.Instance.IsMusicEnabled;
		}
	}
}
