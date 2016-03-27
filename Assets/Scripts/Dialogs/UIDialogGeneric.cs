using UnityEngine;

public abstract class UIDialogGeneric<T> : UIDialogBase where T : UIDialogBase {
	public static T CurrentDialog {
		get;
		private set;
	}

	public static T ShowSingle(bool animated = true) {
		DestroyCurrent();
		return Show(animated);
	}

	public static void DestroyCurrent() {
		if (CurrentDialog != null) {
			var current = CurrentDialog;
			CurrentDialog = null;
			Destroy(current.gameObject);
		}
	}

	public static void CloseCurrent() {
		if (CurrentDialog != null) {
			var current = CurrentDialog;
			CurrentDialog = null;
			current.Close();
		}
	}

	public static T Show(bool animated = true) {
		var prefab = Resources.Load<T>("UI/" + typeof(T).Name);
		if (prefab != null) {
			CurrentDialog = Instantiate(prefab);
			if (animated) {
				CurrentDialog.Show();
			}
		}
		return CurrentDialog;
	}
}
