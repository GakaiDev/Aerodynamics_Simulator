using System;

namespace AerodynamicsSimulator.Math
{
	public class MatrixSolver
	{
		public static double[] Solve(double[][] A, double[] b)
		{
			int n = b.Length;
			for (int p = 0; p < n; p++)
			{
				int max = p;
				for (int i = p + 1; i < n; i++)
				{
					if (System.Math.Abs(A[i][p]) > System.Math.Abs(A[max][p])) max = i;
				}

				double[] tempA = A[p]; A[p] = A[max]; A[max] = tempA;
				double tempB = b[p]; b[p] = b[max]; b[max] = tempB;

				if (System.Math.Abs(A[p][p]) <= 1e-10)
					throw new ArithmeticException("Matriz singular detectada.");

				for (int i = p + 1; i < n; i++)
				{
					double alpha = A[i][p] / A[p][p];
					b[i] -= alpha * b[p];
					for (int j = p; j < n; j++) A[i][j] -= alpha * A[p][j];
				}
			}

			double[] x = new double[n];
			for (int i = n - 1; i >= 0; i--)
			{
				double sum = 0.0;
				for (int j = i + 1; j < n; j++) sum += A[i][j] * x[j];
				x[i] = (b[i] - sum) / A[i][i];
			}
			return x;
		}
	}
}
