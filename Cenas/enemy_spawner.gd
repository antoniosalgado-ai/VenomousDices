extends Node2D

@export var enemy_scenes: Array[PackedScene] = [
	preload("res://Cenas/Enemy.tscn"),
	preload("res://Cenas/Enemy2.tscn")
]

# Quantidades individuais para cada tipo de inimigo no Array acima
@export var enemy_1_count: int = 20
@export var enemy_2_count: int = 10

@export var spawn_interval: float = 2.0
@export var next_scene_path: String = "res://Cenas/fase2.tscn"

# Referência para a barra de abates no Inspector
@export var kill_bar: ProgressBar

var enemies_spawned: int = 0
var enemies_killed: int = 0
var max_enemies: int = 0
var timer: Timer
var spawn_queue: Array[PackedScene] = []

func _ready() -> void:
	add_to_group("spawner")
	
	# Monta a fila com a quantidade exata de cada inimigo
	if enemy_scenes.size() > 0 and enemy_1_count > 0:
		for i in range(enemy_1_count):
			spawn_queue.append(enemy_scenes[0])
			
	if enemy_scenes.size() > 1 and enemy_2_count > 0:
		for i in range(enemy_2_count):
			spawn_queue.append(enemy_scenes[1])
			
	# Mistura a ordem de aparecimento
	spawn_queue.shuffle()
	
	# Calcula o total máximo da fase automaticamente
	max_enemies = spawn_queue.size()
	
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
	if spawn_queue.is_empty():
		timer.stop()
		return
		
	var spawn_points = get_tree().get_nodes_in_group("spawn_points")
	if spawn_points.is_empty():
		print("Aviso: Nenhum Marker2D encontrado no grupo 'spawn_points'!")
		return
		
	var random_point = spawn_points.pick_random() as Node2D
	
	# Pega e remove o próximo inimigo da fila misturada
	var chosen_enemy_scene = spawn_queue.pop_back()
	
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
	
	if enemies_killed >= max_enemies and max_enemies > 0:
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
