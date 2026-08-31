using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 100.0f;
	[Export] public int DamageAmount { get; set; } = 10;

	private Node2D _player;

	public override void _Ready()
	{
		AddToGroup("inimigos");
		FindPlayer();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null)
		{
			FindPlayer();
			if (_player == null) return;
		}

		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		
		MoveAndSlide();

		// Verifica colisões físicas após se mover
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			if (collision.GetCollider() is Player player)
			{
				player.TakeDamage(DamageAmount);
			}
		}
	}

	private void FindPlayer()
	{
		_player = GetTree().GetFirstNodeInGroup("player") as Node2D;
	}
}
