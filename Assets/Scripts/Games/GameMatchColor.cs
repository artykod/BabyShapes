﻿using UnityEngine;
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
		
	}

	protected override void GameStart() {
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
			shape.CurrentVisualMode = Shape.VisualMode.ShapeWithShadow;
			shape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
			shape.CurrentColor = matches.Contains(i) ? mainShape.CurrentColor : GenerateRandomEnumExclude(mainShape.CurrentColor);

			shapes.AddLast(shape);

			shape.OnClick += OnShapeClick;
		}

		StartCoroutine(TurnOffGrid());

		InvokeStartHintIfNotShowed(() => OnMainShapeClick(mainShape));
	}

	private void OnMainShapeClick(Shape shape) {
		shape.ShowHint(UIDialogHintBaloon.Direction.RightBottom, "match_color");
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

				InvokeAfterDelay(1f, GameEnd);
			}
		} else {
			shape.CurrentFaceAnimation = Shape.FaceAnimation.Mad;
			SoundController.Sound(SoundController.SOUND_INCORRECT_2);
			mainShape.ShowHint(UIDialogHintBaloon.Direction.LeftBottom, "where_is", SoundController.VOICE_WHERE_IS);
		}
	}

	protected override void GameUpdate() {
		//
	}

	protected override void GameUnload() {
		ShapesPool.Instance.ReturnAllShapes();
		shapes.Clear();
		mainShape = null;
	}
}
