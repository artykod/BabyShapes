using UnityEngine;

public class UIButtonInApp : UIButton {
	public enum InAppProduct {
		Unknown,
		Developers,
		Artists,
		Testers,
	}

	[SerializeField]
	private InAppProduct product = InAppProduct.Unknown;
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
		Debug.Log("TODO: Purchase product: " + product);
	}
}
