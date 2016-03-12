using UnityEngine;
using System.Collections.Generic;

public class GameFallShapes : GameBase {
	public override GameTypes GameType {
		get {
			return GameTypes.FallShapes;
		}
	}

	private class FallShape {
		public event System.Action<FallShape> OnSuccessClick = delegate { };

		private GameFallShapes game = null;
		private Shape shape = null;
		private Vector3 velocity = Vector3.zero;

		public FallShape(GameFallShapes game) {
			this.game = game;

			var gameTransform = game.transform as RectTransform;
			shape = Random.value > 0.5f ? game.GenerateRandomShape(gameTransform) : game.GenerateSameShape(game.mainShape, gameTransform);
			velocity = new Vector3(Random.Range(-3f, 3f), -2.5f);

			shape.transform.position = new Vector3(Random.Range(game.worldFieldRect.xMin, game.worldFieldRect.xMax), game.worldFieldRect.yMax + 2f);

			shape.OnClick += OnShapeClick;

			game.shapes.AddLast(this);

			shape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		}

		public bool UpdateFrame() {
			Vector3 pos = shape.transform.position;
			pos += velocity * Time.deltaTime;
			if (pos.x < game.worldFieldRect.xMin + 1f) {
				pos.x = game.worldFieldRect.xMin + 1f;
				velocity.x = -velocity.x;
			}
			if (pos.x > game.worldFieldRect.xMax - 1f) {
				pos.x = game.worldFieldRect.xMax - 1f;
				velocity.x = -velocity.x;
			}
			shape.transform.position = pos;

			return pos.y < game.worldFieldRect.yMin - 2f;
		}

		public void Destroy() {
			shape.OnClick -= OnShapeClick;
			ShapesPool.Instance.ReturnShape(shape);
			shape = null;
		}

		private void OnShapeClick(Shape shape) {
			if (shape != null && shape == this.shape) {
				if (game.mainShape.CurrentColor == shape.CurrentColor && game.mainShape.CurrentShape == shape.CurrentShape) {
					OnSuccessClick(this);
					SoundController.Sound(SoundController.SOUND_CORRECT);
				} else {
					shape.CurrentFaceAnimation = Shape.FaceAnimation.Mad;
					SoundController.Sound(SoundController.SOUND_INCORRECT);
				}
			}
		}
	}

	private const float DROP_SHAPE_PERIOD = 2f;

	[SerializeField]
	private RectTransform mainShapeRoot = null;
	[SerializeField]
	private RectTransform fieldRect = null;

	private Shape mainShape = null;
	private LinkedList<FallShape> shapes = new LinkedList<FallShape>();
	private float newShapeDelay = DROP_SHAPE_PERIOD;
	private int successClicks = 0;
	private Rect worldFieldRect;
	private int hackComputeWorldRectSizeCounter = 4;

	protected override void GameLoad() {
		RefreshFieldWorldRect();
	}

	protected void RefreshFieldWorldRect() {
		var corners = new Vector3[4];
		fieldRect.GetWorldCorners(corners);
		var lt = corners[0];
		var rb = corners[2];
		var width = Mathf.Abs(rb.x - lt.x);
		var height = Mathf.Abs(rb.y - lt.y);
		worldFieldRect = new Rect(fieldRect.position.x - width / 2f, fieldRect.position.y - height / 2f, width, height);
	}

	protected override void GameStart() {
		RefreshFieldWorldRect();

		mainShape = GenerateRandomShape(mainShapeRoot);
		mainShape.CurrentVisualMode = Shape.VisualMode.ShapeInSlot;
		mainShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
	}

	protected override void GameUpdate() {
		if (hackComputeWorldRectSizeCounter > 0) {
			RefreshFieldWorldRect();
			hackComputeWorldRectSizeCounter--;
		}

		newShapeDelay -= Time.deltaTime;
		if (newShapeDelay < 0f) {
			newShapeDelay = DROP_SHAPE_PERIOD;

			if (mainShape != null) {
				new FallShape(this).OnSuccessClick += OnSuccessShapeClick;
			}
		}

		var toDestroy = new LinkedList<FallShape>();

		foreach (var shape in shapes) {
			if (shape.UpdateFrame()) {
				shape.Destroy();
				toDestroy.AddLast(shape);
			}
		}

		foreach (var shape in toDestroy) {
			shapes.Remove(shape);
		}
	}

	private void OnSuccessShapeClick(FallShape shape) {
		successClicks++;

		shape.Destroy();
		shapes.Remove(shape);

		newShapeDelay /= 2f;

		if (successClicks > 5) {
			SoundController.Sound(SoundController.SOUND_WIN_KIDS);
			SoundController.Sound(SoundController.SOUND_WIN_HANDS);
			InvokeAfterDelay(0.5f, GameEnd);
		}
	}

	protected override void GameEnd() {
		GameUnload();
		GameStart();
	}

	protected override void GameUnload() {
		successClicks = 0;
		newShapeDelay = 0f;

		shapes.Clear();
		mainShape = null;

		ShapesPool.Instance.ReturnAllShapes();
	}
}
