﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DEBUG_InfoPanel : MonoBehaviour 
{
    Text text;
    InputManager mouseManager;

    void Start () 
    {
        text = this.GetComponentInChildren<Text>();
        mouseManager = FindObjectOfType<InputManager>();        
    }

    void Update()
    {
        string s = "";

        Tile t = mouseManager.CurrentTile;

        s += "ZAZNACZENIE: \n";
        if(mouseManager.SelectedObject != null)
        {
            s += mouseManager.SelectedObject.DEBUG_GetSelectionText() + "\n \n";
        }
        else
        {
            s += "Brak \n \n";
        }

        s += "KURSOR: \n";
        if (t == null)
        {
            s += "Poza mapą \n";
            text.text = s;
        }
        else
        {
            s += "Współrzędne: " + t.Position.ToString() + "\n";

            s += "Typ: " + t.Type.ToString() + "\n";

            s += "Koszt ruchu: " + t.MovementCost.ToString() + "\n";

            text.text = s;
        }
    }
}
