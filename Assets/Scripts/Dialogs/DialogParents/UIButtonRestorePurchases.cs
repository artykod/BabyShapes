public class UIButtonRestorePurchases : UIButton {
	protected override void Awake() {
#if !UNITY_IOS
		Destroy(gameObject);
#endif
	}

	protected override void OnClick() {
		PurchasesManager.Instance.RestorePurchases();
	}
}
