using UnityEngine;

public class UICustomToggleSprites : MonoBehaviour {
	[SerializeField]
	private Sprite spriteOn = null;
	[SerializeField]
	private Sprite spriteOff = null;

	public Sprite SpriteOn {
		get {
			return spriteOn;
		}
	}

	public Sprite SpriteOff {
		get {
			return spriteOff;
		}
	}
}
