using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameMatchShape : GameBase {
	public override GameTypes GameType {
		get {
			return GameTypes.MatchShape;
		}
	}

	private const int SHAPES_COUNT = 4;

	[SerializeField]
	private RectTransform droppedShapeRoot = null;
	[SerializeField]
	private RectTransform shapesForMatchRoot = null;

	private LinkedList<Shape> shapes = new LinkedList<Shape>();
	private Shape droppedShape = null;

	protected override void GameLoad() {
		
	}

	protected override void GameStart() {
		var typesSet = new HashSet<Shape.Type>();
		while (typesSet.Count < SHAPES_COUNT) {
			typesSet.Add(GenerateRandomEnum<Shape.Type>());
		}

		var colorsSet = new HashSet<Shape.Color>();
		while (colorsSet.Count < SHAPES_COUNT) {
			colorsSet.Add(GenerateRandomEnum<Shape.Color>());
		}

		var types = typesSet.ToArray();
		var colors = colorsSet.ToArray();

		for (int i = 0; i < SHAPES_COUNT; i++) {
			var shape = ShapesPool.Instance.GetShape(types[i], colors[i], Shape.VisualMode.EmptySlot, shapesForMatchRoot);
			shapes.AddLast(shape);
		}

		DropNewShape();
	}

	private void DropNewShape() {
		if (droppedShape != null) {
			ShapesPool.Instance.ReturnShape(droppedShape, false);
			droppedShape = null;
		}

		var emptyShapes = new List<Shape>(shapes.Count);
		foreach (var i in shapes) {
			if (i.CurrentVisualMode == Shape.VisualMode.EmptySlot) {
				emptyShapes.Add(i);
			}
		}

		if (emptyShapes.Count > 0) {
			droppedShape = GenerateSameShape(emptyShapes[Random.Range(0, emptyShapes.Count)], droppedShapeRoot);
			droppedShape.OnDrag += OnShapeDrag;
			droppedShape.CurrentVisualMode = Shape.VisualMode.ShapeWithShadow;
			droppedShape.CurrentFaceAnimation = Shape.FaceAnimation.Idle;
		} else {
			InvokeAfterDelay(0.5f, GameEnd);
		}
	}

	private void OnShapeDrag(Shape shape, Vector2 delta) {
		var worldPointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var selfPosition = shape.transform.position;
		worldPointer.z = selfPosition.z;
		selfPosition = Vector3.Lerp(selfPosition, worldPointer, 0.25f);
		shape.transform.position = selfPosition;
	}

	protected override void GameUpdate() {
		foreach (var shape in shapes) {
			if (shape.CurrentVisualMode == Shape.VisualMode.EmptySlot && shape.CurrentShape == droppedShape.CurrentShape) {
				var a = shape.transform.position;
				var b = droppedShape.transform.position;
				a.z = b.z = 0f;

				if (Vector3.Distance(a, b) < 0.8f) {
					shape.CurrentVisualMode = Shape.VisualMode.ShapeInSlot;
					shape.CurrentFaceAnimation = Shape.FaceAnimation.Ok;
					DropNewShape();
					break;
				}
			}
		}
	}

	protected override void GameEnd() {
		GameUnload();
		GameStart();
	}

	protected override void GameUnload() {
		ShapesPool.Instance.ReturnAllShapes();
		shapes.Clear();
		droppedShape = null;
	}
}
