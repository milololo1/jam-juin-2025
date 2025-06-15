using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Vector2Int current_room_coord;
    private RoomConfiguration current_config;
    private RoomStructure current_room;
    private List<Enemy> enemy_room_list;

    public Level level;
    public UISystem ui_system;
    public CameraControler camera_controler;
    public PlayerStats player_stats;
    public CharacterMovement character_movement;
    public GameObject upgrade_item;

    public GameObject globule_rouge_enemy;
    public GameObject anti_corps_enemy;

    private void enter_room(Vector2Int v, bool skip_transition=false) { enter_room(v.x, v.y, skip_transition); }

    private void enter_room(int x, int y, bool skip_transition=false)
    {
        camera_controler.transition(current_room_coord, new Vector2Int(x, y), skip_transition);

        if (!skip_transition) // ensure the player is not stuck in the door
        {
            Vector3 pos = character_movement.transform.position;
            switch (character_movement.get_quadrant())
            {
                case Direction.north:
                    pos.y = (x + 0.4f) * RoomStructure.full_room_width;
                    break;
                case Direction.south:
                    pos.y = (x - 0.4f) * RoomStructure.full_room_width;
                    break;
                case Direction.east:
                    pos.x = (y + 0.4f) * RoomStructure.full_room_length;
                    break;
                case Direction.west:
                    pos.x = (y - 0.4f) * RoomStructure.full_room_length;
                    break;
            }
            character_movement.transform.position = pos;
        }

        current_room_coord = new Vector2Int(x, y);
        current_config = level.get_configuration(x, y);
        current_room = level.get_room(x, y);

        if (!current_config.completed)
        {
            switch (current_config.type)
            {
                default:
                case RoomType.start:
                case RoomType.none:
                case RoomType.simple:
                    complete();
                    break;
                case RoomType.upgrade:
                    create_upgrade_item();
                    break;
                case RoomType.fight:
                    start_combat();
                    break;
                case RoomType.boss:
                    start_combat(true);
                    break;
            }
        }
    }

    private void create_upgrade_item()
    {
        current_room.close_doors();
        var item = Instantiate(upgrade_item);
        item.transform.position = new Vector3(current_room_coord.y * RoomStructure.full_room_length, current_room_coord.x * RoomStructure.full_room_width - 4, 0);
        item.GetComponent<UpgradeItem>().game = this;
    }

    private void spawn_enemies()
    {
        int random_value = Random.Range(0, 2);
        GameObject enemy;
        if(random_value == 0)
        {
            enemy = Instantiate(globule_rouge_enemy);
            enemy.GetComponent<Enemy>().strategy = EnemyStrategy.nothing;
        }
        else
        {
            enemy = Instantiate(anti_corps_enemy);
            enemy.GetComponent<Enemy>().strategy = EnemyStrategy.move_to_player;
        }
        enemy.transform.position = new Vector3(current_room_coord.y * RoomStructure.full_room_length, current_room_coord.x * RoomStructure.full_room_width, 0);
        enemy_room_list.Add(enemy.GetComponent<Enemy>());
    }

    private void spawn_boss()
    {

    }

    private void start_combat(bool boss = false)
    {
        current_room.close_doors();
        if(boss)
        {
            spawn_boss();
        }
        else
        {
            spawn_enemies();
        }
    }

    public void complete()
    {
        int x = current_room_coord.x;
        int y = current_room_coord.y;
        level.complete(x, y);
        current_config = level.get_configuration(x, y);
        current_room.open_doors();
    }

    void Start()
    {
        ui_system.initialize(); //for now

        level.initialize();
        Vector2Int start = level.generate(3, 10);
        level.construct_3D_map();
        UISystem.ui_map.display();

        character_movement.transform.position = new Vector3(start.y * RoomStructure.full_room_length, start.x * RoomStructure.full_room_width, 0);
        CharacterMovement.room_location = start; // for safety
        enter_room(start.x, start.y, true);

        enemy_room_list = new List<Enemy>();
    }

    void Update()
    {

        if (!current_config.completed)
        {
            // check criterion
            // complete(current_room_coord.x, current_room_coord.y)
            if(current_config.type == RoomType.fight)
            {
                bool all_enemy_dead = true;
                foreach(var enemy in enemy_room_list)
                {
                    all_enemy_dead &= enemy.is_dead;
                }

                if(all_enemy_dead)
                {
                    foreach(var enemy in enemy_room_list)
                    {
                        enemy.destroy_enemy();
                    }
                    enemy_room_list.Clear();
                    complete();
                }
            }

            //DEBUG
            //if (Input.GetKeyDown(KeyCode.Space)) complete();
        }
        else
        {
            // check room transition
            if (CharacterMovement.room_location != current_room_coord)
            {
                enter_room(CharacterMovement.room_location);
            }


            //DEBUG
            /*
            if (current_config.north && Input.GetKeyDown(KeyCode.UpArrow)) enter_room(current_room_coord.x + 1, current_room_coord.y);
            else if (current_config.south && Input.GetKeyDown(KeyCode.DownArrow)) enter_room(current_room_coord.x - 1, current_room_coord.y);
            else if (current_config.west && Input.GetKeyDown(KeyCode.LeftArrow)) enter_room(current_room_coord.x, current_room_coord.y - 1);
            else if (current_config.east && Input.GetKeyDown(KeyCode.RightArrow)) enter_room(current_room_coord.x, current_room_coord.y + 1);
            */
        }
    }
}
