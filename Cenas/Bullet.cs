using Godot;
using System;
public partial class Bullet : Area2D
{
	[Export] public float Speed = 400f;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.X * Speed * (float)delta;
	}
	private void OnBodyEntered(Node2D body)
{
	GD.Print("Bala colidiu com: " + body.Name);
	// QueueFree(); <--- Coloque as duas barras aqui temporariamente!
}
}
