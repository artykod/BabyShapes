using UnityEngine;

public class UIButtonInApp : UIButton {
	[SerializeField]
	private PurchasesManager.InAppProduct product = PurchasesManager.InAppProduct.Unknown;
	[SerializeField]
	private string price = "$0.99";
	[SerializeField]
	private UILocalizedText titleText = null;

	private void Start() {
		if (titleText != null) {
			titleText.OnRefresh += (text) => text.SelfText.text += ": " + price;
			titleText.Refresh();
		}
	}

	protected override void OnClick() {
		PurchasesManager.Instance.BuyProduct(product, ShowThanks, ShowError);
	}

	private void ShowThanks() {
		PlatformDialog.Show("Great!", "Thanks for your support!");
	}

	private void ShowError() {
		PlatformDialog.Show("Sorry", "Error happened. Please try later.");
	}
}
