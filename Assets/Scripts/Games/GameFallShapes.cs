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

		private const float SIDE_LEFT = -8.5f;
		private const float SIDE_RIGHT = 4f;
		private const float SIDE_TOP = 8.5f;
		private const float SIDE_BOTTOM = -8.5f;

		private GameFallShapes game = null;
		private Shape shape = null;
		private Vector3 velocity = Vector3.zero;

		public FallShape(GameFallShapes game) {
			this.game = game;

			var gameTransform = game.transform as RectTransform;
			shape = Random.value > 0.5f ? game.GenerateRandomShape(gameTransform) : game.GenerateSameShape(game.mainShape, gameTransform);
			velocity = new Vector3(Random.Range(-3f, 3f), -2.5f);

			shape.transform.position = new Vector3(0f, SIDE_TOP);

			shape.OnClick += OnShapeClick;

			game.shapes.AddLast(this);

			shape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		}

		public bool UpdateFrame() {
			Vector3 pos = shape.transform.position;
			pos += velocity * Time.deltaTime;
			if (pos.x < SIDE_LEFT) {
				pos.x = SIDE_LEFT;
				velocity.x = -velocity.x;
			}
			if (pos.x > SIDE_RIGHT) {
				pos.x = SIDE_RIGHT;
				velocity.x = -velocity.x;
			}
			shape.transform.position = pos;

			return pos.y < SIDE_BOTTOM;
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

	private Shape mainShape = null;
	private LinkedList<FallShape> shapes = new LinkedList<FallShape>();
	private float newShapeDelay = 0f;
	private int successClicks = 0;

	protected override void GameLoad() {
		
	}

	protected override void GameStart() {
		mainShape = GenerateRandomShape(mainShapeRoot);
		mainShape.CurrentVisualMode = Shape.VisualMode.ShapeInSlot;
		mainShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
	}

	protected override void GameUpdate() {
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
