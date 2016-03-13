using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UIButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler {
	private float clickStartTime = 0f;

	protected abstract void OnClick();

	protected virtual float HoldTime {
		get {
			return -1f;
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		var button = GetComponent<Button>();
		var canClick = button != null ? button.interactable : true;

		if (canClick && (Time.unscaledTime - clickStartTime) > HoldTime) {
			OnClick();
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		clickStartTime = Time.unscaledTime;
	}
}
