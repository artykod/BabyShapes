using UnityEngine;
using System.Collections;

#if UNITY_WSA //&& !UNITY_EDITOR
using MarkerMetro.Unity.WinIntegration;
#endif

public class WSADialog : IDialogInterface {
#if UNITY_WSA //&& !UNITY_EDITOR
	private static string positiveButton = "";
	private static string negativeButton = "";

	public void Show ( string message, PlatformDialog.Type buttonType ) {
		Show("", message, buttonType);
	} 

	public void Show ( string title, string message, PlatformDialog.Type buttonType ) {
		string buttonOk = positiveButton;
		string buttonCancel = buttonType == PlatformDialog.Type.OKCancel ? negativeButton : "";

		Helper.Instance.ShowDialog(message, title, (isOk) => {
			try {
				if (isOk) {
					PlatformDialog.Instance.OnPositive(buttonOk);
				} else {
					PlatformDialog.Instance.OnNegative(buttonCancel);
				}
			} catch {
				//
			}
		}, buttonOk, buttonCancel);
	}

	public void SetButtonLabel( string positive ) {
		positiveButton = positive;
	}

	public void SetButtonLabel( string positive, string negative ) {
		positiveButton = positive;
		negativeButton = negative;
	}

	public void Dismiss() {
		//
	}
#else
	public void Show (string message, PlatformDialog.Type buttonType) {}
	public void Show (string title, string message, PlatformDialog.Type buttonType) {}
	public void SetButtonLabel( string positive ) {}
	public void SetButtonLabel( string positive, string negative ) {}
	public void Dismiss() {}
#endif
}
