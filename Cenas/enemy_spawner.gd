extends Node2D

@export var enemy_scenes: Array[PackedScene] = [
	preload("res://Cenas/Enemy.tscn"),
]

@export var spawn_interval: float = 1.0

var timer: Timer

func _ready() -> void:
	timer = Timer.new()
	timer.wait_time = spawn_interval
	timer.autostart = true
	timer.one_shot = false
	timer.timeout.connect(_on_spawn_timeout)
	add_child(timer)

func _on_spawn_timeout() -> void:
	spawn_enemy()

func spawn_enemy() -> void:
	if enemy_scenes.is_empty():
		return
		
	var spawn_points = get_tree().get_nodes_in_group("spawn_points")
	if spawn_points.is_empty():
		print("Aviso: Nenhum Marker2D encontrado no grupo 'spawn_points'!")
		return
		
	var random_point = spawn_points.pick_random() as Node2D
	var chosen_enemy_scene = enemy_scenes.pick_random()
	
	if chosen_enemy_scene and random_point:
		var enemy = chosen_enemy_scene.instantiate() as Node2D
		
		# 1º Adiciona o nó na cena primeiro
		get_tree().current_scene.add_child(enemy)
		
		# 2º Define a posição global depois de entrar na árvore (evita nascer fora da tela)
		enemy.global_position = random_point.global_position
