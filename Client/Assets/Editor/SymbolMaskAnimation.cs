using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class SymbolMaskAnimation
{
    public class Circle
    {
        private static Vector2 Center = new Vector2(0.5f, 0.5f);
        private float minDistSquared;
        private float maxDistSquared;

        public Circle(float minDist, float maxDist)
        {
            this.minDistSquared = minDist * minDist;
            this.maxDistSquared = maxDist * maxDist;
        }

        public bool Draw(Vector2 position, float progress)
        {
            Vector2 toCenter = position - Center;
            float centerDistanceSquared = toCenter.sqrMagnitude;

            float angleUnit = (Vector2.SignedAngle(Vector2.right, toCenter.normalized) % 360) / 360.0f;

            if (angleUnit < 0.0f)
                angleUnit = 0.5f - angleUnit;
            
            return  centerDistanceSquared > minDistSquared &&
                    centerDistanceSquared < maxDistSquared &&
                    progress > angleUnit;
        }
    }

    public class Cross
    {
        private float halfThickness;
        private float minY, maxY;
        private float length;

        public Cross(float thickness, float minY, float maxY)
        {
            this.halfThickness = thickness * 0.5f;
            this.minY = minY;
            this.maxY = maxY;
            this.length = new Vector2(maxY - minY, maxY - minY).magnitude;
        }

        public bool Draw(Vector2 position, float progress)
        {
            // positive pendiente
            if (position.y < minY || position.y > maxY)
                return false;


            return  DrawLine(position, new Vector2(minY, minY), position.y, Mathf.Min(progress / 0.5f, 1.0f)) ||
                    DrawLine(position, new Vector2(minY, maxY), 1.0f - position.y, Mathf.Max((progress - 0.5f) / 0.5f, 0.0f));
        }

        private bool DrawLine(Vector2 position, Vector2 startPosition, float lineX, float progress)
        {
            if (Mathf.Abs(position.x - lineX) > halfThickness)
                return false;
            
            float pointLength = new Vector2(lineX - startPosition.x, position.y - startPosition.y).magnitude;
            float necessaryProgress = pointLength / length;

            return progress > necessaryProgress;
        }
    }


    public delegate bool MaskFramePainter(Vector2 xy, float progress);


    [MenuItem("Textures/Guachamba/Sprite Sheet")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SpriteSheetSizeWindow window = (SpriteSheetSizeWindow)EditorWindow.GetWindow(typeof(SpriteSheetSizeWindow));
        window.Show();
    }

    private static void DrawFrame(Texture2D tex, Vector2Int frameStart, int framePixelSize, MaskFramePainter maskPainter, float progress)
    {
        float onePixelRelativeToFrameSize = 1.0f / framePixelSize;

        for (int y = 0; y != framePixelSize; ++y)
        {
            for (int x = 0; x != framePixelSize; ++x)
            {
                Color color = maskPainter(
                    new Vector2(x * onePixelRelativeToFrameSize, y * onePixelRelativeToFrameSize),
                    progress
                ) ? Color.white : Color.black;

                tex.SetPixel(frameStart.x + x, frameStart.y + y, color);
            }
        }
    }

    private static void DrawSpriteSheet(
        Texture2D tex,
        int frameCount, int framePixelSize, int frameCols,
        MaskFramePainter maskPainter)
    {
        float frameProgress = 1.0f / (float)frameCount;

        for (int i = 0; i != frameCount; ++i)
        {
            Vector2Int frameStart = new Vector2Int(
                framePixelSize * (i % frameCols),
                framePixelSize * (i / frameCols)
            );

            float currentProgress = (float)i * frameProgress;

            DrawFrame(tex, frameStart, framePixelSize, maskPainter, currentProgress);
        }
		
        tex.Apply();
    }

    public static void Paint(int framePixelSize, int frameCount, MaskFramePainter maskPainter, string fileName) {
        int imageCols = 8;
        int imageRows = frameCount / imageCols;

        Texture2D tex = new Texture2D(imageCols * framePixelSize, imageRows * framePixelSize, TextureFormat.RGB24, false);

        DrawSpriteSheet(tex, frameCount, framePixelSize, imageCols, maskPainter);
        SaveTexture(tex, fileName);
    }

    private static void SaveTexture(Texture2D tex, string filename)
    {
        byte[] pngData = tex.EncodeToPNG();

        if (pngData == null)
        {
            Debug.Log("no image data");
        }
        else
        {
            File.WriteAllBytes(string.Format("Assets/Textures/{0}.png", filename), pngData);
        }

        AssetDatabase.Refresh();
    }
}
