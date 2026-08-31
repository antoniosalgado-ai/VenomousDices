using Godot;
using System;
public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 80.0f;

	private Node2D _player;

	public override void _Ready()
	{
		// Busca o jogador no grupo "player"
		_player = GetTree().GetFirstNodeInGroup("player") as Node2D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null) return;

		// Calcula a direção em direção ao jogador
		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		
		// Define a velocidade e move
		Velocity = direction * Speed;
		MoveAndSlide();
	}
}
