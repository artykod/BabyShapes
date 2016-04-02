using UnityEngine;
using UnityEngine.UI;

public class UIButtonParents : UIButton {
	[SerializeField]
	private Text hintText = null;

	private float hideTimeout = 0f;

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

		hideTimeout = 0f;
		hintText.enabled = true;
	}

	protected override void OnUp() {
		base.OnUp();

		hideTimeout = 2f;
	}

	protected override void Update() {
		base.Update();

		if (hideTimeout > 0f) {
			hideTimeout -= Time.deltaTime;
			if (hideTimeout <= 0f) {
				hintText.enabled = false;
			}
		}
	}
}
