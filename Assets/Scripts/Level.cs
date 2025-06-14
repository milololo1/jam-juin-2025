using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    none,
    simple,
    upgrade,
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
}

public class Level : MonoBehaviour
{
    public int width, length;

    public RoomType[,] rooms;
    public bool[,] vertical_doors;
    public bool[,] horizontal_doors;

    public void initialize()
    {
        rooms = new RoomType[width, length];
        vertical_doors = new bool[width - 1, length];
        horizontal_doors = new bool[width, length - 1];
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

        return config;
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
                if (next_type != RoomType.none && next_type != RoomType.boss)
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
                if (next_type != RoomType.none && next_type != RoomType.boss)
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
                for (int dx = -1; dx < 2; dx += 2)
                {
                    if (current.x + dx > -1 && current.x + dx < width)
                    {
                        if (rooms[current.x + dx, current.y] == RoomType.none) possible_nodes.Add(new Vector2Int(current.x + dx, current.y));
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
            }
            else add_room(current.x, current.y, next.x, next.y, RoomType.boss, 0);
            current = next;
        }
        return created_rooms;
    }

    private void generate_side_path(Vector2Int start, int max_length = 5, float p_connect = 0.05f)
    {
        Vector2Int current = start;

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
            }
            else break;
        }
    }

    public void generate()
    {
        Vector2Int origin = new Vector2Int(Random.Range(0, width - 1), 0);
        Vector2Int end = new Vector2Int(Random.Range(0, width - 1), length-1);

        List<Vector2Int> main_nodes = generate_main_path(origin, end);
        int min_length = main_nodes.Count + 1;
        int max_length = Mathf.Min(width, length);
        foreach (Vector2Int node in main_nodes)
        {
            // generate side paths in the at most two remaining transitions
            generate_side_path(node, Random.Range(min_length, Mathf.Min(min_length, max_length)));
            generate_side_path(node, Random.Range(min_length, Mathf.Min(min_length, max_length)));
            min_length -= 1;
        }
    }
}
