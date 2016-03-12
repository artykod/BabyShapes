using UnityEngine;
using System.Collections.Generic;

public class ShapesPool : AbstractSingleton<ShapesPool, ShapesPool> {
	public interface IPooledShape {
		void OnGet(Transform newParent);
		void OnReturn(Transform poolParent, bool animated, float animationDelay);
	}

	private LinkedList<Shape> pool = new LinkedList<Shape>();
	private LinkedList<IPooledShape> allShapes = new LinkedList<IPooledShape>();
	private Transform poolRoot = null;

	public void Initialize() {
		if (pool.Count < 1) {
			ExpandPool(25);
		}
	}

	public Shape GetShape(Shape.Type shapeType, Shape.Color shapeColor, Shape.VisualMode visualMode, Transform newParent = null) {
		if (pool.Count < 1) {
			ExpandPool(1);
		}

		var shape = pool.First.Value;
		pool.RemoveFirst();

		if (shape != null) {
			(shape as IPooledShape).OnGet(newParent);

			shape.CurrentShape = shapeType;
			shape.CurrentColor = shapeColor;
			shape.CurrentVisualMode = visualMode;
		}

		return shape;
	}

	public void ReturnShape(Shape shape, bool animated = true, float animationDelay = 0.25f) {
		if (shape == null) {
			return;
		}

		if (!pool.Contains(shape)) {
			pool.AddLast(shape);
			(shape as IPooledShape).OnReturn(poolRoot, animated, animationDelay);
		}
	}

	public void ReturnAllShapes() {
		foreach (var i in allShapes) {
			ReturnShape(i as Shape, false, 0f);
		}
	}

	private void ExpandPool(int count) {
		if (poolRoot == null) {
			poolRoot = new GameObject("ShapesPool").transform;
		}

		var shapePrefab = Resources.Load<Shape>("Shape");

		for (int i = 0; i < count; i++) {
			var shape = Object.Instantiate(shapePrefab);
			pool.AddLast(shape);
			allShapes.AddLast(shape);
			(shape as IPooledShape).OnReturn(poolRoot, false, 0f);
		}
	}
}
