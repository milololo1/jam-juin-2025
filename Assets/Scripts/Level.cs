using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    none,
    simple,
    upgrade,
    fight,
    boss,
    start
}

public enum Direction
{
    north,
    south,
    east,
    west
}

public struct RoomConfiguration
{
    public bool north, south, east, west;
    public RoomType type;
    public bool completed;
}

public class Level : MonoBehaviour
{
    public int width, length;

    private RoomType[,] rooms;
    private bool[,] vertical_doors;
    private bool[,] horizontal_doors;
    private bool[,] completed;
    private RoomStructure[,] room_structs;

    public GameObject room_prefab;

    public void initialize()
    {
        rooms = new RoomType[width, length];
        vertical_doors = new bool[width - 1, length];
        horizontal_doors = new bool[width, length - 1];
        completed = new bool[width, length];
        room_structs = new RoomStructure[width, length];
    }

    public RoomConfiguration get_configuration(int x, int y)
    {
        RoomConfiguration config = new RoomConfiguration();
        config.type = rooms[x, y];

        if (x >= width - 1) config.north = false;
        else config.north = vertical_doors[x, y];

        if (x-1 < 0) config.south = false;
        else config.south = vertical_doors[x-1, y];


        if (y >= length - 1) config.east = false;
        else config.east = horizontal_doors[x, y];

        if (y - 1 < 0) config.west = false;
        else config.west = horizontal_doors[x, y-1];

        config.completed = completed[x, y];

        return config;
    }

    public RoomStructure get_room(int x, int y)
    {
        return room_structs[x, y];
    }

    public void complete(int x, int y)
    {
        completed[x, y] = true;
    }

    private void add_door(int x_1, int y_1, int x_2, int y_2)
    {
        int diff_x = Mathf.Abs(x_2 - x_1);
        int diff_y = Mathf.Abs(y_2 - y_1);

        if (diff_x > 1 || diff_y > 1) return;
        if (diff_x > 0 && diff_y > 0) return;

        if (diff_x > 0) // vertical
        {
            vertical_doors[Mathf.Min(x_1, x_2), y_1] = true;
        }
        else if (diff_y > 0) // horizontal
        {
            horizontal_doors[x_1, Mathf.Min(y_1, y_2)] = true;
        }
    }

    private void add_door(int x, int y, Direction dir)
    {
        x = dir == Direction.south ? x - 1 : x;
        y = dir == Direction.west ? y - 1 : y;

        if (dir == Direction.north || dir == Direction.south)
        {
            if (x > -1 && x < width - 1) vertical_doors[x, y] = true;
        }
        else
        {
            if (y > -1 && y < length - 1) horizontal_doors[x, y] = true;
        }
    }

    private void close_door(int x_1, int y_1, int x_2, int y_2)
    {
        int diff_x = Mathf.Abs(x_2 - x_1);
        int diff_y = Mathf.Abs(y_2 - y_1);

        if (diff_x > 1 || diff_y > 1) return;
        if (diff_x > 0 && diff_y > 0) return;

        if (diff_x > 0) // vertical
        {
            vertical_doors[Mathf.Min(x_1, x_2), y_1] = false;
        }
        else if (diff_y > 0) // horizontal
        {
            horizontal_doors[x_1, Mathf.Min(y_1, y_2)] = false;
        }
    }

    private void close_door(int x, int y, Direction dir)
    {
        x = dir == Direction.south ? x - 1 : x;
        y = dir == Direction.west ? y - 1 : y;

        if (dir == Direction.north || dir == Direction.south)
        {
            if (x > -1 && x < width - 1) vertical_doors[x, y] = false;
        }
        else
        {
            if (y > -1 && y < length - 1) horizontal_doors[x, y] = false;
        }
    }

    private void close_all_doors(int x, int y)
    {
        for (int dx = -1; dx < 2; dx += 2)
        {
            if (x + dx > -1 && x + dx < width)
            {
                close_door(x, y, x + dx, y);
            }
        }
        for (int dy = -1; dy < 2; dy += 2)
        {
            if (y + dy > -1 && y + dy < length)
            {
                close_door(x, y, x, y + dy);
            }
        }
    }

