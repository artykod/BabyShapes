using UnityEngine;
using System.Collections.Generic;

public enum GameTypes {
	MatchColor,
	MatchShape,
	FallShapes,
}

public class GameController : AbstractSingletonBehaviour<GameController, GameController> {
	public interface IGame {
		void OnStart();
		void OnStop();
	}

	public enum GamesNavigation {
		Next,
		Previous,
	}

	private GameTypes[] gamesOrder = new GameTypes[] {
		GameTypes.MatchShape,
		GameTypes.MatchColor,
		GameTypes.FallShapes,
	};

	private Dictionary<GameTypes, GameBase> allGames = new Dictionary<GameTypes, GameBase>();
	private GameBase currentGame = null;

	public void StartNextGame(GamesNavigation navigation) {
		var nextGame = GameTypes.MatchShape;
		if (currentGame != null) {
			var index = 0;
			for (int i = 0; i < gamesOrder.Length; i++) {
				if (currentGame.GameType == gamesOrder[i]) {
					index = i;
					break;
				}
			}
			nextGame = gamesOrder[(index + (navigation == GamesNavigation.Next ? 1 : -1)) % gamesOrder.Length];
		}
		StartGameOfType(nextGame);
	}

	private void StartGameOfType(GameTypes gameType) {
		currentGame = null;

		foreach (var i in allGames) {
			(i.Value as IGame).OnStop();
			i.Value.gameObject.SetActive(false);
		}

		if (allGames.TryGetValue(gameType, out currentGame)) {
			currentGame.gameObject.SetActive(true);
			(currentGame as IGame).OnStart();
		} else {
			Debug.LogError("Cannot start game of type " + gameType);
		}
	}

	private void Awake() {
		var allGamesPrefabs = Resources.LoadAll<GameBase>("Games");
		foreach (var i in allGamesPrefabs) {
			if (!allGames.ContainsKey(i.GameType)) {
				var game = Instantiate(i);
				allGames[game.GameType] = game;
			}
		}
	}
}
