using UnityEngine;

public class RateUsDialog {
	private const string PREFS_RATE_US = "rateuscanshow";

	private static bool IsRatedSuccess {
		get {
			return PlayerPrefs.GetInt(PREFS_RATE_US, 0) > 0;
		}
		set {
			PlayerPrefs.SetInt(PREFS_RATE_US, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void ShowAppPageInStore() {
		IsRatedSuccess = true;
		GameCore.GoToAppPageInStore(GameCore.ApplicationStoreId);
	}

	public static bool TryShowRateUsDialog() {
		if (!IsRatedSuccess) {
			Debug.Log("Show rate us dialog");

			PlatformDialog.Show(
				"Please rate us!",
				"Please rate game in the store!",
				new PlatformDialog.Button("Yes", ShowAppPageInStore),
				new PlatformDialog.Button("No", null)
			);

			return true;
		} else {
			return false;
		}
	}
}
