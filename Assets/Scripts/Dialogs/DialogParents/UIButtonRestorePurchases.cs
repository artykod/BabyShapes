public class UIButtonRestorePurchases : UIButton {
	protected override void OnClick() {
		PurchasesManager.Instance.RestorePurchases();
	}
}
