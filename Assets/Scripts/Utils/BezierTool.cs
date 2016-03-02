using UnityEngine;

public static class BezierTool
{
	public static Vector3 CalculateBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		Vector3 a1 = Vector3.Lerp(p1, p2, t);
		Vector3 a2 = Vector3.Lerp(p2, p3, t);
		Vector3 a3 = Vector3.Lerp(p3, p4, t);
		Vector3 b1 = Vector3.Lerp(a1, a2, t);
		Vector3 b2 = Vector3.Lerp(a2, a3, t);
		
		return Vector3.Lerp(b1, b2, t);
	}
}
