using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 130.0f;
	[Export] public int DamageAmount { get; set; } = 10;
	
	// Vida do Inimigo (2 acertos para morrer)
	[Export] public int Health { get; set; } = 2;

	// Nome da animação criada no SpriteFrames (ex: "default", "walk", "andar")
	[Export] public string AnimationName { get; set; } = "default";

	private Node2D _player;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		AddToGroup("inimigos");
		
		// Busca o nó de animação
		_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

		// Executa a animação ao nascer
		if (_animatedSprite != null)
		{
			if (_animatedSprite.SpriteFrames != null && _animatedSprite.SpriteFrames.HasAnimation(AnimationName))
			{
				_animatedSprite.Play(AnimationName);
			}
			else
			{
				_animatedSprite.Play();
			}
		}

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

		// Atualiza o flip do sprite para olhar na direção em que se move (esquerda/direita)
		if (_animatedSprite != null && direction != Vector2.Zero)
		{
			if (direction.X < 0)
			{
				_animatedSprite.FlipH = false; // Altere para true caso seu sprite base olhe para a direita
			}
			else if (direction.X > 0)
			{
				_animatedSprite.FlipH = true;
			}
		}
		
		MoveAndSlide();

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

	public void TakeDamage(int amount)
	{
		Health -= amount;
		GD.Print($"Inimigo atingido! Vida restante: {Health}");

		if (Health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		GD.Print("Inimigo derrotado!");
		// Envia a mensagem para o script GDScript do Spawner
		GetTree().CallGroup("spawner", "on_enemy_killed");
		QueueFree();
	}
}
