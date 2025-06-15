using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : MonoBehaviour
{
    public Image background;
    public GameObject left, right, bottom, top;

    public void display(RoomConfiguration config)
    {
        switch (config.type)
        {
            case RoomType.none:
                hide();
                return;
            case RoomType.simple:
                background.color = Color.white;
                break;
            case RoomType.upgrade:
                background.color = Color.blue;
                break;
            case RoomType.fight:
                background.color = Color.gray;
                break;
            case RoomType.boss:
                background.color = Color.red;
                break;
            case RoomType.start:
                background.color = Color.green;
                break;
        }

        left.SetActive(!config.west);
        right.SetActive(!config.east);
        bottom.SetActive(!config.south);
        top.SetActive(!config.north);

        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }

}
