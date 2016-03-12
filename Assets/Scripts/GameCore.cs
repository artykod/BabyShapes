using UnityEngine;

public class GameCore : MonoBehaviour {
	private void Awake() {
		Application.targetFrameRate = 60;
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
		SoundController.Music(SoundController.MUSIC_STANDARD);
	}
}
