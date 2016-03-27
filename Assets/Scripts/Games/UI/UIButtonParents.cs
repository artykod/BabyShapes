using UnityEngine;
using UnityEngine.UI;

public class UIButtonParents : UIButton {
	[SerializeField]
	private Text hintText = null;

	protected override float HoldTime {
		get {
			return 1f;
		}
	}

	protected override void Awake() {
		base.Awake();

		hintText.enabled = false;
	}

	protected override void OnClick() {
		UIDialogParents.ShowSingle();
	}

	protected override void OnDown() {
		base.OnDown();

		hintText.enabled = true;
	}

	protected override void OnUp() {
		base.OnUp();

		hintText.enabled = false;
	}
}
