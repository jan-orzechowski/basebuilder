﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_HasReservation : BT_Node
{
    public override bool Activates { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation == null)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
