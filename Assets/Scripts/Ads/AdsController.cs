using Heyzap;
using UnityEngine;

public class AdsController : AbstractSingletonBehaviour<AdsController, AdsController> {
	private const string PREFS_GAME_STARTS_COUNT = "gmsstrtscnt";

	private bool isAdBannerVisible = false;

	private int GameStartsCount {
		get {
			return PlayerPrefs.GetInt(PREFS_GAME_STARTS_COUNT, 0);
		}
		set {
			PlayerPrefs.SetInt(PREFS_GAME_STARTS_COUNT, value);
			PlayerPrefs.Save();
		}
	}

	public bool IsAdBannerVisible {
		get {
			return isAdBannerVisible;
		}
		set {
			isAdBannerVisible = value && GameStartsCount > 1 && !PurchasesManager.Instance.IsSomeonePurchaseDone;

#if UNITY_ANDROID
			if (isAdBannerVisible) {
				HZBannerAd.ShowWithOptions(new HZBannerShowOptions {
					Position = HZBannerShowOptions.POSITION_BOTTOM,
				});
			} else {
				HZBannerAd.Hide();
			}
#endif
		}
	}

	private void Awake() {
		GameStartsCount++;

#if UNITY_ANDROID
		HeyzapAds.ShowDebugLogs();
		HeyzapAds.ShowThirdPartyDebugLogs();
		HeyzapAds.Start("ece2341e0547e4d11662e8aadfb64ec1", HeyzapAds.FLAG_NO_OPTIONS);

		//HeyzapAds.ShowMediationTestSuite ();
#endif
	}
}
