using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class PrintResultPoints : MonoBehaviour
{
    [Tooltip("Aspect ratio of width to height of QR code")]
    public float qrScale = 1.0f;
    [Tooltip("Width of texture compare to a width of the QR")]
    public float uvWidth = 1.0f;
    [Tooltip("Height of texture compare to a height of the QR")]
    public float uvHeight = 1.0f;
    [Tooltip("Offset X of texture from the left side QR. Unit in Width of the QR")]
    public float uvX = 0.5f;
    [Tooltip("Offset Y of texture from the top side QR. Unit in Height of the QR")]
    public float uvY = 0.5f;

    public RawImage previewImage;

    public void PrintResult(Texture2D texture, ResultPoint[] resultPoints, Vector2 qrOffset, int orientation)
    {
        Vector2[] points = new Vector2[resultPoints.Length];

        // Print corner points of QR from squared webcam image
        for (int i = 0; i < resultPoints.Length; i++)
        {
            Debug.Log(i + " : " + resultPoints[i]);
            ResultPoint point = resultPoints[i];
            points[i] = new Vector2(point.X + qrOffset.x, point.Y + qrOffset.y);
        }

        Vector2[] corners = GetQRCorners(texture.width, texture.height, points);
        Vector2 topLeft = corners[0];
        Vector2 topRight = corners[1];
        Vector2 bottomLeft = corners[2];
        Vector2 bottomRight = GetLastCorners(corners);

        Debug.Log(topLeft + " , " + topRight + " , " + bottomLeft + " , " + bottomRight);

        // Plot the texture offset
        int qrSize = (int)Mathf.Max(topRight.x - topLeft.x, bottomLeft.y - topLeft.y);
        int qrWidth = (int)(qrSize * qrScale);
        int qrHeight = (int)(qrSize / qrScale);

        Debug.Log("QR Size: " + qrSize + " Width: " + qrWidth + "Height: " + qrHeight);

        int textureWidth = (int)(uvWidth * qrWidth);
        int textureHeight = (int)(uvHeight * qrHeight);
        int posX = (int)Mathf.Clamp(topLeft.x + (qrWidth * uvX), 0, texture.width);
        int posY = texture.height - (int)Mathf.Clamp(topLeft.y + (qrHeight * uvY), 0, texture.height) - textureHeight;

        Debug.Log("W:" + textureWidth + " H:" + textureHeight + " x:" + posX + " y:" + posY);

        Texture2D croppedTexture = CropTexture(texture, posX, posY, textureWidth, textureHeight);

        // Debug, Replace color on cropped area
        // FillRectangle(texture, posX, posY, textureWidth, textureHeight, Color.magenta);

        previewImage.texture = croppedTexture;
    }

    public Vector2 GetLastCorners(Vector2[] points)
    {
        Vector2 a = points[0];
        Vector2 b = points[1];
        Vector2 c = points[2];

        return c + (b - a);
    }

    public Vector2[] GetQRCorners(float width, float height, Vector2[] points)
    {
        Vector2[] result = new Vector2[3];
        Vector2 topLeft = points[0];
        Vector2 topRight = points[0];
        Vector2 bottomLeft = points[0];

        Vector2 textureTopLeft = Vector2.zero;
        Vector2 textureTopRight = new Vector2(width, 0);
        Vector2 textureBottomLeft = new Vector2(0, height);

        float topLeftDistance = Mathf.Infinity;
        float topRightDistance = Mathf.Infinity;
        float bottomLeftDistance = Mathf.Infinity;

        foreach (Vector2 point in points)
        {
            // Find top-left (smallest x, smallest y)
            if (Vector2.Distance(textureTopLeft, point) < topLeftDistance)
            {
                topLeft = point;
                topLeftDistance = Vector2.Distance(textureTopLeft, point);
            }
            // Find top-right (largest x, smallest y)
            if (Vector2.Distance(textureTopRight, point) < topRightDistance)
            {
                topRight = point;
                topRightDistance = Vector2.Distance(textureTopRight, point);
            }
            // Find bottom-left (smallest x, largest y)
            if (Vector2.Distance(textureBottomLeft, point) < bottomLeftDistance)
            {
                bottomLeft = point;
                bottomLeftDistance = Vector2.Distance(textureBottomLeft, point);
            }
        }

        result[0] = topLeft;
        result[1] = topRight;
        result[2] = bottomLeft;
        return result;
    }

    public Texture2D CropTexture(Texture2D source, int x, int y, int width, int height)
    {
        // Clamp to stay within bounds
        x = Mathf.Clamp(x, 0, source.width - 1);
        y = Mathf.Clamp(y, 0, source.height - 1);
        width = Mathf.Clamp(width, 1, source.width - x);
        height = Mathf.Clamp(height, 1, source.height - y);

        // Get pixel data
        Color[] pixels = source.GetPixels(x, y, width, height);

        // Create cropped texture
        Texture2D cropped = new Texture2D(width, height);
        cropped.SetPixels(pixels);
        cropped.Apply();
        return cropped;
    }

    public static Texture2D RotateTexture(Texture2D original, int angle)
    {
        int width = original.width;
        int height = original.height;

        Color32[] originalPixels = original.GetPixels32();
        Color32[] rotatedPixels;

        int newWidth = width;
        int newHeight = height;

        if (angle > 45 && angle < 135)
        {
            newWidth = height;
            newHeight = width;
            rotatedPixels = new Color32[originalPixels.Length];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int originalIndex = x + y * width;
                    int rotatedX = height - y - 1;
                    int rotatedY = x;
                    int rotatedIndex = rotatedX + rotatedY * newWidth;
                    rotatedPixels[rotatedIndex] = originalPixels[originalIndex];
                }
            }
        }
        else if (angle >= 135 && angle < 225)
        {
            rotatedPixels = new Color32[originalPixels.Length];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int originalIndex = x + y * width;
                    int rotatedX = width - x - 1;
                    int rotatedY = height - y - 1;
                    int rotatedIndex = rotatedX + rotatedY * width;
                    rotatedPixels[rotatedIndex] = originalPixels[originalIndex];
                }
            }
        }
        else if (angle >= 255 && angle < 315)
        {
            newWidth = height;
            newHeight = width;
            rotatedPixels = new Color32[originalPixels.Length];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int originalIndex = x + y * width;
                    int rotatedX = y;
                    int rotatedY = width - x - 1;
                    int rotatedIndex = rotatedX + rotatedY * newWidth;
                    rotatedPixels[rotatedIndex] = originalPixels[originalIndex];
                }
            }
        }
        else
        {
            Debug.LogError("Unsupported rotation angle");
            return original;
        }

        Texture2D rotatedTexture = new Texture2D(newWidth, newHeight);
        rotatedTexture.SetPixels32(rotatedPixels);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public static void FillRectangle(Texture2D texture, int startX, int startY, int width, int height, Color color)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
        texture.Apply();
    }

}
