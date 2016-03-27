using UnityEngine;
using System.Collections;

public abstract class GameBase : MonoBehaviour, GameController.IGame {
	public abstract GameTypes GameType {
		get;
	}

	private bool startHintShowed = false;

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

	protected void GameEnd() {
		GameUnload();
		UIDialogHintBaloon.ForceHideAll();
		GameStart();
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

	private IEnumerator InvokeAfterDelayRoutine(float delay, System.Action action) {
		if (action != null) {
			yield return new WaitForSeconds(delay);
			action();
		}
	}

	protected abstract void GameLoad();
	protected abstract void GameStart();
	protected abstract void GameUpdate();
	protected abstract void GameUnload();

	void GameController.IGame.OnStart() {
		startHintShowed = false;
		GameLoad();
		GameStart();
	}

	void GameController.IGame.OnStop() {
		GameUnload();
		UIDialogHintBaloon.ForceHideAll();
	}
}
