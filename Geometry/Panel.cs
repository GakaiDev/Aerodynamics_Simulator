using System;
using AerodynamicsSimulator.Math;

namespace AerodynamicsSimulator.Geometry
{
	public class Panel
	{
		public Vector2D pointA, pointB, controlPoint, normal, tangent;
		public double length, angle;

		public Panel(Vector2D pointA, Vector2D pointB)
		{
			this.pointA = pointA;
			this.pointB = pointB;
			CalculateGeometry();
		}

		private void CalculateGeometry()
		{
			this.controlPoint = new Vector2D((pointA.x + pointB.x) / 2.0, (pointA.y + pointB.y) / 2.0);
			
			double dx = pointB.x - pointA.x;
			double dy = pointB.y - pointA.y;

			this.length = System.Math.Sqrt((dx * dx) + (dy * dy));
			this.angle = System.Math.Atan2(dy, dx);

			this.normal = new Vector2D(-dy, dx).Normalize();
			this.tangent = new Vector2D(dx, dy).Normalize();
		}
	}
}
