using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 150.0f;
	[Export] public PackedScene BulletScene { get; set; }
	
	[Export] public float SecondsPerAttack { get; set; } = 0.5f; 
	private bool _canShoot = true;

	// Configurações do Dash (Esquiva)
	[Export] public float DashSpeed { get; set; } = 500.0f;      // Velocidade durante o impulso
	[Export] public float DashDuration { get; set; } = 0.15f;    // Tempo em segundos que o dash dura
	[Export] public float DashCooldown { get; set; } = 5.0f;     // Tempo de recarga
	[Export] public ProgressBar DashBar { get; set; }            // Referência para a ProgressBar3

	private bool _isDashing = false;
	private bool _canDash = true;
	private float _dashCooldownTimer = 0.0f;
	private Vector2 _dashDirection = Vector2.Zero;

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

		// Busca automática da ProgressBar3 para o Dash
		if (DashBar == null)
		{
			DashBar = GetNodeOrNull<ProgressBar>("DashBar");
			if (DashBar == null && GetTree().CurrentScene != null)
			{
				DashBar = GetTree().CurrentScene.FindChild("ProgressBar3", true, false) as ProgressBar;
			}
		}

		UpdateHealthBar();
		InitDashBar();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Atualiza o tempo de recarga e o preenchimento da barra do Dash
		UpdateDashCooldown((float)delta);

		// Se estiver executando o Dash, aplica o impulso rápido e ignora os movimentos normais
		if (_isDashing)
		{
			Velocity = _dashDirection * DashSpeed;
			MoveAndSlide();
			return;
		}

		// 1. Movimentação (WASD / Setas + Trava do Shift)
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		bool isHoldingShift = Input.IsKeyPressed(Key.Shift);

		if (direction != Vector2.Zero && !isHoldingShift)
		{
			Velocity = direction * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();

		// Ativação do Dash (Tecla Espaço)
		if (Input.IsKeyPressed(Key.Space) && _canDash)
		{
			// Se estiver andando, dá o dash na direção do movimento; se estiver parado/Shift, dá na direção do mouse
			Vector2 targetDashDir = direction != Vector2.Zero 
				? direction 
				: (GetGlobalMousePosition() - GlobalPosition).Normalized();

			if (targetDashDir != Vector2.Zero)
			{
				StartDash(targetDashDir);
			}
		}

		// 2. Mira em 360° em direção ao cursor do rato
		Vector2 mousePosition = GetGlobalMousePosition();

		if (_muzzle != null)
		{
			_muzzle.LookAt(mousePosition);
		}

		// 3. Inverter o sprite de acordo com o lado do rato
		if (_animatedSprite != null)
		{
			if (mousePosition.X < GlobalPosition.X)
			{
				_animatedSprite.FlipH = false;
			}
			else if (mousePosition.X > GlobalPosition.X)
			{
				_animatedSprite.FlipH = true;
			}
		}

		// 4. Disparo (Suporta manter o botão pressionado respeitando o SecondsPerAttack)
		if (Input.IsActionPressed("shoot"))
		{
			Shoot();
		}
	}

	private async void StartDash(Vector2 dir)
	{
		_isDashing = true;
		_canDash = false;
		_dashDirection = dir;
		_dashCooldownTimer = DashCooldown;

		if (DashBar != null) DashBar.Value = 0;

		await ToSignal(GetTree().CreateTimer(DashDuration), SceneTreeTimer.SignalName.Timeout);

		_isDashing = false;
	}

	private void UpdateDashCooldown(float delta)
	{
		if (!_canDash)
		{
			_dashCooldownTimer -= delta;

			// Enche a barra continuamente durante os 5 segundos
			if (DashBar != null)
			{
				DashBar.Value = DashCooldown - Mathf.Max(_dashCooldownTimer, 0.0f);
			}

			if (_dashCooldownTimer <= 0)
			{
				_canDash = true;
				if (DashBar != null) DashBar.Value = DashCooldown;
			}
		}
	}

	private void InitDashBar()
	{
		if (DashBar != null)
		{
			DashBar.MaxValue = DashCooldown;
			DashBar.Value = DashCooldown;
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

	private async void Shoot()
	{
		// Se não puder atirar ou faltar a cena do tiro, ignora
		if (!_canShoot || BulletScene == null || _muzzle == null) return;

		_canShoot = false; // Trava o disparo temporariamente

		Node2D bulletInstance = BulletScene.Instantiate<Node2D>();
		GetTree().CurrentScene.AddChild(bulletInstance);

		bulletInstance.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance.GlobalRotation = _muzzle.GlobalRotation;

		// Aguarda o tempo definido em SecondsPerAttack para libertar o próximo tiro
		await ToSignal(GetTree().CreateTimer(SecondsPerAttack), SceneTreeTimer.SignalName.Timeout);
		_canShoot = true;
	}
}
