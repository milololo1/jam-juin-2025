using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public UIMap _ui_map;

    public static UIMap ui_map;

    public void initialize()
    {
        ui_map = _ui_map;
        ui_map.initialize();
    }
}
