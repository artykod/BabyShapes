using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;



/// <summary>
/// Plugin will allow you to add multi platform simple dialog in your project.<br/>
/// <br/>
/// Supported Platforms:<br/>
///		iOS<br/>
///		Android<br/>
///		Web Player<br/>
///		Unity Editor<br/>
/// </summary>
public class PlatformDialog : MonoBehaviour
{
	/// <summary> Dialog Button Types </summary>
	public enum Type {
		SubmitOnly=0,
    	OKCancel=1,
	}
	
	private static PlatformDialog instance;
	public  static PlatformDialog Instance {
		get {
			if (instance == null) {
				instance = new GameObject ("PlatformDialog").AddComponent<PlatformDialog> ();
			}
			return instance;
		}
	}

    private Action positiveDelegate;
    private Action negativeDelegate;

    private IDialogInterface dialog = null;
	
	
	void Awake ()
	{
#if UNITY_EDITOR
		this.dialog = new UnityEditorDialog();
#elif UNITY_ANDROID
		this.dialog = new AndroidDialog();
#elif UNITY_IPHONE
		this.dialog = new iOSDialog();
#elif UNITY_WSA
		this.dialog = new WSADialog();
#elif UNITY_WEBPLAYER
		this.dialog = new WebPlayerDialog();
#endif
	}

	void OnDestroy ()
	{
	}

	public class Button {
		public string Text {
			get;
			private set;
		}
		public Action OnClick {
			get;
			private set;
		}

		public Button(string text, Action onClick) {
			Text = text;
			OnClick = onClick;
		}
	}

	public static void Show(string title, string message, params Button[] buttons) {
		var type = Type.SubmitOnly;
		var buttonOk = new Button("OK", null);
		var buttonCancel = new Button(null, null);

		if (buttons != null && buttons.Length > 0) {
			buttonOk = buttons[0];
			if (buttons.Length > 1) {
				buttonCancel = buttons[1];
				type = Type.OKCancel;
			}
		}

		SetButtonLabel(buttonOk.Text, buttonCancel.Text);
		Show(title, message, type, buttonOk.OnClick, buttonCancel.OnClick);
	}
	
	/// <summary> Show Platform Dialog </summary>
	/// <param name='message'> dialog message </param>
	/// <param name='buttonType'> dialog button type : submit only or OK and Cancel </param>
	/// <param name='positiveDelegate'> delegate: on click positive button </param>
	/// <param name='negativeDelegate'> delegate: on click negative button </param>
	public static void Show (string message, PlatformDialog.Type buttonType, Action positiveDelegate, Action negativeDelegate=null)
	{
		switch( buttonType ) {
		case PlatformDialog.Type.SubmitOnly:
			Instance.positiveDelegate = positiveDelegate;
			Instance.negativeDelegate = positiveDelegate;
			break;
		default:
			Instance.positiveDelegate = positiveDelegate;
			Instance.negativeDelegate = negativeDelegate;
			break;
		}

		Instance.dialog.Show( message, buttonType );
	}
	
	/// <summary> Show Platform Dialog with title</summary>
	/// <remarks> You can display the title, only Android and iOS. Not supported UnityEditor and Web Player.
	/// <param name='title'> dialog title </param>
	/// <param name='message'> dialog message </param>
	/// <param name='buttonType'> dialog button type : submit only or OK and Cancel </param>
	/// <param name='positiveDelegate'> delegate: on click positive button </param>
	/// <param name='negativeDelegate'> delegate: on click negative button </param>
	public static void Show (string title, string message, PlatformDialog.Type buttonType, Action positiveDelegate, Action negativeDelegate=null )
	{
		switch( buttonType ) {
		case PlatformDialog.Type.SubmitOnly:
			Instance.positiveDelegate = positiveDelegate;
			Instance.negativeDelegate = positiveDelegate;
			break;
		default:
			Instance.positiveDelegate = positiveDelegate;
			Instance.negativeDelegate = negativeDelegate;
			break;
		}

		Instance.dialog.Show(title, message, buttonType);
	}
	
	/// <summary> Dismiss this dialog, removing it from the screen. </summary>
	public static void Dismiss() {
		Instance.dialog.Dismiss();
	}
	
	/// <summary> Set button Label </summary>
	/// <param name='positive'> positive button label </param>
	/// <param name='negative'> negative button label </param>
	public static void SetButtonLabel(string positive, string negative=null) {
		if( negative != null ) {
			Instance.dialog.SetButtonLabel( positive, negative );
		}
		else {
			Instance.dialog.SetButtonLabel( positive );
		}		
	}


	public void OnPositive( string data ) {
		if( positiveDelegate != null ) {
			this.positiveDelegate();
		}
		this.positiveDelegate = null;
	}

	public void OnNegative( string data ) {
		if( this.negativeDelegate != null ) {
			this.negativeDelegate();
		}
		this.negativeDelegate = null;
	}
}

public static class PlatformDialogTypeExtention {
   	public static int ToInt( this PlatformDialog.Type type ) {
   		return (int)type;
   	}
}

