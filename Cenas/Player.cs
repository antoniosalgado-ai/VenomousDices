using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 150.0f;
	[Export] public PackedScene BulletScene { get; set; }
	
	// Configurações de Vida e UI
	[Export] public int MaxHealth { get; set; } = 50;
	public int CurrentHealth { get; private set; }
	[Export] public float DamageCooldown { get; set; } = 2.0f;
	
	// Referência para a barra de vida na interface
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

		UpdateHealthBar();
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

	public void TakeDamage(int amount)
	{
		if (!_canTakeDamage) return;

		CurrentHealth -= amount;
		UpdateHealthBar();
		GD.Print($"DANO! Vida restante do Player: {CurrentHealth}/{MaxHealth}");

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

		// Aguarda os 2 segundos de cooldown mantendo a aparência normal do personagem
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
		GD.Print("GAME OVER! O jogador foi derrotado.");
		GetTree().ReloadCurrentScene();
	}

	private void Shoot()
	{
		if (BulletScene == null || _muzzle == null) return;

		Node bulletInstance = BulletScene.Instantiate();
		
		if (bulletInstance is Node2D bullet2D)
		{
			bullet2D.GlobalPosition = _muzzle.GlobalPosition;
			bullet2D.GlobalRotation = _muzzle.GlobalRotation;
		}

		GetTree().CurrentScene.AddChild(bulletInstance);
	}
}
