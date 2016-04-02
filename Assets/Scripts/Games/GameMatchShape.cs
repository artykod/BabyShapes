using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameMatchShape : GameBase {
	public override GameTypes GameType {
		get {
			return GameTypes.MatchShape;
		}
	}

	private int EmptyShapesCount {
		get;
		set;
	}

	[SerializeField]
	private RectTransform droppedShapeRoot = null;
	[SerializeField]
	private RectTransform shapesForMatchRoot = null;

	private LinkedList<Shape> shapes = new LinkedList<Shape>();
	private Shape droppedShape = null;
	private Shape lastMatchedShape = null;
	private Shape.Color[] shapesColorsSequence = new Shape.Color[] {
		Shape.Color.Red,
		Shape.Color.Orange,
		Shape.Color.Yellow,
		Shape.Color.Green,
		Shape.Color.LightBlue,
		Shape.Color.DarkBlue,
		Shape.Color.Violet
	};
	private int shapesCurrentShapesColorIndex = -1;

	protected override void GameLoad() {
		base.GameLoad();

		shapesCurrentShapesColorIndex = -1;

		// [0..2] - 3 shapes
		// [3..5] - 4 shapes
		// [6..8] - 3 shapes
		// etc.
		EmptyShapesCount = (((GameLoadsCountTotal - 1) / 3) % 2 == 0) ? 3 : 4;
	}

	protected override void GameStart() {
		base.GameStart();

		shapesCurrentShapesColorIndex = (shapesCurrentShapesColorIndex + 1) % shapesColorsSequence.Length;

		var typesSet = new HashSet<Shape.Type>();
		while (typesSet.Count < EmptyShapesCount) {
			typesSet.Add(GenerateRandomEnum<Shape.Type>());
		}
		var types = typesSet.ToArray();

		var colorsSet = new HashSet<Shape.Color>();
		while (colorsSet.Count < EmptyShapesCount) {
			colorsSet.Add(GenerateRandomEnum<Shape.Color>());
		}
		var colors = colorsSet.ToArray();

		for (int i = 0; i < EmptyShapesCount; i++) {
			var shapesColor = GamesDoneCount >= shapesColorsSequence.Length ? colors[i] : shapesColorsSequence[shapesCurrentShapesColorIndex];
			var shape = ShapesPool.Instance.GetShape(types[i], shapesColor, Shape.VisualMode.EmptySlot, shapesForMatchRoot);
			shape.OnClick += OnEmptyShapeClick;
			shapes.AddLast(shape);
		}

		DropNewShape();

		InvokeStartHintIfNotShowed(() => OnDroppedShapeClick(droppedShape));
	}

	protected override void GameUpdate() {
		base.GameUpdate();

		if (droppedShape != null) {
			foreach (var shape in shapes) {
				var a = shape.transform.position;
				var b = droppedShape.transform.position;
				a.z = b.z = 0f;

				if (Vector3.Distance(a, b) < 0.8f && CheckMatchWithShape(shape)) {
					break;
				}
			}
		}
	}

	protected override void GameUnload() {
		base.GameUnload();

		ShapesPool.Instance.ReturnAllShapes();
		shapes.Clear();
		droppedShape = null;
		lastMatchedShape = null;
	}

	protected override void OnGameWin() {
		base.OnGameWin();

		if (((GameWinsCountTotal - 1) % 3) == 0) {
			RateUsDialog.TryShowRateUsDialog();
		}
	}

	protected override bool ShowTutorial() {
		if (base.ShowTutorial() && droppedShape != null) {
			var targetShape = null as Shape;
			foreach (var shape in shapes) {
				if (shape.CurrentShape == droppedShape.CurrentShape) {
					targetShape = shape;
					break;
				}
			}

			if (targetShape != null) {
				UITutorialHand.Play(new UITutorialHand.HandMovementSettings {
					showTime = 0.5f,
					startDelay = 0.15f,
					moveTime = 0.7f,
					start = droppedShape.transform,
					end = targetShape.transform,
					endDelay = 0.15f,
					hideTime = 0.5f,
					repeatCount = int.MaxValue,
					repeatTimeout = 0.25f,
				});
			}

			return true;
		} else {
			return false;
		}
	}

	private void DropNewShape() {
		var isFirstShape = true;

		if (droppedShape != null) {
			ShapesPool.Instance.ReturnShape(droppedShape, false);
			droppedShape = null;
			isFirstShape = false;
		}

		var emptyShapes = new List<Shape>(shapes.Count);
		foreach (var i in shapes) {
			if (i.CurrentVisualMode == Shape.VisualMode.EmptySlot) {
				emptyShapes.Add(i);
			}
		}

		if (emptyShapes.Count > 0) {
			droppedShape = GenerateSameShape(emptyShapes[Random.Range(0, emptyShapes.Count)], droppedShapeRoot);
			droppedShape.CurrentVisualMode = Shape.VisualMode.ShapeWithShadow;
			droppedShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
			droppedShape.OnDrag += OnShapeDrag;
			droppedShape.OnClick += OnDroppedShapeClick;
			if (isFirstShape) {
				droppedShape.SpeakWhereIsShape();
			} else {
				droppedShape.SpeakShapeType();
			}
		} else {
			SoundController.Sound(SoundController.SOUND_WIN);

			foreach (var i in shapes) {
				if (lastMatchedShape != i) {
					i.CurrentFaceAnimation = Shape.FaceAnimation.Happy;
				}
			}

			if (lastMatchedShape != null) {
				lastMatchedShape.ShowHint(UIDialogHintBaloon.Direction.LeftBottom, "excellent", SoundController.RandomVoiceExcellent);
			}

			GameEnd();
		}

		TryShowTutorialAfterDelay(0.5f);
	}

	private void OnDroppedShapeClick(Shape shape) {
		//shape.ShowHint(UIDialogHintBaloon.Direction.RightTop, "match_shape");
	}

	private void OnEmptyShapeClick(Shape shape) {
		if (!CheckMatchWithShape(shape) && droppedShape != null) {
			droppedShape.CurrentFaceAnimation = Shape.FaceAnimation.Mad;
			SoundController.Sound(SoundController.SOUND_INCORRECT);
			InvokeAfterDelay(0.15f, () => droppedShape.SpeakWhereIsShape());
		}
	}

	private void OnShapeDrag(Shape shape, Vector2 delta) {
		var worldPointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var selfPosition = shape.transform.position;
		worldPointer.z = selfPosition.z;
		selfPosition = Vector3.Lerp(selfPosition, worldPointer, 0.4f);
		shape.transform.position = selfPosition;
	}

	private bool CheckMatchWithShape(Shape shape) {
		if (shape.CurrentVisualMode == Shape.VisualMode.EmptySlot && shape.CurrentShape == droppedShape.CurrentShape) {
			shape.CurrentVisualMode = Shape.VisualMode.ShapeInSlot;
			shape.CurrentFaceAnimation = Shape.FaceAnimation.Ok;
			lastMatchedShape = shape;
			DropNewShape();
			SoundController.Sound(SoundController.SOUND_CORRECT);

			return true;
		}

		return false;
	}
}
