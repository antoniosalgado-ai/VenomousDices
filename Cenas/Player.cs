using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 150.0f;
	[Export] public PackedScene BulletScene { get; set; }
	
	// Configurações de Vida
	[Export] public int MaxHealth { get; set; } = 50;
	public int CurrentHealth { get; private set; }
	[Export] public float DamageCooldown { get; set; } = 2.0f;
	
	[Export] public ProgressBar HealthBar { get; set; }

	private bool _canTakeDamage = true;
	private AnimatedSprite2D _animatedSprite;
	private Marker2D _muzzle;

	public override void _Ready()
	{
		AddToGroup("player");
		CurrentHealth = MaxHealth;

		_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_muzzle = GetNodeOrNull<Marker2D>("Muzzle");
		if (HealthBar == null) HealthBar = GetNodeOrNull<ProgressBar>("ProgressBar");

		UpdateHealthBar();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			
			// Gira o Muzzle para a direção exata do movimento (8 direções)
			if (_muzzle != null)
			{
				_muzzle.Rotation = direction.Angle();
			}

			// Controla o flip da imagem do personagem (esquerda/direita)
			if (_animatedSprite != null)
			{
				if (direction.X < 0) _animatedSprite.FlipH = false;
				else if (direction.X > 0) _animatedSprite.FlipH = true;
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

	public void TakeDamage(int amount)
	{
		if (!_canTakeDamage) return;

		CurrentHealth -= amount;
		UpdateHealthBar();
		GD.Print($"DANO! Vida do Player: {CurrentHealth}/{MaxHealth}");

		if (CurrentHealth <= 0)
		{
			Die();
			return;
		}

		StartDamageCooldown();
	}

	private async void StartDamageCooldown()
	{
		_canTakeDamage = false;
		await ToSignal(GetTree().CreateTimer(DamageCooldown), SceneTreeTimer.SignalName.Timeout);
		_canTakeDamage = true;
	}

	private void UpdateHealthBar()
	{
		if (HealthBar != null)
		{
			HealthBar.MaxValue = MaxHealth;
			HealthBar.Value = CurrentHealth;
		}
	}

	private void Die()
	{
		GD.Print("GAME OVER!");
		GetTree().ReloadCurrentScene();
	}

	private void Shoot()
	{
		if (BulletScene == null || _muzzle == null) return;

		Node2D bulletInstance = BulletScene.Instantiate<Node2D>();

		GetTree().CurrentScene.AddChild(bulletInstance);

		bulletInstance.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance.GlobalRotation = _muzzle.GlobalRotation;
	}
}
