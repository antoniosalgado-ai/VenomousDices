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
		//_animatedSprite.Play("idle");
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
				_muzzle.RotationDegrees = 180f;
			}
			else if (direction.X > 0)
			{
				_animatedSprite.FlipH = true;
				_muzzle.RotationDegrees = 0f;
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();

		// Detecta a ação de atirar a cada quadro da física
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

		GD.Print("Tiro disparado com sucesso!");

		Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
		bulletInstance.GlobalTransform = _muzzle.GlobalTransform;
		GetTree().CurrentScene.AddChild(bulletInstance);
	}
}
