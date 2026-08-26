using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 150.0f;
	[Export] public PackedScene BulletScene { get; set; }
	private AnimatedSprite2D _animatedSprite;
	private Marker2D _muzzle;
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
		_muzzle = GetNode<Marker2D>("Muzzle");
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			if (direction.X < 0)
			{
				_animatedSprite.FlipH = false;
				_muzzle.RotationDegrees = 180f; // Aponta o tiro para a esquerda
			}
			else if (direction.X > 0)
			{
				_animatedSprite.FlipH = true;
				_muzzle.RotationDegrees = 0f; // Aponta o tiro para a direita
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("shoot"))
		{
			Shoot();
		}
	}
	private void Shoot()
	{
		if (BulletScene == null) return;
		Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
		bulletInstance.GlobalTransform = _muzzle.GlobalTransform;
		GetTree().CurrentScene.AddChild(bulletInstance);
	}
}
