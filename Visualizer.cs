using Godot;
using System;
using System.Collections.Generic;
using AerodynamicsSimulator.Math;
using AerodynamicsSimulator.Geometry;
using Panel = AerodynamicsSimulator.Geometry.Panel;

public partial class Visualizer : Node2D
{
	private Panel[] cylinder;
	private PanelMethod simulator;
	
	private int numPanels = 40; 
	private float simRadius = 2.5f; 
	
	private List<Vector2[]> streamlines = new List<Vector2[]>();
	private List<Color[]> streamColors = new List<Color[]>();

	private float animationProgress = 0f;

	public override void _Ready()
	{
		RenderingServer.SetDefaultClearColor(new Color(0.2f, 0.2f, 0.2f));
		LoadGeometry(CreateCircle(simRadius, numPanels));
	}

	private void LoadGeometry(Panel[] newGeometry)
	{
		cylinder = newGeometry; 
		
		Vector2D wind = new Vector2D(10.0, 0.0);
		simulator = new PanelMethod(cylinder, wind);
		simulator.Solve();

		streamlines.Clear();
		streamColors.Clear();
		GenerateStreamlines();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Key1) LoadGeometry(CreateCircle(simRadius, numPanels));
			else if (keyEvent.Keycode == Key.Key2) LoadGeometry(CreateSquare(simRadius * 1.8f, numPanels / 4)); 
			else if (keyEvent.Keycode == Key.Key3) LoadGeometry(CreateTriangle(simRadius * 2.2f, numPanels / 3)); 
		}
	}

	public override void _Process(double delta)
	{
		animationProgress += (float)delta * 120f; 
		QueueRedraw();
	}

	private bool IsInsideGeometry(Vector2D pt)
	{
		foreach (var p in cylinder)
		{
			Vector2D ptToPanel = new Vector2D(pt.x - p.controlPoint.x, pt.y - p.controlPoint.y);
			
			// Permite que a partícula toque a superfície e escorregue por ela 
			// sem que o algoritmo a destrua prematuramente.
			if (ptToPanel.DotProduct(p.normal) > -0.05)
			{
				return false; 
			}
		}
		return true;
	}

	private void GenerateStreamlines()
	{
		float stepSize = 0.02f; 
		float maxVelocity = 20.0f; 

		for (float startY = -4.5f; startY <= 4.5f; startY += 0.25f)
		{
			List<Vector2> points = new List<Vector2>();
			List<Color> colors = new List<Color>();

			Vector2D currentPos = new Vector2D(-6.0, startY);

			for (int step = 0; step < 2000; step++)
			{
				if (IsInsideGeometry(currentPos)) break;
				if (currentPos.x > 6.0) break;

				Vector2D vel = simulator.GetVelocityAt(currentPos);
				
				points.Add(new Vector2((float)currentPos.x * 100f, -(float)currentPos.y * 100f));
				colors.Add(GetColorForVelocity(vel.Magnitude(), maxVelocity));

				currentPos.x += vel.x * stepSize;
				currentPos.y += vel.y * stepSize;
			}

			if (points.Count > 1)
			{
				streamlines.Add(points.ToArray());
				streamColors.Add(colors.ToArray());
			}
		}
	}

	public override void _Draw()
	{
		if (cylinder == null || simulator == null) return;
		Vector2 screenCenter = GetViewportRect().Size / 2;

		int particleSpacing = 60; 
		int tailLength = 4;       

		for (int i = 0; i < streamlines.Count; i++)
		{
			Vector2[] pts = streamlines[i];
			Color[] cls = streamColors[i];

			Color[] faintColors = new Color[cls.Length];
			Vector2[] screenPts = new Vector2[pts.Length];
			for (int j = 0; j < pts.Length; j++) 
			{
				faintColors[j] = new Color(cls[j].R, cls[j].G, cls[j].B, 0.15f);
				screenPts[j] = pts[j] + screenCenter;
			}
			DrawPolylineColors(screenPts, faintColors, 1.5f, true);

			for (int offset = 0; offset < pts.Length; offset += particleSpacing)
			{
				int currentIndex = (int)(offset + animationProgress) % pts.Length;

				if (currentIndex >= tailLength)
				{
					Vector2 headPos = pts[currentIndex] + screenCenter;
					Vector2 tailPos = pts[currentIndex - tailLength] + screenCenter;
					Color color = cls[currentIndex];

					DrawLine(tailPos, headPos, color, 2.5f, true);
					DrawCircle(headPos, 2.0f, Colors.White);
				}
			}
		}

		for (int i = 0; i < cylinder.Length; i++)
		{
			var p = cylinder[i];
			double cp = simulator.pressureCoefficients[i];

			Vector2 p1 = new Vector2((float)p.pointA.x * 100f, -(float)p.pointA.y * 100f) + screenCenter;
			Vector2 p2 = new Vector2((float)p.pointB.x * 100f, -(float)p.pointB.y * 100f) + screenCenter;

			Color panelColor = GetColorForCp(cp);
			DrawLine(p1, p2, panelColor, 6.0f, true);
		}
	}

	private Color GetColorForVelocity(double v, double vMax)
	{
		double ratio = System.Math.Clamp(v / vMax, 0.0, 1.0);
		float hue = (float)(0.66 * (1.0 - ratio)); 
		return Color.FromHsv(hue, 1.0f, 1.0f);
	}

	private Color GetColorForCp(double cp)
	{
		double minCp = -3.0;
		double maxCp = 1.0;
		double ratio = System.Math.Clamp((cp - minCp) / (maxCp - minCp), 0.0, 1.0);
		float hue = (float)(0.66 * (1.0 - ratio));
		return Color.FromHsv(hue, 1.0f, 1.0f);
	}

	private Panel[] CreateCircle(float radius, int segments)
	{
		Panel[] panels = new Panel[segments];
		Vector2D[] points = new Vector2D[segments + 1];

		for (int i = 0; i <= segments; i++)
		{
			double theta = 2.0 * System.Math.PI * i / segments;
			double x = radius * System.Math.Cos(theta);
			double y = -radius * System.Math.Sin(theta);
			points[i] = new Vector2D(x, y);
		}

		for (int i = 0; i < segments; i++) panels[i] = new Panel(points[i], points[i + 1]);
		return panels;
	}

	private Panel[] CreateSquare(float side, int panelsPerSide)
	{
		List<Vector2D> pts = new List<Vector2D>();
		float half = side / 2.0f;

		for (int i = 0; i < panelsPerSide; i++) pts.Add(new Vector2D(-half + (side * i / panelsPerSide), half));
		for (int i = 0; i < panelsPerSide; i++) pts.Add(new Vector2D(half, half - (side * i / panelsPerSide)));
		for (int i = 0; i < panelsPerSide; i++) pts.Add(new Vector2D(half - (side * i / panelsPerSide), -half));
		for (int i = 0; i < panelsPerSide; i++) pts.Add(new Vector2D(-half, -half + (side * i / panelsPerSide)));
		pts.Add(pts[0]);

		Panel[] panels = new Panel[pts.Count - 1];
		for (int i = 0; i < panels.Length; i++) panels[i] = new Panel(pts[i], pts[i + 1]);
		return panels;
	}

	private Panel[] CreateTriangle(float size, int panelsPerSide)
	{
		List<Vector2D> pts = new List<Vector2D>();
		float half = size / 2.0f;

		for (int i = 0; i < panelsPerSide; i++)
		{
			float t = (float)i / panelsPerSide;
			pts.Add(new Vector2D(-half + (size * t), 0 + (half * t)));
		}
		for (int i = 0; i < panelsPerSide; i++)
		{
			float t = (float)i / panelsPerSide;
			pts.Add(new Vector2D(half, half - (size * t)));
		}
		for (int i = 0; i < panelsPerSide; i++)
		{
			float t = (float)i / panelsPerSide;
			pts.Add(new Vector2D(half - (size * t), -half + (half * t)));
		}
		pts.Add(pts[0]);

		Panel[] panels = new Panel[pts.Count - 1];
		for (int i = 0; i < panels.Length; i++) panels[i] = new Panel(pts[i], pts[i + 1]);
		return panels;
	}
}
