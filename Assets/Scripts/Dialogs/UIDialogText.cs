using UnityEngine;
using UnityEngine.UI;

public class UIDialogText : UIDialogGeneric<UIDialogText> {
	[SerializeField]
	private Text titleText = null;
	[SerializeField]
	private Text messageText = null;
	[SerializeField]
	private Button yesButton = null;
	[SerializeField]
	private Button noButton = null;

	public System.Action OnYesClick = null;
	public System.Action OnNoClick = null;

	public UIDialogText Build(string title, string message, string yesText = null, string noText = null) {
		titleText.text = title;
		messageText.text = message;

		if (yesText != null) {
			yesButton.GetComponentInChildren<Text>().text = yesText;
			yesButton.onClick.AddListener(() => {
				if (OnYesClick != null) {
					OnYesClick();
				}

				Close();
			});
		} else {
			yesButton.gameObject.SetActive(false);
		}

		if (noText != null) {
			noButton.GetComponentInChildren<Text>().text = noText;
			noButton.onClick.AddListener(() => {
				if (OnNoClick != null) {
					OnNoClick();
				}

				Close();
			});
		} else {
			noButton.gameObject.SetActive(false);
		}

		return this;
	}
}
