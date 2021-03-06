﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_TakeResource : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null 
            && am.Character.HasResource == false
            && am.Character.Reservation.SourceStorage != null
            && (am.Character.Reservation.SourceStorage.GetAccessTile(false) == am.Character.CurrentTile 
                || am.Character.Reservation.SourceStorage.GetAccessTile(true) == am.Character.CurrentTile))
        {
            if (am.Character.Reservation.SourceStorage.TransferFromStorage(am.Character.Reservation.Resource, am.Character))
            {
                return BT_Result.SUCCESS;
            }
            else
            {
                // Nie mieliśmy rezerwacji - coś jest bardzo nie tak

            }
        }
        return BT_Result.FAILURE;
    }
}
