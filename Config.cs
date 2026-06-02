using Godot;
using System;

public partial class Config : Control
{
	public override void _Ready()
	{
		var voltar = GetNode<TextureButton>("TextureRect/voltar_btn");
		voltar.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://telaInicial.tscn");
		};
	}
}
