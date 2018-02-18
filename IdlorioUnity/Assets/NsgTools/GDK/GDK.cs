using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDK
{
    private static GDK _instance;
    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = new GDK();
        }
    }
    public static GDK GetInstance()
    {
        return _instance;
    }

    private GDK() { }

    private static Stack<GDKShape> _pendingShapeRequests = new Stack<GDKShape>();
    private static Stack<GDKText> _pendingTextRequests = new Stack<GDKText>();
    private static Material _depthBufferMaterial = new Material(Shader.Find("Unlit/Transparent Colored/ZTest"));
    private static Material _noDepthBufferMaterial = new Material(Shader.Find("Unlit/Transparent Colored/NoZTest"));
    private static MaterialPropertyBlock _materialProperty = new MaterialPropertyBlock();

    public static Color FadeColor(Color color, float fade)
    {
        return new Color(color.r, color.g, color.b, color.a * fade);
    }

    private static Color GetAutoFillColor(Color color)
    {
        return new Color(color.r, color.g, color.b, color.a < 0.99f ? color.a : 0.5f);
    }

    private static Color GetAutoWireColor(Color color)
    {
        return new Color(color.r, color.g, color.b, 1f);
    }

    public void Update()
    {
        _pendingTextRequests.Clear();
    }

    public void DrawObjects()
    {
        foreach(GDKShape shape in _pendingShapeRequests)
        {
            Mesh mesh = shape.GetMesh();
            DrawMesh(mesh, shape.GetColor(), shape.GetUseDepthBuffer());
            GameObject.Destroy(mesh);

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    GDK.DrawText(i.ToString(), vertices[i], Color.black);
            //}
        }

        _pendingShapeRequests.Clear();
    }

    public void OnGUI()
    {
        Color previousColor = GUI.color;

        foreach (GDKText text in _pendingTextRequests)
        {
            Rect textPos = new Rect(text.GetScreenPos().x + 1, Screen.height - text.GetScreenPos().y + 1, Screen.width, Screen.height);

            if (text.GetDrawShadow())
            {
                GUI.color = Color.black;
                GUI.Label(textPos, text.GetText());
            }

            textPos.x -= 1;
            textPos.y -= 1;
            GUI.color = text.GetColor();
            GUI.Label(textPos, text.GetText());
        }

        GUI.color = previousColor;
    }

    public static void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKTriangle(p1, p2, p3, color, useDepthBuffer));
    }

    public static void DrawWireTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireTriangle(p1, p2, p3, color, useDepthBuffer));
    }

    public static void DrawFilledTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKTriangle(p1, p2, p3, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireTriangle(p1, p2, p3, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKQuad(p1, p2, p3, p4, color, useDepthBuffer));
    }

    public static void DrawWireQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireQuad(p1, p2, p3, p4, color, useDepthBuffer));
    }

    public static void DrawFilledQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKQuad(p1, p2, p3, p4, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireQuad(p1, p2, p3, p4, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawCircle(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        DrawCircle(center, Vector3.up, radius, color, useDepthBuffer);
    }
    
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCircle(center, normal, radius, color, useDepthBuffer));
    }

    public static void DrawWireCircle(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        DrawWireCircle(center, Vector3.up, radius, color, useDepthBuffer);
    }

    public static void DrawWireCircle(Vector3 center, Vector3 normal, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireCircle(center, normal, radius, color, useDepthBuffer));
    }

    public static void DrawFilledCircle(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        DrawFilledCircle(center, Vector3.up, radius, color, useDepthBuffer);
    }

    public static void DrawFilledCircle(Vector3 center, Vector3 normal, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCircle(center, normal, radius, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireCircle(center, normal, radius, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawArc(Vector3 center, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawArc(center, Vector3.up, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawArc(center, normal, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawArc(Vector3 center, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        DrawArc(center, Vector3.up, dir, radius, angleDeg, sides, color, useDepthBuffer);
    }

    public static void DrawArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKArc(center, normal, dir, radius, angleDeg, sides, color, useDepthBuffer));
    }

    public static void DrawWireArc(Vector3 center, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawWireArc(center, Vector3.up, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawWireArc(center, normal, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawWireArc(Vector3 center, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        DrawWireArc(center, Vector3.up, dir, radius, angleDeg, sides, color, useDepthBuffer);
    }

    public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireArc(center, normal, dir, radius, angleDeg, sides, color, useDepthBuffer));
    }

    public static void DrawFilledArc(Vector3 center, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawFilledArc(center, Vector3.up, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawFilledArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, Color color, bool useDepthBuffer = true)
    {
        DrawFilledArc(center, normal, dir, radius, angleDeg, Mathf.Max(3, (int)(angleDeg / 15)), color, useDepthBuffer);
    }

    public static void DrawFilledArc(Vector3 center, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        DrawFilledArc(center, Vector3.up, dir, radius, angleDeg, sides, color, useDepthBuffer);
    }

    public static void DrawFilledArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKArc(center, normal, dir, radius, angleDeg, sides, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireArc(center, normal, dir, radius, angleDeg, sides, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawCylinder(Vector3 center, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCylinder(center, Vector3.up, radius, length, color, useDepthBuffer));
    }

    public static void DrawCylinder(Vector3 center, Vector3 normal, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCylinder(center, normal, radius, length, color, useDepthBuffer));
    }

    public static void DrawWireCylinder(Vector3 center, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireCylinder(center, Vector3.up, radius, length, color, useDepthBuffer));
    }

    public static void DrawWireCylinder(Vector3 center, Vector3 normal, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireCylinder(center, normal, radius, length, color, useDepthBuffer));
    }

    public static void DrawFilledCylinder(Vector3 center, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCylinder(center, Vector3.up, radius, length, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireCylinder(center, Vector3.up, radius, length, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawFilledCylinder(Vector3 center, Vector3 normal, float radius, float length, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKCylinder(center, normal, radius, length, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireCylinder(center, normal, radius, length, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawSphere(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKSphere(center, radius, color, useDepthBuffer));
    }

    public static void DrawWireSphere(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireSphere(center, radius, color, useDepthBuffer));
    }

    public static void DrawFilledSphere(Vector3 center, float radius, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKSphere(center, radius, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireSphere(center, radius, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawLine(Vector3 p1, Vector3 p2, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKLine(p1, p2, color, useDepthBuffer));
    }

    public static void DrawPoint(Vector3 point, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKPoint(point, color, useDepthBuffer));
    }

    public static void DrawAABB(Vector3 center, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKAABB(center, halfExtents, color, useDepthBuffer));
    }

    public static void DrawWireAABB(Vector3 center, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireAABB(center, halfExtents, color, useDepthBuffer));
    }

    public static void DrawFilledAABB(Vector3 center, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKAABB(center, halfExtents, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireAABB(center, halfExtents, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawOBB(Vector3 center, Quaternion rotation, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKOBB(center, rotation, halfExtents, color, useDepthBuffer));
    }

    public static void DrawWireOBB(Vector3 center, Quaternion rotation, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKWireOBB(center, rotation, halfExtents, color, useDepthBuffer));
    }

    public static void DrawFilledOBB(Vector3 center, Quaternion rotation, Vector3 halfExtents, Color color, bool useDepthBuffer = true)
    {
        _pendingShapeRequests.Push(new GDKOBB(center, rotation, halfExtents, GetAutoFillColor(color), useDepthBuffer));
        _pendingShapeRequests.Push(new GDKWireOBB(center, rotation, halfExtents, GetAutoWireColor(color), useDepthBuffer));
    }

    public static void DrawShape(GDKShape shape)
    {
        _pendingShapeRequests.Push(shape);
    }

    private static void DrawMesh(Mesh mesh, Color color, bool useDepthBuffer)
    {
        _materialProperty.SetColor("_Color", color);
        Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, useDepthBuffer ? _depthBufferMaterial : _noDepthBufferMaterial, 0, null, 0, _materialProperty);
    }

    /// <summary>
    /// Will draw at screen point if either axis is above 1, or at a normalized position otherwise.
    /// </summary>
    public static void DrawText(string text, Vector2 screenPos, Color color, bool drawShadow = true)
    {
        if (screenPos.x > 1f || screenPos.y > 1f)
        {
            DrawTextScreenPoint(text, screenPos, color, drawShadow);
        }
        else
        {
            DrawTextScreenPosNormalized(text, screenPos, color, drawShadow);
        }
    }

    public static void DrawTextScreenPoint(string text, Vector2 screenPos, Color color, bool drawShadow = true)
    {
        _pendingTextRequests.Push(new GDKText(text, screenPos, color, drawShadow));
    }

    public static void DrawTextScreenPosNormalized(string text, Vector2 screenPos, Color color, bool drawShadow = true)
    {
        _pendingTextRequests.Push(new GDKText(text, new Vector2(screenPos.x * Screen.width, screenPos.y * Screen.height), color, drawShadow));
    }

    public static void DrawText(string text, Vector3 worldPos, Color color, bool drawShadow = true)
    {
        _pendingTextRequests.Push(new GDKText(text, worldPos, color, drawShadow));
    }

    /// <summary>
    /// Can only be called during OnGUI() for editor windows
    /// </summary>
    public static void DrawLineInEditor(Vector2 pointA, Vector2 pointB, float width, Color color)
    {
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        GUI.color = color;
        GUIUtility.RotateAroundPivot(Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * Mathf.Rad2Deg, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, (pointA - pointB).magnitude, width), Texture2D.whiteTexture);
        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }
}
