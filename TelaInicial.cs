using Godot;
using System;

public partial class TelaInicial : Control
{
	public override void _Ready()
	{
		var quitBtn = GetNode<TextureButton>("MarginContainer/HBoxContainer/VBoxContainer/quit_btn");
		quitBtn.Pressed += () => GetTree().Quit();
		var startBtn = GetNode<TextureButton>("MarginContainer/HBoxContainer/VBoxContainer/start_btn");
		startBtn.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://fase1.tscn");
		};
		var creditsBtn = GetNode<TextureButton>("MarginContainer/HBoxContainer/VBoxContainer/creditos_btn");
		creditsBtn.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://creditos.tscn");
		};
		var configBtn = GetNode<TextureButton>("MarginContainer/HBoxContainer/VBoxContainer/config_btn");
		configBtn.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://config.tscn");
		};
	}
}
