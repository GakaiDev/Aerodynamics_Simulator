using System;

namespace AerodynamicsSimulator.Math
{
	public class Vector2D
	{
		public double x, y;

		public Vector2D(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public double DotProduct(Vector2D v)
		{
			return (this.x * v.x) + (this.y * v.y);
		}

		public double Magnitude()
		{
			return System.Math.Sqrt((this.x * this.x) + (this.y * this.y));
		}

		public Vector2D Normalize()
		{
			double mag = Magnitude();
			if (mag == 0) throw new ArithmeticException("Vetor nulo.");
			return new Vector2D(this.x / mag, this.y / mag);
		}
	}
}
