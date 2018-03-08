using UnityEngine;
using UnityEditor;
using BeardedManStudios.Forge.Networking.UnityEditor;
using System;

[System.Serializable]
public class ForgeCompressionSettings
{
    public bool compress = false;
    public int min = 0;
    public int max = 255;
    public float accuracy = 0.01f;

    internal void Render(Rect changingRect, ForgeAcceptableFieldTypes fieldType)
    {
        throw new NotImplementedException();
    }
}