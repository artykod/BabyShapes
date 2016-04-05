using UnityEngine;
using System.Collections.Generic;

public class GameFallShapes : GameBase {
	public override GameTypes GameType {
		get {
			return GameTypes.FallShapes;
		}
	}

	private class FallShape {
		public event System.Action<FallShape, bool> OnSuccessClick = delegate { };

		private GameFallShapes game = null;
		private Shape shape = null;
		private Vector3 velocity = Vector3.zero;
		private bool isSameShape = false;

		public FallShape(GameFallShapes game) {
			this.game = game;

			var gameTransform = game.fieldRect;
			isSameShape = Random.value > 0.5f;
			shape = isSameShape ? game.GenerateSameShape(game.mainShape, gameTransform) : game.GenerateRandomShapeExcludeShape(game.mainShape, gameTransform);
			velocity = new Vector3(Random.Range(-3f, 3f), -2.5f);

			shape.transform.position = new Vector3(Random.Range(game.worldFieldRect.xMin, game.worldFieldRect.xMax), game.worldFieldRect.yMax + 2f);

			shape.OnClick += OnShapeClick;

			game.shapes.AddLast(this);

			shape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		}

		public bool UpdateFrame() {
			Vector3 pos = shape.transform.position;
			pos += velocity * Time.deltaTime;
			if (pos.x < game.worldFieldRect.xMin + 1.1f) {
				pos.x = game.worldFieldRect.xMin + 1.1f;
				velocity.x = -velocity.x;
			}
			if (pos.x > game.worldFieldRect.xMax - 1.1f) {
				pos.x = game.worldFieldRect.xMax - 1.1f;
				velocity.x = -velocity.x;
			}
			shape.transform.position = pos;

			if (isSameShape && game.lastSameShape == null && pos.y < game.worldFieldRect.yMax - 2f) {
				game.lastSameShape = shape;
				game.TryShowTutorialAfterDelay(0.1f);
			}

			return pos.y < game.worldFieldRect.yMin - 2f;
		}

		public void Destroy() {
			if (game.lastSameShape == shape) {
				game.lastSameShape = null;
			}
			shape.OnClick -= OnShapeClick;
			ShapesPool.Instance.ReturnShape(shape);
			shape = null;
		}

		private void OnShapeClick(Shape shape) {
			if (shape != null && shape == this.shape) {
				if (game.mainShape.CurrentColor == shape.CurrentColor && game.mainShape.CurrentShape == shape.CurrentShape) {
					OnSuccessClick(this, true);
					SoundController.Sound(SoundController.SOUND_CORRECT);
				} else {
					OnSuccessClick(this, false);
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

	private Rect worldFieldRect;
	private Shape mainShape = null;
	private LinkedList<FallShape> shapes = new LinkedList<FallShape>();
	private float newShapeDelay = DROP_SHAPE_PERIOD;
	private int successClicks = 0;
	private int hackComputeWorldRectSizeCounter = 4;
	private int wrongBaloonCounter = 0;
	private Shape lastSameShape = null;
	private bool isRoundDone = false;

	protected override int TutorialShowMaxCount {
		get {
			return 4;
		}
	}

	protected override void GameLoad() {
		base.GameLoad();

		RefreshFieldWorldRect();
		wrongBaloonCounter = 0;
	}

	protected override void GameStart() {
		base.GameStart();

		isRoundDone = false;

		RefreshFieldWorldRect();

		mainShape = GenerateRandomShape(mainShapeRoot);
		mainShape.CurrentVisualMode = Shape.VisualMode.ShapeInSlot;
		mainShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		mainShape.OnClick += OnMainShapeClick;

		InvokeStartHintIfNotShowed(() => OnMainShapeClick(mainShape));

		mainShape.SpeakWhereIsShape();
	}

	protected override void GameUpdate() {
		base.GameUpdate();

		if (hackComputeWorldRectSizeCounter > 0) {
			RefreshFieldWorldRect();
			hackComputeWorldRectSizeCounter--;
		}

		newShapeDelay -= Time.deltaTime;
		if (newShapeDelay < 0f && !isRoundDone) {
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

	protected override void GameUnload() {
		base.GameUnload();

		successClicks = 0;
		newShapeDelay = 0f;

		shapes.Clear();
		mainShape = null;

		ShapesPool.Instance.ReturnAllShapes();
	}

	protected override bool ShowTutorial() {
		if (lastSameShape == null) {
			return false;
		}

		if (base.ShowTutorial()) {
			var tutorialSettings = new UITutorialHand.HandMovementSettings {
				showTime = 0.5f,
				startDelay = 0.15f,
				moveTime = 0.25f,
				start = lastSameShape.transform,
				endDelay = 0.15f,
				hideTime = 0.5f,
				repeatCount = int.MaxValue,
				repeatTimeout = 0.25f,
			};

			UITutorialHand.Play(tutorialSettings).OnClose += () => lastSameShape = null;

			return true;
		} else {
			return false;
		}
	}

	private void RefreshFieldWorldRect() {
		var corners = new Vector3[4];

		fieldRect.GetWorldCorners(corners);

		var lt = corners[0];
		var rb = corners[2];
		var width = Mathf.Abs(rb.x - lt.x);
		var height = Mathf.Abs(rb.y - lt.y);

		worldFieldRect = new Rect(fieldRect.position.x - width / 2f, fieldRect.position.y - height / 2f, width, height);
	}

	private void OnMainShapeClick(Shape shape) {
		//shape.ShowHint(UIDialogHintBaloon.Direction.LeftTop, "match_shape_color");
	}

	private void OnSuccessShapeClick(FallShape shape, bool isSuccess) {
		if (isRoundDone) {
			return;
		}

		if (isSuccess) {
			successClicks++;

			shape.Destroy();
			shapes.Remove(shape);

			newShapeDelay /= 2f;

			if (successClicks >= 3) {
				isRoundDone = true;

				mainShape.ShowHint(UIDialogHintBaloon.Direction.LeftBottom, "excellent", SoundController.RandomVoiceExcellent);
				InvokeAfterDelay(0.25f, GameEnd);
			}
		} else {
			if (wrongBaloonCounter++ % 3 == 0) {
				//mainShape.ShowHint(UIDialogHintBaloon.Direction.LeftBottom, "where_is", SoundController.VOICE_WHERE_IS);
			}
		}
	}
}
