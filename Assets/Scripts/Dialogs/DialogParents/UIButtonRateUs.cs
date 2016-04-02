public class UIButtonRateUs : UIButton {
	protected override void OnClick() {
		AnalyticsTracker.RateUs(AnalyticsTracker.RateUsTypes.ParentsScreen);
		RateUsDialog.ShowAppPageInStore();
	}
}
