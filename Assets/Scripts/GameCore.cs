using UnityEngine;
using System.Collections;

public class GameCore : MonoBehaviour {
	private void Awake() {
		Application.targetFrameRate = 60;
		SoundController.Music(SoundController.MUSIC_STANDARD);
	}

	private IEnumerator Start() {
		var dialog = UIDialogHello.ShowSingle();
		SoundController.Voice(SoundController.VOICE_HELLO);
		yield return new WaitForSeconds(1.25f);
		dialog.Close();
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
	}
}
