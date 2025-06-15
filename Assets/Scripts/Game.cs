using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Vector2Int current_room_coord;
    private RoomConfiguration current_config;
    private RoomStructure current_room;

    public Level level;
    public UISystem ui_system;

    private void enter_room(int x, int y)
    {
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
                case RoomType.upgrade:
                    complete(x, y);
                    break;
                case RoomType.simple:
                    start_combat();
                    break;
                case RoomType.boss:
                    start_combat(true);
                    break;
            }
        }
    }

    private void start_combat(bool boss = false)
    {
        current_room.close_doors();
        // spawn enemies/boss
    }

    private void complete(int x, int y)
    {
        level.complete(x, y);
        current_room.open_doors();
    }

    void Start()
    {
        ui_system.initialize(); //for now

        level.initialize();
        Vector2Int start = level.generate(3);
        level.construct_3D_map();
        UISystem.ui_map.display();

        enter_room(start.x, start.y);
    }

    void Update()
    {
        if (!current_config.completed)
        {
            // check criterion
            // complete(current_room_coord.x, current_room_coord.y)
        }
        else
        {
            // check room transition
        }
    }
}
