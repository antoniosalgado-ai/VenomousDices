using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 150.0f;
	private AnimatedSprite2D animatedSprite;
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.Play("idle");
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			Velocity= direction * Speed;
			if (direction.X>0)
			{
				animatedSprite.FlipH= false;
			}
			else if (direction.X < 0)
			{
				animatedSprite.FlipH = true;
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}
}
