using System;
using AerodynamicsSimulator.Math;

namespace AerodynamicsSimulator.Geometry
{
	public class PanelMethod
	{
		public Panel[] panels;
		private Vector2D freeStreamVelocity;
		public double[] lambdas, tangentialVelocities, pressureCoefficients;

		public PanelMethod(Panel[] panels, Vector2D freeStreamVelocity)
		{
			this.panels = panels;
			this.freeStreamVelocity = freeStreamVelocity;
		}

		public void Solve()
		{
			int n = panels.Length;
			double[][] A = new double[n][];
			for (int i = 0; i < n; i++) A[i] = new double[n];
			double[] b = new double[n];

			for (int i = 0; i < n; i++)
			{
				b[i] = -freeStreamVelocity.DotProduct(panels[i].normal);
				for (int j = 0; j < n; j++)
				{
					if (i == j) A[i][j] = 0.5;
					else A[i][j] = CalculateInfluence(panels[i], panels[j], false);
				}
			}

			this.lambdas = MatrixSolver.Solve(A, b);
			CalculatePressures();
		}

		private void CalculatePressures()
		{
			int n = panels.Length;
			tangentialVelocities = new double[n];
			pressureCoefficients = new double[n];
			double vInf = freeStreamVelocity.Magnitude();

			for (int i = 0; i < n; i++)
			{
				double vt = freeStreamVelocity.DotProduct(panels[i].tangent);
				for (int j = 0; j < n; j++)
				{
					if (i != j) vt += lambdas[j] * CalculateInfluence(panels[i], panels[j], true);
				}
				tangentialVelocities[i] = vt;
				pressureCoefficients[i] = 1.0 - System.Math.Pow(vt / vInf, 2);
			}
		}

		private double CalculateInfluence(Panel i, Panel j, bool isTangential)
		{
			double dx = i.controlPoint.x - j.pointA.x;
			double dy = i.controlPoint.y - j.pointA.y;

			double cosT = System.Math.Cos(j.angle);
			double sinT = System.Math.Sin(j.angle);

			double xLocal = dx * cosT + dy * sinT;
			double yLocal = -dx * sinT + dy * cosT;

			double r1 = System.Math.Sqrt(xLocal * xLocal + yLocal * yLocal);
			double r2 = System.Math.Sqrt(System.Math.Pow(xLocal - j.length, 2) + yLocal * yLocal);
			
			double theta1 = System.Math.Atan2(yLocal, xLocal);
			double theta2 = System.Math.Atan2(yLocal, xLocal - j.length);

			double uLocal = (1.0 / (2.0 * System.Math.PI)) * System.Math.Log(r1 / r2);
			double vLocal = (1.0 / (2.0 * System.Math.PI)) * (theta2 - theta1);

			double refX = isTangential ? i.tangent.x : i.normal.x;
			double refY = isTangential ? i.tangent.y : i.normal.y;

			if (isTangential) return uLocal * (refX * cosT + refY * sinT) + vLocal * (-refX * sinT + refY * cosT);
			else return uLocal * (refX * cosT - refY * sinT) + vLocal * (refX * sinT + refY * cosT);
		}

		public Vector2D GetVelocityAt(Vector2D point)
		{
			double vx = freeStreamVelocity.x;
			double vy = freeStreamVelocity.y;

			for (int j = 0; j < panels.Length; j++)
			{
				Panel p = panels[j];
				
				double dx = point.x - p.pointA.x;
				double dy = point.y - p.pointA.y;

				double cosT = System.Math.Cos(p.angle);
				double sinT = System.Math.Sin(p.angle);

				double xLoc = dx * cosT + dy * sinT;
				double yLoc = -dx * sinT + dy * cosT;

				double r1 = System.Math.Sqrt(xLoc * xLoc + yLoc * yLoc);
				double r2 = System.Math.Sqrt(System.Math.Pow(xLoc - p.length, 2) + yLoc * yLoc);
				
				double theta1 = System.Math.Atan2(yLoc, xLoc);
				double theta2 = System.Math.Atan2(yLoc, xLoc - p.length);

				if (r1 < 1e-5) r1 = 1e-5;
				if (r2 < 1e-5) r2 = 1e-5;

				double uLoc = (lambdas[j] / (2.0 * System.Math.PI)) * System.Math.Log(r1 / r2);
				double vLoc = (lambdas[j] / (2.0 * System.Math.PI)) * (theta2 - theta1);

				vx += uLoc * cosT - vLoc * sinT;
				vy += uLoc * sinT + vLoc * cosT;
			}

			return new Vector2D(vx, vy);
		}
	}
}
