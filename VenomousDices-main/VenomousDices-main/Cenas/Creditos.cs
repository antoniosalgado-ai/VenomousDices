using Godot;
using System;

public partial class Creditos : Control
{
	public override void _Ready()
	{
		var sair = GetNode<Button>("TextureRect/sairBtn");
		sair.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://Cenas/telaInicial.tscn");
		};
	}
}
