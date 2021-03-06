﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServicePanel : MonoBehaviour
{
    public SelectionPanel SelectionPanel;

    public Service Service { get; protected set; }

    public ProgressBar ProgressBar;

    List<GameObject> icons;

    public ResourceIconSlot InputSlot;

    List<int> tempRequiredResources;
    List<int> tempResources;

    public Text TextSubpanel;

    bool firstShow;

    void Awake()
    {
        icons = new List<GameObject>();
        tempRequiredResources = new List<int>();
        tempResources = new List<int>();
    }

    void Update()
    {
        if (Service == null || Service.Building.IsDeconstructed)
        {
            Service = null;
            SelectionPanel.RemoveSelection();
            return;
        }

        ProgressBar.SetFillPercentage(Service.GetServicePercentage());

        if (firstShow || Service.InputStorage.Changed)
        {
            if (firstShow) firstShow = false;
            if (Service.InputStorage.Changed) Service.InputStorage.Changed = false;

            SelectionPanel.HideResourceIcons(icons, this.transform);

            if (Service.Prototype.ConsumedResources != null)
            {
                tempRequiredResources = SelectionPanel.GetResourcesList(Service.Prototype.ConsumedResources);
                tempResources = SelectionPanel.GetResourcesList(Service.InputStorage.Resources);
                foreach (Character c in Service.InputStorage.ReservedResources.Keys)
                {
                    tempResources.Add(Service.InputStorage.ReservedResources[c]);
                }

                SelectionPanel.ShowIconsWithRequirements(new List<ResourceIconSlot>() { InputSlot },
                                                         tempRequiredResources, tempResources, icons);
            }
        }
    }

    public void SetService(Service s)
    {
        Service = s;
        if (s != null)
        {
            firstShow = true;
            if (s.Building != null) TextSubpanel.text = s.Building.Name;
            Update();
        }
    }
}
