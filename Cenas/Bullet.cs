using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export] public float Speed { get; set; } = 400.0f;
	[Export] public int Damage { get; set; } = 1;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Vector2.Right.Rotated(Rotation) * Speed * (float)delta;
	}

	private void OnBodyEntered(Node2D body)
	{
		// Ignora o jogador
		if (body is Player || body.IsInGroup("player")) return;

		// Se atingir um inimigo, causa 1 de dano
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
		}

		// Destrói a bala após o impacto (em inimigos ou paredes)
		QueueFree();
	}
}
