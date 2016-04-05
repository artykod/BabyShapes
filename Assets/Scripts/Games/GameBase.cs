using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameBase : MonoBehaviour, GameController.IGame {
	private const string PREFS_GAME_LOADS_COUNT = "gmldscnt";
	private const string PREFS_GAME_WINS_COUNT = "gmwnscnt";
	
	private bool startHintShowed = false;
	private int gamesDoneCount = 0;
	private int tutorialCounter = 0;

	public abstract GameTypes GameType {
		get;
	}

	protected string PrefsGameId {
		get {
			return GameType.ToString();
		}
	}

	protected string PrefsKeyGameLoadsCount {
		get {
			return PREFS_GAME_LOADS_COUNT + PrefsGameId;
		}
	}

	protected string PrefsKeyGameWinsCount {
		get {
			return PREFS_GAME_WINS_COUNT + PrefsGameId;
		}
	}

	protected int GameLoadsCountTotal {
		get {
			return PlayerPrefs.GetInt(PrefsKeyGameLoadsCount, 0);
		}
		set {
			PlayerPrefs.SetInt(PrefsKeyGameLoadsCount, value);
			PlayerPrefs.Save();
		}
	}

	protected int GameWinsCountTotal {
		get {
			return PlayerPrefs.GetInt(PrefsKeyGameWinsCount, 0);
		}
		set {
			PlayerPrefs.SetInt(PrefsKeyGameWinsCount, value);
			PlayerPrefs.Save();
		}
	}

	protected virtual int GamesCountForApplause {
		get {
			return 3;
		}
	}

	protected virtual int GamesCountForBaloons {
		get {
			return (GameWinsCountTotal % 2) == 0 ? 7 : 10;
		}
	}

	protected int GamesDoneCount {
		get {
			return gamesDoneCount;
		}
	}

	protected virtual int TutorialShowMaxCount {
		get {
			return 2;
		}
	}

	protected bool NeedShowTutorial {
		get {
			return (GameLoadsCountTotal - 1) % 3 == 0 && tutorialCounter < TutorialShowMaxCount;
		}
	}

	protected virtual bool CanShowAdBanner {
		get {
			return true;
		}
	}

	private void Awake() {
		var canvas = GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.worldCamera = Camera.main;
		Canvas.ForceUpdateCanvases();

		ShapesPool.Instance.Initialize();
	}

	private void Update() {
		GameUpdate();
	}

	protected T GenerateRandomEnum<T>() where T : struct {
		var allValues = (T[])System.Enum.GetValues(typeof(T));
		var randomIndex = Random.Range(1, allValues.Length - 1);

		return allValues[randomIndex];
	}

	protected T GenerateRandomEnumExclude<T>(T excluded) where T : struct {
		T result = excluded;
		int limit = 10;

		while (result.Equals(excluded) && limit >= 0) {
			result = GenerateRandomEnum<T>();
			limit--;
		}

		if (limit < 0) {
			var allValues = (T[])System.Enum.GetValues(typeof(T));
			for (int i = 1; i < allValues.Length - 1; i++) {
				var c = allValues[i];
				if (!c.Equals(excluded)) {
					result = c;
					break;
				}
			}
		}

		return result;
	}

	protected Shape GenerateRandomShape(RectTransform parent) {
		return ShapesPool.Instance.GetShape(
			GenerateRandomEnum<Shape.Type>(),
			GenerateRandomEnum<Shape.Color>(),
			Shape.VisualMode.ShapeWithShadow,
			parent
		);
	}

	protected Shape GenerateRandomShapeExcludeShape(Shape excludedShape, RectTransform parent) {
		return ShapesPool.Instance.GetShape(
			GenerateRandomEnumExclude(excludedShape.CurrentShape),
			GenerateRandomEnumExclude(excludedShape.CurrentColor),
			Shape.VisualMode.ShapeWithShadow,
			parent
		);
	}

	protected Shape GenerateSameShape(Shape mainShape, RectTransform parent) {
		return ShapesPool.Instance.GetShape(
			mainShape.CurrentShape,
			mainShape.CurrentColor,
			Shape.VisualMode.ShapeWithShadow,
			parent
		);
	}

	protected void InvokeAfterDelay(float delay, System.Action action) {
		StartCoroutine(InvokeAfterDelayRoutine(delay, action));
	}

	protected void InvokeStartHintIfNotShowed(System.Action action) {
		if (!startHintShowed) {
			action();
			startHintShowed = true;
		}
	}

	protected virtual void GameLoad() {
		gamesDoneCount = 0;
		GameLoadsCountTotal++;
		tutorialCounter = 0;
		UITutorialHand.IsTutorialSkipped = false;
	}
	protected virtual void GameStart() {
		// for override
	}
	protected virtual void GameUpdate() {
		// for override
	}
	protected virtual void GameUnload() {
		UIDialogHintBaloon.ForceHideAll();
	}
	protected virtual void OnGameWin() {
		// for override
	}
	protected virtual bool ShowTutorial() {
		var result = NeedShowTutorial && !UITutorialHand.IsTutorialSkipped;
		tutorialCounter++;
		return result;
	}

	protected void TryShowTutorialAfterDelay(float delay) {
		UITutorialHand.IsTutorialSkipped = false;
		InvokeAfterDelay(delay, () => ShowTutorial());
	}

	protected void GameEnd() {
		gamesDoneCount++;

		if (gamesDoneCount >= GamesCountForBaloons) {
			EffectWin.PlayEffect();
			GameWinsCountTotal++;
			InvokeAfterDelay(1.5f, () => SoundController.Voice(SoundController.VOICE_WELL_DONE));
			InvokeAfterDelay(2.25f, HandleGameWin);
		} else {
			if (GamesCountForApplause > 0) {
				var needApplause = (gamesDoneCount % GamesCountForApplause) == 0;
				var isLastApplauseGame = (gamesDoneCount / GamesCountForApplause) == (GamesCountForBaloons / GamesCountForApplause);

				if (needApplause && !isLastApplauseGame) {
					EffectWin.PlaySoundOnly();
				}
			}

			InvokeAfterDelay(1f, RestartCurrentGame);
		}
	}

	private void RestartCurrentGame() {
		GameUnload();
		GameStart();
	}

	private void HandleGameWin() {
		AnalyticsTracker.MiniGameWin(GameType);

		GameUnload();
		OnGameWin();
		GameController.Instance.StartNextGame(GameController.GamesNavigation.Next);
	}

	private IEnumerator InvokeAfterDelayRoutine(float delay, System.Action action) {
		if (action != null) {
			yield return new WaitForSeconds(delay);
			action();
		}
	}

	void GameController.IGame.OnStart() {
		AnalyticsTracker.MiniGameStart(GameType);

		UITutorialHand.DestroyCurrent();
		startHintShowed = false;
		GameLoad();
		GameStart();

		AdsController.Instance.IsAdBannerVisible = CanShowAdBanner;

		SoundController.PlayNextMusic();
	}

	void GameController.IGame.OnStop() {
		GameUnload();
	}
}
