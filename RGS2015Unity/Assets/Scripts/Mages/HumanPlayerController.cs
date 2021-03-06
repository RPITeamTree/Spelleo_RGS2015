﻿using UnityEngine;
using System.Collections;

public class HumanPlayerController : PlayerController
{
    private int control_scheme = 1;
    //private GameManager match;


    public void Initialize(int control_scheme)
    {
        this.control_scheme = control_scheme;
        //this.match = match;
    }

    new private void Update()
    {
        // Movement (projectile control)
        float h = Input.GetAxis("H" + control_scheme);
        float v = Input.GetAxis("V" + control_scheme);
        InputMove = new Vector2(h, v);

        // Spell code
        if (ADown() && InputSpellCodeA != null) { InputSpellCodeA(); StartAutoCast(); }
        else if (BDown() && InputSpellCodeB != null) { InputSpellCodeB(); StartAutoCast(); }
        else if (XDown() && InputSpellCodeX != null) { InputSpellCodeX(); StartAutoCast(); }
        else if (YDown() && InputSpellCodeY != null) { InputSpellCodeY(); StartAutoCast(); }

        // Casting (explicit)
        if (Input.GetAxis("Cast" + control_scheme) > 0 && InputCast != null)
        {
            InputCast();
        }

        base.Update();
    }

    private bool ADown()
    {
        return control_scheme == 1 ? Input.GetKeyDown(KeyCode.Joystick1Button0) :
               control_scheme == 2 ? Input.GetKeyDown(KeyCode.Joystick2Button0) : 
               control_scheme == 10 ? Input.GetKeyDown(KeyCode.S) : false;
    }
    private bool BDown()
    {
        return control_scheme == 1 ? Input.GetKeyDown(KeyCode.Joystick1Button1) :
               control_scheme == 2 ? Input.GetKeyDown(KeyCode.Joystick2Button1) :
               control_scheme == 10 ? Input.GetKeyDown(KeyCode.D) : false;
    }
    private bool XDown()
    {
        return control_scheme == 1 ? Input.GetKeyDown(KeyCode.Joystick1Button2) :
               control_scheme == 2 ? Input.GetKeyDown(KeyCode.Joystick2Button2) :
               control_scheme == 10 ? Input.GetKeyDown(KeyCode.A) : false;
    }
    private bool YDown()
    {
        return control_scheme == 1 ? Input.GetKeyDown(KeyCode.Joystick1Button3) :
               control_scheme == 2 ? Input.GetKeyDown(KeyCode.Joystick2Button3) :
               control_scheme == 10 ? Input.GetKeyDown(KeyCode.W) : false;
    }

}
