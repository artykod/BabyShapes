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
			return IsPaidAppVersion ? "1046852687" : "1074030366";
#elif UNITY_WSA
			return IsPaidAppVersion ? "9nblggh5kpw4" : "9nblggh5kpw4";
#else
			return Application.bundleIdentifier;
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
		Application.targetFrameRate = 60;
		LanguageController.Instance.Initialize();
		SoundController.Music(SoundController.MUSIC_STANDARD);
		SoundController.StartButtonsClickTracker();

		Debug.Log("Is someone purchase done: " + PurchasesManager.Instance.IsSomeonePurchaseDone);
	}

	private IEnumerator Start() {
		/*var dialog = UIDialogHello.ShowSingle();
		SoundController.Voice(SoundController.VOICE_HELLO);
		yield return new WaitForSeconds(1.25f);
		dialog.Close();*/

		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);

		yield break;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Utils/Clear prefs")]
#endif
	private static void ClearUserPrefs() {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
