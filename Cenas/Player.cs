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
		AddToGroup("player");

		_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_muzzle = GetNodeOrNull<Marker2D>("Muzzle");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			if (_animatedSprite != null)
			{
				if (direction.X < 0)
				{
					_animatedSprite.FlipH = false;
					if (_muzzle != null) _muzzle.RotationDegrees = 180f;
				}
				else if (direction.X > 0)
				{
					_animatedSprite.FlipH = true;
					if (_muzzle != null) _muzzle.RotationDegrees = 0f;
				}
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();

		if (Input.IsActionJustPressed("shoot"))
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		if (BulletScene == null)
		{
			GD.Print("ERRO: BulletScene está nula no Inspector do Player!");
			return;
		}

		if (_muzzle == null)
		{
			GD.Print("ERRO: Nó Muzzle não foi encontrado!");
			return;
		}

		Node bulletInstance = BulletScene.Instantiate();
		
		if (bulletInstance is Node2D bullet2D)
		{
			bullet2D.GlobalPosition = _muzzle.GlobalPosition;
			bullet2D.GlobalRotation = _muzzle.GlobalRotation;
		}

		GetTree().CurrentScene.AddChild(bulletInstance);
	}
}
