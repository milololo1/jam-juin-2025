using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMap : MonoBehaviour
{
    public Level level;
    public GameObject room_prefab;

    private UIRoom[,] rooms;
    private int size_room = 50;

    private void initialize()
    {
        // for now
        level.initialize();
        level.generate();

        GetComponent<RectTransform>().sizeDelta = new Vector2(level.length*size_room, level.width*size_room);

        rooms = new UIRoom[level.width, level.length];
        for (int x  = 0; x < level.width; x++)
        {
            for (int y = 0; y < level.length; y++)
            {
                UIRoom room = Instantiate(room_prefab, transform).GetComponent<UIRoom>();
                room.name = "(" + x + "," + y + ")";
                rooms[x, y] = room;
                room.GetComponent<RectTransform>().anchoredPosition = new Vector2(y * size_room, x * size_room);
            }
        }
    }

    public void display()
    {
        for (int x = 0; x < level.width; x++)
        {
            for (int y = 0; y < level.length; y++)
            {
                rooms[x, y].display(level.get_configuration(x, y));
            }
        }
    }

    public void hide()
    {
        foreach (UIRoom room in rooms) room.hide();
    }

    void Start()
    {
        initialize();
        display();

        Debug.Log(level.get_configuration(0, 0).north);
    }
}
