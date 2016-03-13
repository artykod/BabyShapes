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
		Debug.Log("TODO show thanks purchase dialog");
	}

	private void ShowError() {
		Debug.Log("TODO show failed purchase dialog");
	}
}
