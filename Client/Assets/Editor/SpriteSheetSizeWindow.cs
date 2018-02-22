using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

class SpriteSheetSizeWindow : EditorWindow
{
    protected int framePixelSize;
    protected int frameCount;

    protected string[] createNames;
    protected Action[] creators;
    protected int createIndex;

    void Awake()
    {
		createIndex = 0;

        framePixelSize = 64;
        frameCount = 32;

        createNames = new string[] {
            "Circle",
            "Cross"
        };

        creators = new Action[] {
            CreateCircleFrames,
            CreateCrossFrames
        };
    }

    void OnGUI()
    {
        framePixelSize = EditorGUILayout.IntField("frame size", framePixelSize);
        frameCount = EditorGUILayout.IntField("frame count", frameCount);

        createIndex = EditorGUILayout.Popup(createIndex, createNames);

        if (GUILayout.Button("Create Sprite Sheet"))
        {
            creators[createIndex]();
			Close();
        }
    }

    void CreateCircleFrames()
    {
        SymbolMaskAnimation.Circle circle = new SymbolMaskAnimation.Circle(0.3f, 0.4f);
        SymbolMaskAnimation.Paint(framePixelSize, frameCount, circle.Draw, "circleTex");
    }

    void CreateCrossFrames()
    {
        SymbolMaskAnimation.Cross cross = new SymbolMaskAnimation.Cross(0.2f, 0.1f, 0.9f);
        SymbolMaskAnimation.Paint(framePixelSize, frameCount, cross.Draw, "crossTex");
    }
}