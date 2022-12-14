using UnityEngine;
using System.Collections;

public class GameCore : MonoBehaviour {
	public static bool IsPaidAppVersion {
		get {
#if PAID
			return true;
#else
			return false;
#endif
		}
	}

	public static string ApplicationStoreId {
		get {
#if UNITY_ANDROID
			return IsPaidAppVersion ? "com.oriplay.babyshapes.paid" : "com.oriplay.babyshapes";
#elif UNITY_IOS
			return IsPaidAppVersion ? "1099447638" : "1099447638";
#elif UNITY_WSA
			return IsPaidAppVersion ? "9nblggh4nhm2" : "9nblggh4nhm2";
#else
			//return Application.appId;
			return "";
#endif
		}
	}

	public static void GoToAppPageInStore(string storeId) {
#if UNITY_WSA
		Application.OpenURL(string.Format("https://www.microsoft.com/store/apps/{0}", storeId));
#elif UNITY_ANDROID
		Application.OpenURL(string.Format("market://details?id={0}&reviewId=0", storeId));
#elif UNITY_IOS
		Application.OpenURL(string.Format("http://itunes.apple.com/app/id{0}?mt=8", storeId));
#endif
	}

	private void Awake() {
		AnalyticsTracker.GameAppStart();

		Application.targetFrameRate = 60;
		LanguageController.Instance.Initialize();
		SoundController.StartButtonsClickTracker();

		Debug.Log("Is someone purchase done: " + PurchasesManager.Instance.IsSomeonePurchaseDone);
	}

	private IEnumerator Start() {
		var dialog = UIDialogHello.ShowSingle();
		SoundController.Voice(SoundController.VOICE_HELLO);
		yield return new WaitForSeconds(1.25f);
		dialog.Close();

		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (UIDialogText.CurrentDialog == null) {
				var dialog = UIDialogText.ShowSingle().Build("Exit", "Exit from game?", "Yes", "No");
				dialog.OnYesClick += () => {
					Debug.Log("Quit from game clicked");
					Application.Quit();
				};
			} else {
				UIDialogText.CurrentDialog.Close();
			}
		}
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Utils/Clear prefs")]
#endif
	private static void ClearUserPrefs() {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
