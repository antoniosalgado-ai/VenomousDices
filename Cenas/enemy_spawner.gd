extends Node2D

@export var enemy_scenes: Array[PackedScene] = [
	preload("res://Cenas/Enemy.tscn"),
]

@export var spawn_interval: float = 2.5
@export var max_enemies: int = 20
@export var next_scene_path: String = "res://Cenas/fase2.tscn"

# Referência para a barra de abates no Inspector
@export var kill_bar: ProgressBar

var enemies_spawned: int = 0
var enemies_killed: int = 0
var timer: Timer

func _ready() -> void:
	add_to_group("spawner")
	
	# Se não foi arrastada pelo Inspector, busca automaticamente pelo nome ProgressBar2
	if not kill_bar:
		kill_bar = get_tree().current_scene.find_child("ProgressBar2", true, false) as ProgressBar

	update_kill_bar()
	
	timer = Timer.new()
	timer.wait_time = spawn_interval
	timer.autostart = true
	timer.one_shot = false
	timer.timeout.connect(_on_spawn_timeout)
	add_child(timer)

func _on_spawn_timeout() -> void:
	spawn_enemy()

func spawn_enemy() -> void:
	if enemies_spawned >= max_enemies:
		timer.stop()
		return
		
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
		get_tree().current_scene.add_child(enemy)
		enemy.global_position = random_point.global_position
		
		enemies_spawned += 1
		print("Inimigo Spawnado: ", enemies_spawned, "/", max_enemies)

func on_enemy_killed() -> void:
	enemies_killed += 1
	update_kill_bar()
	print("Abates: ", enemies_killed, "/", max_enemies)
	
	if enemies_killed >= max_enemies:
		print("Fase Concluída! Carregando próxima fase...")
		if ResourceLoader.exists(next_scene_path):
			get_tree().change_scene_to_file(next_scene_path)
		else:
			print("Aviso: Cena '", next_scene_path, "' não foi encontrada. Reiniciando a fase atual.")
			get_tree().reload_current_scene()

func update_kill_bar() -> void:
	if kill_bar:
		kill_bar.max_value = max_enemies
		kill_bar.value = enemies_killed
