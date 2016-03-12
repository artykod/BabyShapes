using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UICustomToggleSprites))]
public class UICustomToggle : Toggle {
	public bool IsChecked {
		get {
			return isOn;
		}
		set {
			isOn = value;
			Refresh(isOn);
		}
	}

	protected override void Awake() {
		base.Awake();
		onValueChanged.RemoveListener(Refresh);
		onValueChanged.AddListener(Refresh);
	}

	private void Refresh(bool value) {
		var sprites = GetComponent<UICustomToggleSprites>();
		image.sprite = isOn ? sprites.SpriteOn : sprites.SpriteOff;
	}
}
