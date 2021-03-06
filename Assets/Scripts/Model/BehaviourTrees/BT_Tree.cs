﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum BT_Result : int
{
    FAILURE,
    SUCCESS,
    RUNNING,
    ERROR
}

public class BT_Tree
{
    public BT_Node Root { get; protected set; }

    Dictionary<int, BT_Node> nodes = new Dictionary<int, BT_Node>();

    public BT_Result Tick(BT_AgentMemory am)
    {
        am.ResetActiveNodesList();
        am.ProcessTimers(am.DeltaTime);
        am.CurrentTree = this;

        BT_Result result = BT_Node.TickChild(Root, am);
        // Debug.Log(am.Character.Name + " - " + am.FinalNodeLastCall + " - " + GetNodeByID(am.FinalNodeLastCall).GetType());
        return result;
    }
     
    public BT_Node GetNodeByID(int id)
    {
        if (nodes.ContainsKey(id)) return nodes[id];
        else return null;
    }

    void AssignIDs()
    {
        int idCounter = 1;
        Root.AssignID(0, ref idCounter, nodes);
    }

    BT_CompositeNode Subtree(BT_CompositeNode parent, params BT_Node[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            parent.Add(nodes[i]);
        }

        return parent;
    }
   
    public void LoadHumanTree()
    {
        float waitBeforeEntering = 0.2f;

        Root =
        Subtree(new BT_Priority(),
                    new BT_IsUsingService(),
                    new BT_RemoveWorkplaceIfCannotReserve(),
                    new BT_GoToRandomTile(),
                    Subtree(new BT_Sequence(),
                        new BT_HasReservation(),
                        new BT_Inverter(new BT_AreReservedStoragesAccessible()),
                        new BT_RemoveReservation()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_HasResourceForReservation(),
                        new BT_GoToTargetStorage(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_DepositResource()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_GoToSourceStorage(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_TakeResource()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_HasResource(),
                        new BT_Inverter(new BT_HasReservation()),
                        new BT_FindNewStorageForResource()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasService(),
                        new BT_GoToService(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_StartUsingService()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Health", StaticData.NeedLevelToDie),
                        new BT_Die()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Health", StaticData.NeedLevelToSeekService),
                        new BT_FindService("Health")
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Hunger", StaticData.NeedLevelToDie),
                        new BT_Die()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Hunger", StaticData.NeedLevelToSeekService),
                        new BT_FindService("Hunger")
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasWorkplace(),
                        new BT_GoToWorkplace(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_Work()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_Inverter(new BT_HasWorkplace()),
                        new BT_FindTransportJob()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_Inverter(new BT_HasReservation()),
                        new BT_FindWorkplace()
                        ),                                      
                    new BT_SetRandomTile()
        );

        AssignIDs();
    }

    public void LoadRobotTree()
    {
        float waitBeforeEntering = 0.2f;

        Root =
        Subtree(new BT_Priority(),
                    new BT_IsUsingService(),
                    new BT_RemoveWorkplaceIfCannotReserve(),
                    new BT_GoToRandomTile(),
                    Subtree(new BT_Sequence(),
                        new BT_HasReservation(),
                        new BT_Inverter(new BT_AreReservedStoragesAccessible()),
                        new BT_RemoveReservation()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_HasResourceForReservation(),
                        new BT_GoToTargetStorage(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_DepositResource()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_GoToSourceStorage(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_TakeResource()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_HasResource(),
                        new BT_Inverter(new BT_HasReservation()),
                        new BT_FindNewStorageForResource()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasService(),
                        new BT_GoToService(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_StartUsingService()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Condition", StaticData.NeedLevelToDie),
                        new BT_Die()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_IsNeedHigherThan("Condition", StaticData.NeedLevelToSeekService),
                        new BT_FindService("Condition")
                        ),                 
                    Subtree(new BT_MemSequence(),
                        new BT_HasWorkplace(),
                        new BT_GoToWorkplace(),
                        new BT_Wait(waitBeforeEntering),
                        new BT_Work()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_Inverter(new BT_HasWorkplace()),
                        new BT_FindTransportJob()
                        ),
                    Subtree(new BT_Sequence(),
                        new BT_Inverter(new BT_HasReservation()),
                        new BT_FindWorkplace()
                        ),
                    new BT_SetRandomTile()
        );

        AssignIDs();
    }
}
