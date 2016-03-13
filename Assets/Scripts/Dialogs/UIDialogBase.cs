using UnityEngine;

public class UIDialogBase : MonoBehaviour {
	[SerializeField]
	private Transform dialogContentRoot = null;

	private CanvasGroup canvasGroup = null;
	private Vector3 endScale = Vector3.one;
	private System.Action onScaleAnimationEndAction = null;

	public virtual void Show() {
		if (dialogContentRoot != null) {
			dialogContentRoot.localScale = Vector3.one * 5f;
			endScale = Vector3.one;
		}
	}

	public virtual void Close() {
		if (dialogContentRoot != null) {
			canvasGroup = dialogContentRoot.GetComponent<CanvasGroup>();
			dialogContentRoot.localScale = Vector3.one;
			endScale = Vector3.one * 2f;
			onScaleAnimationEndAction = () => Destroy(gameObject);
		}
	}

	protected virtual void Awake() {
		var canvas = GetComponent<Canvas>();
		if (canvas != null) {
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = Camera.main;
		}
	}

	private void Update() {
		if (dialogContentRoot != null) {
			dialogContentRoot.localScale = Vector3.Lerp(dialogContentRoot.localScale, endScale, 0.2f);

			if (canvasGroup != null) {
				canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, 0.2f);
			}

			if (Mathf.Abs(dialogContentRoot.localScale.x - endScale.x) < 0.01f) {
				if (onScaleAnimationEndAction != null) {
					onScaleAnimationEndAction();
					onScaleAnimationEndAction = null;
				}
			}
		}
	}
}
