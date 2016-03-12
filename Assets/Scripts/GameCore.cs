using UnityEngine;

public class GameCore : MonoBehaviour {
	private void Awake() {
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
	}
}
