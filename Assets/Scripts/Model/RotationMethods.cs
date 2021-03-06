﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Rotation
{
    N,
    E,
    S,
    W
}

public static class RotationMethods
{
    public static Vector3 ToEulerAngles(this Rotation rotation)
    {
        Vector3 result;
        if      (rotation == Rotation.N) { result = new Vector3(0, 180, 0); }
        else if (rotation == Rotation.E) { result = new Vector3(0, -90, 0); }
        else if (rotation == Rotation.S) { result = new Vector3(0, 0, 0); }
        else    { result = new Vector3(0, 90, 0); }
        return result;
    }

    public static Rotation Rotate(this Rotation oldRotation, Rotation rotationToApply)
    {
        Rotation result;
        if (rotationToApply == Rotation.N)
        {
            result = oldRotation;
        }
        else if (rotationToApply == Rotation.E)
        {
            if      (oldRotation == Rotation.N) { result = Rotation.E; }
            else if (oldRotation == Rotation.E) { result = Rotation.S; }
            else if (oldRotation == Rotation.S) { result = Rotation.W; }
            else    { result = Rotation.N; }
        }
        else if (rotationToApply == Rotation.S)
        {
            if      (oldRotation == Rotation.N) { result = Rotation.S; }
            else if (oldRotation == Rotation.E) { result = Rotation.W; }
            else if (oldRotation == Rotation.S) { result = Rotation.N; }
            else    { result = Rotation.E; }
        }
        else
        {
            if      (oldRotation == Rotation.N) { result = Rotation.W; }
            else if (oldRotation == Rotation.E) { result = Rotation.N; }
            else if (oldRotation == Rotation.S) { result = Rotation.E; }
            else    { result = Rotation.S; }
        }
        return result;
    }

    public static Rotation GetRotationTowardsPosition(TilePosition positionToBeRotated, TilePosition positionToRotateAt)
    {
        Rotation result;
        if (positionToBeRotated.Y > positionToRotateAt.Y)
        {
            if      (positionToBeRotated.X < positionToRotateAt.X) { result = Rotation.E; }
            else if (positionToBeRotated.X > positionToRotateAt.X) { result = Rotation.W; }
            else    { result = Rotation.S; }
        }
        else
        {
            if      (positionToBeRotated.X < positionToRotateAt.X) { result = Rotation.E; }
            else if (positionToBeRotated.X > positionToRotateAt.X) { result = Rotation.W; }
            else    { result = Rotation.N; }
        }
        return result;
    }

    public static Rotation GetRandomRotation()
    {
        Rotation result = Rotation.N;
        int numberOfRotations = UnityEngine.Random.Range(0, 3);
        for (int i = 0; i <= numberOfRotations; i++)
        {
            result = result.Rotate(Rotation.E);
        }
        return result;
    }
}