    private void add_room(int x, int y, RoomType type = RoomType.simple, float p_connect = 0.25f)
    {
        rooms[x, y] = type;

        // connect to existing (non-boss) rooms with probability p_connect
        for (int dx = -1; dx < 2; dx +=2)
        {
            if (x + dx > -1 && x + dx < width)
            {
                RoomType next_type = rooms[x + dx, y];
                if (next_type != RoomType.none && next_type != RoomType.boss && next_type != RoomType.upgrade)
                {
                    if (Random.value < p_connect) add_door(x, y, x + dx, y);
                }
            }  
        }
        for (int dy = -1; dy < 2; dy += 2)
        {
            if (y + dy > -1 && y + dy < length)
            {
                RoomType next_type = rooms[x, y + dy];
                if (next_type != RoomType.none && next_type != RoomType.boss && next_type != RoomType.upgrade)
                {
                    if (Random.value < p_connect) add_door(x, y, x, y + dy);
                }
            }
        }
    }

    private void add_room(int old_x, int old_y, int x, int y, RoomType type = RoomType.simple, float p_connect = 0.25f)
    {
        add_door(old_x, old_y, x, y);
        add_room(x, y, type, p_connect);
    }

    private int manhattan_distance(Vector2Int v, Vector2Int u)
    {
        return Mathf.Abs(v.x - u.x) + Mathf.Abs(v.y - u.y);
    }

    private List<Vector2Int> generate_main_path(Vector2Int origin, Vector2Int end)
    {
        List<Vector2Int> created_rooms = new List<Vector2Int>();
        created_rooms.Add(origin);

        Vector2Int current = origin;

        int x_move_counter = 0; //Limit vertical movement

        // add start
        add_room(origin.x, origin.y, RoomType.start);

        // create a path from origin to end (on the last rank) where each transition is either up/down/right
        while (manhattan_distance(current, end) > 0)
        {
            // search possible transition
            List<Vector2Int> possible_nodes = new List<Vector2Int>();

            if (current.y == end.y) // last-rank -> only up/down to the end room
            {
                int dx = current.x > end.x ? -1 : 1;
                possible_nodes.Add(new Vector2Int(current.x + dx, current.y));
            }
            else
            {
                if(x_move_counter < 3)
                {
                    for (int dx = -1; dx < 2; dx += 2)
                    {
                        if (current.x + dx > -1 && current.x + dx < width)
                        {
                            if (rooms[current.x + dx, current.y] == RoomType.none) possible_nodes.Add(new Vector2Int(current.x + dx, current.y));
                        }
                    }
                }
                possible_nodes.Add(new Vector2Int(current.x, current.y + 1));
            }

            // create new room
            int rand_index = Random.Range(0, possible_nodes.Count); // add an extra proba to go right
            Vector2Int next = possible_nodes[Mathf.Min(rand_index, possible_nodes.Count-1)];
            if (manhattan_distance(next, end) > 0)
            {
                add_room(current.x, current.y, next.x, next.y);
                created_rooms.Add(next);
                x_move_counter = next.y > current.y ? 0 : x_move_counter+1;
            }
            else add_room(current.x, current.y, next.x, next.y, RoomType.boss, 0);
            current = next;
        }
        return created_rooms;
    }

