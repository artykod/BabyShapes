using UnityEngine;

public abstract class UIDialogGeneric<T> : UIDialogBase where T : UIDialogBase {
	private static T currentDialog = null;

	public static UIDialogBase ShowSingle(bool animated = true) {
		if (currentDialog != null) {
			Destroy(currentDialog.gameObject);
			currentDialog = null;
		}

		return Show(animated);
	}

	public static T Show(bool animated = true) {
		var prefab = Resources.Load<T>("UI/" + typeof(T).Name);
		if (prefab != null) {
			currentDialog = Instantiate(prefab);
			if (animated) {
				currentDialog.Show();
			}
		}
		return currentDialog;
	}
}
