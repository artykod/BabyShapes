using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMatchColor : GameBase {
	public override GameTypes GameType {
		get {
			return GameTypes.MatchColor;
		}
	}
	private const int SHAPES_COUNT = 8;
	private const int SHAPES_MATCHES_LIMIT = 2;

	[SerializeField]
	private RectTransform droppedShapeRoot = null;
	[SerializeField]
	private RectTransform shapesForMatchRoot = null;
	[SerializeField]
	private GridLayoutGroup shapesGrid = null;

	private Shape mainShape = null;
	private LinkedList<Shape> shapes = new LinkedList<Shape>();

	protected override void GameLoad() {
		base.GameLoad();
	}

	protected override void GameStart() {
		base.GameStart();

		mainShape = GenerateRandomShape(droppedShapeRoot);
		mainShape.CurrentVisualMode = Shape.VisualMode.ShapeWithShadow;
		mainShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		mainShape.OnClick += OnMainShapeClick;

		var matches = new HashSet<int>();
		while (matches.Count < SHAPES_MATCHES_LIMIT) {
			matches.Add(Random.Range(0, SHAPES_COUNT));
		}

		for (int i = 0; i < SHAPES_COUNT; i++) {
			var shape = GenerateRandomShape(shapesForMatchRoot);
			var makeSameShape = matches.Contains(i);

			shape.CurrentVisualMode = Shape.VisualMode.ShapeWithShadow;
			shape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
			shape.CurrentColor = makeSameShape ? mainShape.CurrentColor : GenerateRandomEnumExclude(mainShape.CurrentColor);
			shape.CurrentShape = makeSameShape ? mainShape.CurrentShape : GenerateRandomEnumExclude(mainShape.CurrentShape);

			shapes.AddLast(shape);

			shape.OnClick += OnShapeClick;
		}

		StartCoroutine(TurnOffGrid());

		InvokeStartHintIfNotShowed(() => OnMainShapeClick(mainShape));

		TryShowTutorialAfterDelay(0.5f);
	}

	protected override void GameUpdate() {
		base.GameUpdate();
	}

	protected override void GameUnload() {
		base.GameUnload();

		ShapesPool.Instance.ReturnAllShapes();
		shapes.Clear();
		mainShape = null;
	}

	protected override bool ShowTutorial() {
		if (base.ShowTutorial()) {
			var shapesForTutorial = new LinkedList<Shape>();
			foreach (var shape in shapes) {
				if (shape.CurrentColor == mainShape.CurrentColor) {
					shapesForTutorial.AddLast(shape);
				}
			}

			if (shapesForTutorial.Count > 0) {
				ShowTutorialForShape(shapesForTutorial.First);
			}

			return true;
		} else {
			return false;
		}
	}

	private void ShowTutorialForShape(LinkedListNode<Shape> shapeNode) {
		var tutorialSettings = new UITutorialHand.HandMovementSettings {
			showTime = 0.5f,
			startDelay = 0.15f,
			moveTime = 0.25f,
			start = shapeNode.Value.transform,
			endDelay = 0.15f,
			hideTime = 0.5f,
			repeatCount = int.MaxValue,
			repeatTimeout = 0.25f,
		};
		var nextShape = shapeNode.Next;
		var tutorialHand = UITutorialHand.Play(tutorialSettings);

		if (shapeNode.Next != null) {
			tutorialHand.OnClose += () => ShowTutorialForShape(nextShape);
		}
	}

	private void OnMainShapeClick(Shape shape) {
		//shape.ShowHint(UIDialogHintBaloon.Direction.RightBottom, "match_color");
	}

	private IEnumerator TurnOffGrid() {
		yield return null;
		shapesGrid.enabled = true;
		Canvas.ForceUpdateCanvases();
		yield return null;
		shapesGrid.enabled = false;
	}

	private void OnShapeClick(Shape shape) {
		if (shape.CurrentColor == mainShape.CurrentColor) {
			ShapesPool.Instance.ReturnShape(shape);
			shapes.Remove(shape);

			SoundController.Sound(SoundController.SOUND_CORRECT_2);

			bool isSomeoneExists = false;
			foreach (var i in shapes) {
				if (i.CurrentColor == mainShape.CurrentColor) {
					isSomeoneExists = true;
					break;
				}
			}

			if (!isSomeoneExists) {
				SoundController.Sound(SoundController.SOUND_WIN);
				mainShape.ShowHint(UIDialogHintBaloon.RandomBottom, "excellent", SoundController.RandomVoiceExcellent);
				mainShape.CurrentFaceAnimation = Shape.FaceAnimation.Ok;

				foreach (var i in shapes) {
					i.CurrentFaceAnimation = Shape.FaceAnimation.Happy;
				}

				GameEnd();
			}
		} else {
			shape.CurrentFaceAnimation = Shape.FaceAnimation.Mad;
			SoundController.Sound(SoundController.SOUND_INCORRECT_2);
			//mainShape.ShowHint(UIDialogHintBaloon.Direction.LeftBottom, "where_is", SoundController.VOICE_WHERE_IS);
		}
	}
}
