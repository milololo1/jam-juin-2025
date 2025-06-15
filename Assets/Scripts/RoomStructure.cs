using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RoomStructure : MonoBehaviour
{
    public GameObject pillar, wall_v, wall_h;
    public GameObject left, right, top, bottom;
    public Transform structure_transform;

    private float half_width = 8;
    private float half_length = 13;
    private float cell_size = 4;
    private float dx, dy;

    private int grid_width, grid_length;
    private int grid_width_center, grid_length_center;
    private bool[,] floor_plan;

    private RoomConfiguration config;

    public static float full_room_width = 10 * 2;
    public static float full_room_length = 15 * 2;

    private void initialize()
    {
        grid_width = 2 * Mathf.FloorToInt((half_width - cell_size * 0.5f) / cell_size) + 1;
        grid_length = 2 * Mathf.FloorToInt((half_length - cell_size * 0.5f) / cell_size) + 1;
        grid_width_center = Mathf.FloorToInt((half_width - cell_size * 0.5f) / cell_size);
        grid_length_center = Mathf.FloorToInt((half_length - cell_size * 0.5f) / cell_size);
        dx = (half_width - grid_width_center * cell_size - cell_size * 0.5f) / grid_width_center;
        dy = (half_length - grid_length_center * cell_size - cell_size * 0.5f) / grid_length_center;

        floor_plan = new bool[grid_width, grid_length];
    }
    public void load_config(RoomConfiguration config)
    {
        initialize();

        this.config = config;

        switch (config.type)
        {
            case RoomType.none:
                gameObject.SetActive(false);
                return;
            default:
            case RoomType.fight:
            case RoomType.simple:
                generate(Random.Range(0, 4 + 1), Random.Range(0, 1 + 1));
                break;
            case RoomType.start:
                generate(Random.Range(0, 2 + 1), 0, true);
                break;
            case RoomType.boss:
                generate(Random.Range(0, 3 + 1), 0);
                break;
            case RoomType.upgrade:
                generate(0, 0);
                break;
        }

        left.SetActive(!config.west);
        right.SetActive(!config.east);
        bottom.SetActive(!config.south);
        top.SetActive(!config.north);

        gameObject.SetActive(true);
    }

    public void open_doors()
    {
        left.SetActive(!config.west);
        right.SetActive(!config.east);
        bottom.SetActive(!config.south);
        top.SetActive(!config.north);
    }

    public void close_doors()
    {
        left.SetActive(true);
        right.SetActive(true);
        bottom.SetActive(true);
        top.SetActive(true);
    }

    public void generate(int n_pillars, int n_walls, bool leave_center=false)
    {
        for (int w = 0; w < n_walls; w++)
        {
            if (Random.value < 0.5f) add_vertical_walls(1);
            else add_horizontal_walls(1);
        }

        add_pillars(n_pillars, leave_center);
    }

    private void add_pillars(int n, bool leave_center=false)
    {
        List<Vector2Int> samples = sample_distinct_location(n, leave_center);
        foreach (Vector2Int v in samples)
        {
            floor_plan[v.x, v.y] = true;
            Instantiate(pillar, structure_transform).transform.localPosition = getLocation(v.x, v.y);
        }
    }

    private void add_vertical_walls(int n)
    {
        List<Vector2Int> samples = sample_distinct_vertical_location(n);
        foreach (Vector2Int v in samples)
        {
            floor_plan[v.x, v.y] = true;
            floor_plan[v.x + 1, v.y] = true;
            floor_plan[v.x - 1, v.y] = true;
            Instantiate(wall_v, structure_transform).transform.localPosition = getLocation(v.x, v.y);
        }
    }

    private void add_horizontal_walls(int n)
    {
        List<Vector2Int> samples = sample_distinct_horizontal_location(n);
        foreach (Vector2Int v in samples)
        {
            floor_plan[v.x, v.y] = true;
            floor_plan[v.x, v.y + 1] = true;
            floor_plan[v.x, v.y - 1] = true;
            Instantiate(wall_h, structure_transform).transform.localPosition = getLocation(v.x, v.y);
        }
    }

    private Vector3 getLocation(int x, int y)
    {
        return new Vector3((y - grid_length_center) * (cell_size + dy), (x - grid_width_center) * (cell_size + dx), 0);
    }

    private bool isInFrontDoor(int x, int y)
    {
        bool x_at_border = x == grid_width - 1 || x == 0;
        bool y_at_border = y == grid_length - 1 || y == 0;
        bool x_at_center = x == grid_width_center;
        bool y_at_center = y == grid_length_center;

        return ((x_at_border && y_at_center) || (x_at_center && y_at_border));
    }

    private bool isCenter(int x, int y, bool leave_center = true)
    {
        if (!leave_center) return false;
        else return x == grid_width_center && y == grid_length_center;
    }

    private List<Vector2Int> sample_distinct_location(int n, bool leave_center = false)
    {
        List<Vector2Int> samples = new List<Vector2Int>();
        for (int x  = 0; x < grid_width; x++)
        {
            for (int y = 0; y < grid_length; y++)
            {
                if (!isCenter(x, y, leave_center) && !floor_plan[x, y] && !isInFrontDoor(x, y)) samples.Add(new Vector2Int(x, y));
            }
        }

        // shuffle
        for (int k = 0; k < samples.Count; k++)
        {
            Vector2Int temp = samples[k];
            int random_index = Random.Range(k, samples.Count);
            samples[k] = samples[random_index];
            samples[random_index] = temp;
        }

        return samples.GetRange(0, Mathf.Min(n, samples.Count));
    }

    private List<Vector2Int> sample_distinct_vertical_location(int n, int l=3)
    {
        List<Vector2Int> samples = new List<Vector2Int>();

        int half_l = Mathf.FloorToInt((l - 1) / 2);
        for (int x = half_l; x < grid_width - half_l; x++)
        {
            for (int y = 0; y < grid_length; y++)
            {
                bool floor_plan_free = !floor_plan[x, y];
                if (floor_plan_free)
                {
                    for (int k = 0; k < half_l; k++)
                    {
                        if (floor_plan[x + (k + 1), y] || floor_plan[x - (k + 1), y])
                        {
                            floor_plan_free = false;
                            break;
                        }
                    }
                }

                bool not_in_front_door = !isInFrontDoor(x, y);
                if (not_in_front_door)
                {
                    for (int k = 0; k < half_l; k++)
                    {
                        if (isInFrontDoor(x + (k + 1), y) || isInFrontDoor(x - (k + 1), y))
                        {
                            not_in_front_door = false;
                            break;
                        }
                    }
                }

                if (floor_plan_free && not_in_front_door) samples.Add(new Vector2Int(x, y));
            }
        }

        // shuffle
        for (int k = 0; k < samples.Count; k++)
        {
            Vector2Int temp = samples[k];
            int random_index = Random.Range(k, samples.Count);
            samples[k] = samples[random_index];
            samples[random_index] = temp;
        }

        return samples.GetRange(0, Mathf.Min(n, samples.Count));
    }

    private List<Vector2Int> sample_distinct_horizontal_location(int n, int l = 3)
    {
        List<Vector2Int> samples = new List<Vector2Int>();

        int half_l = Mathf.FloorToInt((l - 1) / 2);
        for (int y = half_l; y < grid_length - half_l; y++)
        {
            for (int x = 0; x < grid_width; x++)
            {
                bool floor_plan_free = !floor_plan[x, y];
                if (floor_plan_free)
                {
                    for (int k = 0; k < half_l; k++)
                    {
                        if (floor_plan[x, y + (k + 1)] || floor_plan[x, y - (k + 1)])
                        {
                            floor_plan_free = false;
                            break;
                        }
                    }
                }

                bool not_in_front_door = !isInFrontDoor(x, y);
                if (not_in_front_door)
                {
                    for (int k = 0; k < half_l; k++)
                    {
                        if (isInFrontDoor(x, y + (k + 1)) || isInFrontDoor(x, y - (k + 1)))
                        {
                            not_in_front_door = false;
                            break;
                        }
                    }
                }

                if (floor_plan_free && not_in_front_door) samples.Add(new Vector2Int(x, y));
            }
        }

        // shuffle
        for (int k = 0; k < samples.Count; k++)
        {
            Vector2Int temp = samples[k];
            int random_index = Random.Range(k, samples.Count);
            samples[k] = samples[random_index];
            samples[random_index] = temp;
        }

        return samples.GetRange(0, Mathf.Min(n, samples.Count));
    }
}