    private bool generate_side_path(Vector2Int start, int max_length = 5, float p_connect = 0.05f, bool create_upgrade_room = false)
    {
        Vector2Int current = start;
        bool has_generated_path = false;

        while (max_length > 0)
        {
            // search possible transition
            List<Vector2Int> possible_nodes = new List<Vector2Int>();

            for (int dx = -1; dx < 2; dx += 2)
            {
                if (current.x + dx > -1 && current.x + dx < width)
                {
                    if (rooms[current.x + dx, current.y] == RoomType.none) possible_nodes.Add(new Vector2Int(current.x + dx, current.y));
                }
            }
            for (int dy = -1; dy < 2; dy += 2)
            {
                if (current.y + dy > -1 && current.y + dy < length)
                {
                    if (rooms[current.x, current.y + dy] == RoomType.none) possible_nodes.Add(new Vector2Int(current.x, current.y + dy));
                }
            }

            if (possible_nodes.Count > 0) // create new room
            {
                int rand_index = Random.Range(0, possible_nodes.Count - 1);
                Vector2Int next = possible_nodes[rand_index];
                add_room(current.x, current.y, next.x, next.y, RoomType.simple, p_connect);
                current = next;
                max_length -= 1;
                has_generated_path = true;
            }
            else
            {
                break;
            }
        }

        if (has_generated_path && create_upgrade_room) //The last room will be an upgrade room
        {
            rooms[current.x, current.y] = RoomType.upgrade;
            var config_current = get_configuration(current.x, current.y);
            var random_index = Random.Range(0, 4);

            for (int i = 0; i < 4; ++i)
            {
                random_index = random_index % 4;
                if(random_index == 0 && config_current.north)
                {
                    close_all_doors(current.x, current.y);
                    add_door(current.x, current.y, Direction.north);
                    break;
                }
                if(random_index == 1 && config_current.west)
                {
                    close_all_doors(current.x, current.y);
                    add_door(current.x, current.y, Direction.west);
                    break;
                }
                if(random_index == 2 && config_current.east)
                {
                    close_all_doors(current.x, current.y);
                    add_door(current.x, current.y, Direction.east);
                    break;
                }
                if(random_index == 3 && config_current.south)
                {
                    close_all_doors(current.x, current.y);
                    add_door(current.x, current.y, Direction.south);
                    break;
                }
                random_index++;
            }
        }


        return has_generated_path;
    }
    private void generate_upgrade_room(int total_upgrade_room, List<Vector2Int> main_nodes)
    {
        List<Vector2Int> side_path_generation_order = new List<Vector2Int>();

        foreach (var node in main_nodes)
        {
            side_path_generation_order.Add(node);
            side_path_generation_order.Add(node);
        }

        //Shuffle order of generation
        for (int i = 0; i < side_path_generation_order.Count; ++i)
        {
            Vector2Int temp = side_path_generation_order[i];
            var random_index = Random.Range(i, side_path_generation_order.Count);
            side_path_generation_order[i] = side_path_generation_order[random_index];
            side_path_generation_order[random_index] = temp;
        }

        int total_upgrade_room_created = 0;
        foreach(Vector2Int node in side_path_generation_order)
        {
            var result = generate_side_path(node, Mathf.Min(width, length), create_upgrade_room: true);
            total_upgrade_room_created = result ? total_upgrade_room_created + 1 : total_upgrade_room_created;
            if(total_upgrade_room_created == total_upgrade_room)
            {
                break;
            }
        }
    }

    private void generate_fight_room(int total_fight_room)
    {
        List<(int x, int y)> fight_room_generation_order = new List<(int x, int y)>();
        for (int i = 0; i < rooms.GetLength(0); ++i)
        {
            for(int j = 0; j < rooms.GetLength(1); ++j)
            {
                fight_room_generation_order.Add((i, j));
            }
        }

        //Shuffle order of generation
        for (int i = 0; i < fight_room_generation_order.Count; ++i)
        {
            var temp = fight_room_generation_order[i];
            var random_index = Random.Range(i, fight_room_generation_order.Count);
            fight_room_generation_order[i] = fight_room_generation_order[random_index];
            fight_room_generation_order[random_index] = temp;
        }

        int total_fight_room_created = 0;
        foreach(var node in fight_room_generation_order)
        {
            var type = rooms[node.x, node.y];
            if (type == RoomType.simple)
            {
                rooms[node.x, node.y] = RoomType.fight;
                total_fight_room_created++;

                if(total_fight_room_created == total_fight_room)
                {
                    break;
                }
            }
        }
    }


    public Vector2Int generate(int total_upgrade_room, int total_fight_room)
    {
        Vector2Int origin = new Vector2Int(Random.Range(0, width - 1), 0);
        Vector2Int end = new Vector2Int(Random.Range(0, width - 1), length-1);

        List<Vector2Int> main_nodes = generate_main_path(origin, end);
        int min_length = main_nodes.Count + 1;
        int max_length = Mathf.Min(width, length);

        generate_upgrade_room(total_upgrade_room, main_nodes);

        foreach (Vector2Int node in main_nodes)
        {
            // generate side paths in the at most two remaining transitions
            generate_side_path(node, Random.Range(min_length, Mathf.Min(min_length, max_length)));
            generate_side_path(node, Random.Range(min_length, Mathf.Min(min_length, max_length)));
            min_length -= 1;
        }

        generate_fight_room(total_fight_room);

        return origin;
    }

    public void construct_3D_map()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                RoomStructure room = Instantiate(room_prefab, transform).GetComponent<RoomStructure>();
                room_structs[x, y] = room;
                room.transform.position = new Vector3(y * RoomStructure.full_room_length, x * RoomStructure.full_room_width, 0);
                room.load_config(get_configuration(x, y));
            }
        }
    }
}
