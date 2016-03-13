public class UIButtonRateUs : UIButton {
	protected override void OnClick() {
		GameCore.GoToAppPageInStore(GameCore.ApplicationStoreId);
	}
}
