using Heyzap;

public class AdsController : AbstractSingletonBehaviour<AdsController, AdsController> {
	private bool isAdBannerVisible = false;

	public bool IsAdBannerVisible {
		get {
			return isAdBannerVisible;
		}
		set {
			isAdBannerVisible = value;
			if (isAdBannerVisible) {
				HZBannerAd.ShowWithOptions(new HZBannerShowOptions {
					Position = HZBannerShowOptions.POSITION_BOTTOM,
				});
			} else {
				HZBannerAd.Hide();
			}
		}
	}

	private void Awake() {
		//HeyzapAds.ShowDebugLogs();
		//HeyzapAds.ShowThirdPartyDebugLogs();
		HeyzapAds.Start("ece2341e0547e4d11662e8aadfb64ec1", HeyzapAds.FLAG_NO_OPTIONS);
	}
}
