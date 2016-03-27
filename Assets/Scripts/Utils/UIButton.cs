using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UIButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
	private float clickStartTime = 0f;
	private bool clicked = false;
	private bool clickHandled = false;

	protected abstract void OnClick();

	protected virtual void OnDown() {
		// for override
	}

	protected virtual void OnUp() {
		// for override
	}

	protected virtual float HoldTime {
		get {
			return -1f;
		}
	}

	protected virtual void Awake() {
		// for override
	}

	protected virtual void Update() {
		if (HoldTime > 0f && clicked && !clickHandled && Time.unscaledTime - clickStartTime > HoldTime) {
			Click();
		}
	}

	private void Click() {
		if (!clickHandled) {
			var button = GetComponent<Button>();
			var canClick = button != null ? button.interactable : true;
			if (canClick) {
				clickHandled = true;
				OnClick();
				SoundController.Sound(SoundController.SOUND_BUTTON_CLICK);
			}
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if (HoldTime <= 0f) {
			Click();
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		clickHandled = false;
		clicked = true;
		clickStartTime = Time.unscaledTime;
		OnDown();
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		clicked = false;
		OnUp();
	}
}
