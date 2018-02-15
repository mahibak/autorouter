using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GDKShape
{
    protected const float line_width = 0.01f; // How thick lines are in world space
    protected const float point_line_extent = 0.3f; // How big the cross is for the point shape

    private Color _color;
    private bool _useDepthBuffer;

    public Color GetColor() { return _color; }
    public bool GetUseDepthBuffer() { return _useDepthBuffer; }

    public GDKShape(Color color, bool useDepthBuffer)
    {
        _color = color;
        _useDepthBuffer = useDepthBuffer;
    }

    public abstract Mesh GetMesh();

    protected static Mesh GetMeshFromLines(Line[] _lines)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4 * _lines.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] tri = new int[6 * _lines.Length];
        Vector3 camPos = Camera.main.transform.position;
        Vector3 side;

        int i = 0;
        int j = 0;
        foreach (Line line in _lines)
        {
            side = Vector3.Cross(camPos - (line._p1 + line._p2) * 0.5f, line._p1 - line._p2).normalized * line_width;
            
            vertices[i] = line._p1 + side;
            vertices[i+1] = line._p1 - side;
            vertices[i+2] = line._p2 - side;
            vertices[i+3] = line._p2 + side;
            
            tri[j+0] = i+1; tri[j+1] = i; tri[j+2] = i+2;
            tri[j+3] = i+2; tri[j+4] = i; tri[j+5] = i+3;

            i += 4;
            j += 6;
        }

        for (int k = 0; k < normals.Length; ++k)
        {
            normals[k] = -Vector3.forward;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }

    protected struct Line
    {
        public Line(Vector3 p1, Vector3 p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public Vector3 _p1;
        public Vector3 _p2;
    }
}

public class GDKText
{
    private string _text;
    private Color _color;
    private bool _drawShadow;
    private Vector2 _screenPos;

    public string GetText() { return _text; }
    public Color GetColor() { return _color; }
    public bool GetDrawShadow() { return _drawShadow; }
    public Vector2 GetScreenPos() { return _screenPos; }
    
    public GDKText(string text, Vector2 screenPos, Color color, bool drawShadow)
    {
        _text = text;
        _color = color;
        _drawShadow = drawShadow;
        _screenPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
    }

    public GDKText(string text, Vector3 worldPos, Color color, bool drawShadow)
    {
        _text = text;
        _color = color;
        _drawShadow = drawShadow;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        _screenPos = new Vector2(screenPos.x, screenPos.y);
    }
}

public class GDKArc : GDKShape
{
    Vector3 _center;
    Vector3 _normal;
    Vector3 _dir;
    float _radius;
    float _angleDeg;
    int _sides;

    public GDKArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _dir = dir;
        _radius = radius;
        _angleDeg = angleDeg;
        _sides = sides;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = _angleDeg * Mathf.Deg2Rad / _sides;
        float baseAngle = (90f - _angleDeg * 0.5f) * Mathf.Deg2Rad;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[_sides + 2];
        vertices[0] = _center;

        Vector3 forward = _dir.normalized * _radius;
        Vector3 right = Vector3.Cross(forward, _normal).normalized * _radius;
        Vector3 up = Vector3.Cross(right, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);

        float currentAngle = baseAngle;
        for (int i = 0; i <= _sides; i++)
        {
            currentAngle = baseAngle + i * anglePerSide;
            vertices[i + 1] = _center + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)));
        }

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[_sides * 6];

        int baseIndex;
        for (int i = 0; i < _sides; ++i)
        {
            baseIndex = i * 6;
            tri[baseIndex] = 0; tri[baseIndex + 1] = i + 1; tri[baseIndex + 2] = i + 2;
            tri[baseIndex + 3] = 0; tri[baseIndex + 4] = i + 2; tri[baseIndex + 5] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireArc : GDKShape
{
    Vector3 _center;
    Vector3 _normal;
    Vector3 _dir;
    float _radius;
    float _angleDeg;
    int _sides;

    public GDKWireArc(Vector3 center, Vector3 normal, Vector3 dir, float radius, float angleDeg, int sides, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _dir = dir;
        _radius = radius;
        _angleDeg = angleDeg;
        _sides = sides;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = _angleDeg * Mathf.Deg2Rad / _sides;
        float baseAngle = (90f - _angleDeg * 0.5f) * Mathf.Deg2Rad;
        
        Vector3[] points = new Vector3[_sides + 2];
        points[0] = _center;

        Vector3 forward = _dir.normalized * _radius;
        Vector3 right = Vector3.Cross(forward, _normal).normalized * _radius;
        Vector3 up = Vector3.Cross(right, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);

        float currentAngle = baseAngle;
        for (int i = 0; i <= _sides; i++)
        {
            currentAngle = baseAngle + i * anglePerSide;
            points[i + 1] = _center + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)));
        }

        Line[] lines = new Line[_sides + 2];

        for (int i = 0; i <= _sides; i++)
        {
            lines[i] = new Line(points[i], points[i + 1]);
        }

        lines[_sides + 1] = new Line(points[_sides + 1], points[0]);

        return GetMeshFromLines(lines);
    }
}

public class GDKCircle : GDKShape
{ 
    Vector3 _center;
    Vector3 _normal;
    float _radius;
    const int _sides = 23;

    public GDKCircle(Vector3 center, Vector3 normal, float radius, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _radius = radius;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = Math.PI_2 / _sides;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[_sides];

        Vector3 up = _normal.normalized * _radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);

        for (int i = 0; i < _sides; i++)
        {
            vertices[i] = _center + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(i * anglePerSide), 0, Mathf.Sin(i * anglePerSide)));
        }

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[(_sides - 2) * 6];

        int baseIndex;
        for (int i = 0; i < _sides - 2; ++i)
        {
            baseIndex = i * 6;
            tri[baseIndex] = 0; tri[baseIndex + 1] = i + 1; tri[baseIndex + 2] = i + 2;
            tri[baseIndex + 3] = 0; tri[baseIndex + 4] = i + 2; tri[baseIndex + 5] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireCircle : GDKShape
{
    Vector3 _center;
    Vector3 _normal;
    float _radius;
    const int _sides = 23;

    public GDKWireCircle(Vector3 center, Vector3 normal, float radius, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _radius = radius;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = Mathf.PI * 2.0f / _sides;
        Vector3 up = _normal.normalized * _radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);
        
        Vector3[] points = new Vector3[_sides];

        for (int i = 0; i < _sides; i++)
        {
            points[i] = _center + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(i * anglePerSide), 0, Mathf.Sin(i * anglePerSide)));
        }

        Line[] lines = new Line[_sides];

        for (int i = 0; i < _sides - 1; i++)
        {
            lines[i] = new Line(points[i], points[i + 1]);
        }

        lines[_sides - 1] = new Line(points[_sides - 1], points[0]);

        return GetMeshFromLines(lines);
    }
}

public class GDKCylinder : GDKShape
{
    Vector3 _center;
    Vector3 _normal;
    float _radius;
    float _length;
    static int _sides = 17;

    public GDKCylinder(Vector3 center, Vector3 normal, float radius, float length, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _radius = radius;
        _length = length;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = Mathf.PI * 2.0f / _sides;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[_sides * 2];

        Vector3 up = _normal.normalized * _radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);

        Vector3 centerTop = _center + _normal * _length * 0.5f;
        Vector3 offsetFromTop = -_normal * _length;

        for (int i = 0; i < _sides; i++)
        {
            vertices[i] = centerTop + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(i * anglePerSide), 0, Mathf.Sin(i * anglePerSide)));
            vertices[i + _sides] = vertices[i] + offsetFromTop;
        }
        
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }
        
        int[] tri = new int[(_sides * 4 - 4) * 3];
        int baseIndexTop;
        int baseIndexBot;
        int baseIndexSide;

        for (int i = 0; i < _sides - 2; ++i)
        {
            baseIndexTop = i * 3;
            tri[baseIndexTop] = 0; tri[baseIndexTop + 1] = i + 2; tri[baseIndexTop + 2] = i + 1;

            baseIndexBot = (i + _sides - 2) * 3;
            tri[baseIndexBot] = _sides; tri[baseIndexBot + 1] = _sides + i + 1; tri[baseIndexBot + 2] = _sides + i + 2;

            baseIndexSide = ((i + _sides) * 2 - 4) * 3;
            tri[baseIndexSide] = i; tri[baseIndexSide + 1] = i + _sides + 1; tri[baseIndexSide + 2] = i + _sides;
            tri[baseIndexSide + 3] = i; tri[baseIndexSide + 4] = i + 1; tri[baseIndexSide + 5] = i + _sides + 1;
        }

        baseIndexBot = tri.Length - 12;
        tri[baseIndexBot] = _sides - 1; tri[baseIndexBot + 1] = 2 * _sides - 1; tri[baseIndexBot + 2] = 2 * _sides - 2;
        tri[baseIndexBot + 3] = _sides - 1; tri[baseIndexBot + 4] = 2 * _sides - 2; tri[baseIndexBot + 5] = _sides - 2;

        baseIndexBot = tri.Length - 6;
        tri[baseIndexBot] = 0; tri[baseIndexBot + 1] = _sides; tri[baseIndexBot + 2] = 2 * _sides - 1;
        tri[baseIndexBot + 3] = 0; tri[baseIndexBot + 4] = 2 * _sides - 1; tri[baseIndexBot + 5] = _sides - 1;
        
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireCylinder : GDKShape
{
    Vector3 _center;
    Vector3 _normal;
    float _radius;
    float _length;
    static int _sides = 24;

    public GDKWireCylinder(Vector3 center, Vector3 normal, float radius, float length, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _normal = normal;
        _radius = radius;
        _length = length;
    }

    public override Mesh GetMesh()
    {
        float anglePerSide = Mathf.PI * 2.0f / _sides;
        Vector3 up = _normal.normalized * _radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * _radius;

        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetColumn(0, right);
        matrix.SetColumn(1, up);
        matrix.SetColumn(2, forward);

        Vector3 centerTop = _center + _normal * _length * 0.5f;
        Vector3 offsetFromTop = -_normal * _length;

        Vector3[] points = new Vector3[_sides * 2];

        for (int i = 0; i < _sides; i++)
        {
            points[i] = centerTop + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(i * anglePerSide), 0, Mathf.Sin(i * anglePerSide)));
            points[i + _sides] = points[i] + offsetFromTop;
        }

        Line[] lines = new Line[_sides * 2 + 8];

        for (int i = 0; i < _sides - 1; i++)
        {
            lines[i] = new Line(points[i], points[i + 1]);
            lines[i + _sides] = new Line(points[i + _sides], points[i + 1 + _sides]);
        }

        lines[_sides - 1] = new Line(points[_sides - 1], points[0]);
        lines[_sides * 2 - 1] = new Line(points[_sides * 2 - 1], points[_sides]);

        lines[_sides * 2] = new Line(points[0], points[_sides]);
        lines[_sides * 2 + 1] = new Line(points[3], points[_sides + 3]);
        lines[_sides * 2 + 2] = new Line(points[6], points[_sides + 6]);
        lines[_sides * 2 + 3] = new Line(points[9], points[_sides + 9]);
        lines[_sides * 2 + 4] = new Line(points[12], points[_sides + 12]);
        lines[_sides * 2 + 5] = new Line(points[15], points[_sides + 15]);
        lines[_sides * 2 + 6] = new Line(points[18], points[_sides + 18]);
        lines[_sides * 2 + 7] = new Line(points[21], points[_sides + 21]);

        return GetMeshFromLines(lines);
    }
}

public class GDKTriangle : GDKShape
{
    Vector3 _p1;
    Vector3 _p2;
    Vector3 _p3;

    public GDKTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
    }

    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        vertices[0] = _p1;
        vertices[1] = _p2;
        vertices[2] = _p3;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[6];
        tri[0] = 0; tri[1] = 1; tri[2] = 2;
        tri[3] = 1; tri[4] = 0; tri[5] = 2;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireTriangle : GDKShape
{
    Vector3 _p1;
    Vector3 _p2;
    Vector3 _p3;

    public GDKWireTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
    }

    public override Mesh GetMesh()
    {
        Line[] lines = new Line[3];
        lines[0] = new Line(_p1, _p2);
        lines[1] = new Line(_p2, _p3);
        lines[2] = new Line(_p3, _p1);

        return GetMeshFromLines(lines);
    }
}

public class GDKQuad : GDKShape
{
    Vector3 _p1;
    Vector3 _p2;
    Vector3 _p3;
    Vector3 _p4;

    public GDKQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
        _p4 = p4;
    }

    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = _p1;
        vertices[1] = _p2;
        vertices[2] = _p3;
        vertices[3] = _p4;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[12];
        tri[0] = 0; tri[1] = 1; tri[2] = 2;
        tri[3] = 1; tri[4] = 0; tri[5] = 2;
        tri[6] = 0; tri[7] = 2; tri[8] = 3;
        tri[9] = 2; tri[10] = 0; tri[11] = 3;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireQuad : GDKShape
{
    Vector3 _p1;
    Vector3 _p2;
    Vector3 _p3;
    Vector3 _p4;

    public GDKWireQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
        _p4 = p4;
    }

    public override Mesh GetMesh()
    {
        Line[] lines = new Line[4];
        lines[0] = new Line(_p1, _p2);
        lines[1] = new Line(_p2, _p3);
        lines[2] = new Line(_p3, _p4);
        lines[3] = new Line(_p4, _p1);

        return GetMeshFromLines(lines);
    }
}

/// <summary>
/// Default sphere implementation is a UV sphere
/// </summary>
public class GDKSphere : GDKShape
{
    protected Vector3 _center;
    protected float _radius;

    public GDKSphere(Vector3 center, float radius, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _radius = radius;
    }

    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[26];
        float sqrt2 = 0.7071069f * _radius;
        float half = _radius * 0.5f;

        vertices[0] = _center + new Vector3(0, _radius, 0);

        vertices[1] = _center + new Vector3(0, sqrt2, sqrt2);
        vertices[2] = _center + new Vector3(half, sqrt2, half);
        vertices[3] = _center + new Vector3(sqrt2, sqrt2, 0);
        vertices[4] = _center + new Vector3(half, sqrt2, -half);
        vertices[5] = _center + new Vector3(0, sqrt2, -sqrt2);
        vertices[6] = _center + new Vector3(-half, sqrt2, -half);
        vertices[7] = _center + new Vector3(-sqrt2, sqrt2, 0);
        vertices[8] = _center + new Vector3(-half, sqrt2, half);

        vertices[9] = _center + new Vector3(0, 0, _radius);
        vertices[10] = _center + new Vector3(sqrt2, 0, sqrt2);
        vertices[11] = _center + new Vector3(_radius, 0, 0);
        vertices[12] = _center + new Vector3(sqrt2, 0, -sqrt2);
        vertices[13] = _center + new Vector3(0, 0, -_radius);
        vertices[14] = _center + new Vector3(-sqrt2, 0, -sqrt2);
        vertices[15] = _center + new Vector3(-_radius, 0, 0);
        vertices[16] = _center + new Vector3(-sqrt2, 0, sqrt2);

        vertices[17] = _center + new Vector3(0, -sqrt2, sqrt2);
        vertices[18] = _center + new Vector3(half, -sqrt2, half);
        vertices[19] = _center + new Vector3(sqrt2, -sqrt2, 0);
        vertices[20] = _center + new Vector3(half, -sqrt2, -half);
        vertices[21] = _center + new Vector3(0, -sqrt2, -sqrt2);
        vertices[22] = _center + new Vector3(-half, -sqrt2, -half);
        vertices[23] = _center + new Vector3(-sqrt2, -sqrt2, 0);
        vertices[24] = _center + new Vector3(-half, -sqrt2, half);

        vertices[25] = _center + new Vector3(0, -_radius, 0);
        
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[144];

        tri[0] = 0; tri[1] = 1; tri[2] = 2;
        tri[3] = 0; tri[4] = 2; tri[5] = 3;
        tri[6] = 0; tri[7] = 3; tri[8] = 4;
        tri[9] = 0; tri[10] = 4; tri[11] = 5;
        tri[12] = 0; tri[13] = 5; tri[14] = 6;
        tri[15] = 0; tri[16] = 6; tri[17] = 7;
        tri[18] = 0; tri[19] = 7; tri[20] = 8;
        tri[21] = 0; tri[22] = 8; tri[23] = 1;
        
        tri[24] = 1; tri[25] = 9; tri[26] = 10;
        tri[27] = 1; tri[28] = 10; tri[29] = 2;
        tri[30] = 2; tri[31] = 10; tri[32] = 11;
        tri[33] = 2; tri[34] = 11; tri[35] = 3;
        tri[36] = 3; tri[37] = 11; tri[38] = 12;
        tri[39] = 3; tri[40] = 12; tri[41] = 4;
        tri[42] = 4; tri[43] = 12; tri[44] = 13;
        tri[45] = 4; tri[46] = 13; tri[47] = 5;
        tri[48] = 5; tri[49] = 13; tri[50] = 14;
        tri[51] = 5; tri[52] = 14; tri[53] = 6;
        tri[54] = 6; tri[55] = 14; tri[56] = 15;
        tri[57] = 6; tri[58] = 15; tri[59] = 7;
        tri[60] = 7; tri[61] = 15; tri[62] = 16;
        tri[63] = 7; tri[64] = 16; tri[65] = 8;
        tri[66] = 8; tri[67] = 16; tri[68] = 9;
        tri[69] = 8; tri[70] = 9; tri[71] = 1;

        tri[72] = 9; tri[73] = 17; tri[74] = 18;
        tri[75] = 9; tri[76] = 18; tri[77] = 10;
        tri[78] = 10; tri[79] = 18; tri[80] = 19;
        tri[81] = 10; tri[82] = 19; tri[83] = 11;
        tri[84] = 11; tri[85] = 19; tri[86] = 20;
        tri[87] = 11; tri[88] = 20; tri[89] = 12;
        tri[90] = 12; tri[91] = 20; tri[92] = 21;
        tri[93] = 12; tri[94] = 21; tri[95] = 13;
        tri[96] = 13; tri[97] = 21; tri[98] = 22;
        tri[99] = 13; tri[100] = 22; tri[101] = 14;
        tri[102] = 14; tri[103] = 22; tri[104] = 23;
        tri[105] = 14; tri[106] = 23; tri[107] = 15;
        tri[108] = 15; tri[109] = 23; tri[110] = 24;
        tri[111] = 15; tri[112] = 24; tri[113] = 16;
        tri[114] = 16; tri[115] = 24; tri[116] = 17;
        tri[117] = 16; tri[118] = 17; tri[119] = 9;

        tri[120] = 25; tri[121] = 18; tri[122] = 17;
        tri[123] = 25; tri[124] = 19; tri[125] = 18;
        tri[126] = 25; tri[127] = 20; tri[128] = 19;
        tri[129] = 25; tri[130] = 21; tri[131] = 20;
        tri[132] = 25; tri[133] = 22; tri[134] = 21;
        tri[135] = 25; tri[136] = 23; tri[137] = 22;
        tri[138] = 25; tri[139] = 24; tri[140] = 23;
        tri[141] = 25; tri[142] = 17; tri[143] = 24;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireSphere : GDKShape
{
    protected Vector3 _center;
    protected float _radius;

    public GDKWireSphere(Vector3 center, float radius, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _radius = radius;
    }

    public override Mesh GetMesh()
    {
        float edge = _radius;
        float far = _radius * 0.866025403f;
        float close = _radius * 0.5f;

        Vector3[] points = new Vector3[30];
        points[0]  = _center + new Vector3(0, edge, 0);
        points[1]  = _center + new Vector3(close, far, 0);
        points[2]  = _center + new Vector3(far, close, 0);
        points[3]  = _center + new Vector3(edge, 0, 0);
        points[4]  = _center + new Vector3(far, -close, 0);
        points[5]  = _center + new Vector3(close, -far, 0);
        points[6]  = _center + new Vector3(0, -edge, 0);
        points[7]  = _center + new Vector3(-close, -far, 0);
        points[8]  = _center + new Vector3(-far, -close, 0);
        points[9]  = _center + new Vector3(-edge, 0, 0);
        points[10] = _center + new Vector3(-far, close, 0);
        points[11] = _center + new Vector3(-close, far, 0);

        points[12] = _center + new Vector3(0, far, -close);
        points[13] = _center + new Vector3(0, close, -far);
        points[14] = _center + new Vector3(0, 0, -edge);
        points[15] = _center + new Vector3(0, -close, -far);
        points[16] = _center + new Vector3(0, -far, -close);
        points[17] = _center + new Vector3(0, -far, close);
        points[18] = _center + new Vector3(0, -close, far);
        points[19] = _center + new Vector3(0, 0, edge);
        points[20] = _center + new Vector3(0, close, far);
        points[21] = _center + new Vector3(0, far, close);

        points[22] = _center + new Vector3(close, 0, far);
        points[23] = _center + new Vector3(far, 0, close);
        points[24] = _center + new Vector3(far, 0, -close);
        points[25] = _center + new Vector3(close, 0, -far);
        points[26] = _center + new Vector3(-close, 0, -far);
        points[27] = _center + new Vector3(-far, 0, -close);
        points[28] = _center + new Vector3(-far, 0, close);
        points[29] = _center + new Vector3(-close, 0, far);

        Line[] lines = new Line[36];
        lines[0] = new Line(points[0], points[1]);
        lines[1] = new Line(points[1], points[2]);
        lines[2] = new Line(points[2], points[3]);
        lines[3] = new Line(points[3], points[4]);
        lines[4] = new Line(points[4], points[5]);
        lines[5] = new Line(points[5], points[6]);
        lines[6] = new Line(points[6], points[7]);
        lines[7] = new Line(points[7], points[8]);
        lines[8] = new Line(points[8], points[9]);
        lines[9] = new Line(points[9], points[10]);
        lines[10] = new Line(points[10], points[11]);
        lines[11] = new Line(points[11], points[0]);

        lines[12] = new Line(points[0], points[12]);
        lines[13] = new Line(points[12], points[13]);
        lines[14] = new Line(points[13], points[14]);
        lines[15] = new Line(points[14], points[15]);
        lines[16] = new Line(points[15], points[16]);
        lines[17] = new Line(points[16], points[6]);
        lines[18] = new Line(points[6], points[17]);
        lines[19] = new Line(points[17], points[18]);
        lines[20] = new Line(points[18], points[19]);
        lines[21] = new Line(points[19], points[20]);
        lines[22] = new Line(points[20], points[21]);
        lines[23] = new Line(points[21], points[0]);

        lines[24] = new Line(points[19], points[22]);
        lines[25] = new Line(points[22], points[23]);
        lines[26] = new Line(points[23], points[3]);
        lines[27] = new Line(points[3], points[24]);
        lines[28] = new Line(points[24], points[25]);
        lines[29] = new Line(points[25], points[14]);
        lines[30] = new Line(points[14], points[26]);
        lines[31] = new Line(points[26], points[27]);
        lines[32] = new Line(points[27], points[9]);
        lines[33] = new Line(points[9], points[28]);
        lines[34] = new Line(points[28], points[29]);
        lines[35] = new Line(points[29], points[19]);

        return GetMeshFromLines(lines);
    }
}

public class GDKIcosphere : GDKSphere
{
    public GDKIcosphere(Vector3 center, float radius, Color color, bool useDepthBuffer) : base(center, radius, color, useDepthBuffer)
    {
    }

    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
    
        Vector3[] vertices = new Vector3[12];
        float t = (1.0f + Mathf.Sqrt(5.0f)) * 0.25f * _radius;
        float d = _radius * 0.5f;
    
        vertices[0] = new Vector3(-d, t, 0) + _center;
        vertices[1] = new Vector3(d, t, 0) + _center;
        vertices[2] = new Vector3(-d, -t, 0) + _center;
        vertices[3] = new Vector3(d, -t, 0) + _center;
    
        vertices[4] = new Vector3(0, -d, t) + _center;
        vertices[5] = new Vector3(0, d, t) + _center;
        vertices[6] = new Vector3(0, -d, -t) + _center;
        vertices[7] = new Vector3(0, d, -t) + _center;
    
        vertices[8] = new Vector3(t, 0, -d) + _center;
        vertices[9] = new Vector3(t, 0, d) + _center;
        vertices[10] = new Vector3(-t, 0, -d) + _center;
        vertices[11] = new Vector3(-t, 0, d) + _center;
    
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }
    
        int[] tri = new int[60];
    
        tri[0] = 0; tri[1] = 11; tri[2] = 5;
        tri[3] = 0; tri[4] = 5; tri[5] = 1;
        tri[6] = 0; tri[7] = 1; tri[8] = 7;
        tri[9] = 0; tri[10] = 7; tri[11] = 10;
        tri[12] = 0; tri[13] = 10; tri[14] = 11;
    
        tri[15] = 1; tri[16] = 5; tri[17] = 9;
        tri[18] = 5; tri[19] = 11; tri[20] = 4;
        tri[21] = 11; tri[22] = 10; tri[23] = 2;
        tri[24] = 10; tri[25] = 7; tri[26] = 6;
        tri[27] = 7; tri[28] = 1; tri[29] = 8;
    
        tri[30] = 3; tri[31] = 9; tri[32] = 4;
        tri[33] = 3; tri[34] = 4; tri[35] = 2;
        tri[36] = 3; tri[37] = 2; tri[38] = 6;
        tri[39] = 3; tri[40] = 6; tri[41] = 8;
        tri[42] = 3; tri[43] = 8; tri[44] = 9;
    
        tri[45] = 4; tri[46] = 9; tri[47] = 5;
        tri[48] = 2; tri[49] = 4; tri[50] = 11;
        tri[51] = 6; tri[52] = 2; tri[53] = 10;
        tri[54] = 8; tri[55] = 6; tri[56] = 7;
        tri[57] = 9; tri[58] = 8; tri[59] = 1;
    
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;
    
        return mesh;
    }
}

public class GDKLine : GDKShape
{
    Vector3 _p1;
    Vector3 _p2;

    public GDKLine(Vector3 p1, Vector3 p2, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = p1;
        _p2 = p2;
    }

    public override Mesh GetMesh()
    {
        Line[] lines = new Line[1];
        lines[0] = new Line(_p1, _p2);
        return GetMeshFromLines(lines);
    }
}

public class GDKPoint : GDKShape
{
    Vector3 _p1;

    public GDKPoint(Vector3 point, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _p1 = point;
    }

    public override Mesh GetMesh()
    {
        Line[] lines = new Line[3];
        lines[0] = new Line(_p1 + new Vector3(-point_line_extent, 0f, 0f), _p1 + new Vector3(point_line_extent, 0f, 0f));
        lines[1] = new Line(_p1 + new Vector3(0f, - point_line_extent, 0f), _p1 + new Vector3(0f, point_line_extent, 0f));
        lines[2] = new Line(_p1 + new Vector3(0f, 0f, - point_line_extent), _p1 + new Vector3(0f, 0f, point_line_extent));
        return GetMeshFromLines(lines);
    }
}

public class GDKAABB : GDKShape
{
    Vector3 _center;
    Vector3 _halfExtents;

    public GDKAABB(Vector3 center, Vector3 halfExtents, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _halfExtents = halfExtents;
    }
    
    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        vertices[0] = _center + new Vector3(-_halfExtents.x, _halfExtents.y, _halfExtents.z);
        vertices[1] = _center + new Vector3(_halfExtents.x, _halfExtents.y, _halfExtents.z);
        vertices[2] = _center + new Vector3(_halfExtents.x, _halfExtents.y, -_halfExtents.z);
        vertices[3] = _center + new Vector3(-_halfExtents.x, _halfExtents.y, -_halfExtents.z);
        vertices[4] = _center + new Vector3(-_halfExtents.x, -_halfExtents.y, _halfExtents.z);
        vertices[5] = _center + new Vector3(_halfExtents.x, -_halfExtents.y, _halfExtents.z);
        vertices[6] = _center + new Vector3(_halfExtents.x, -_halfExtents.y, -_halfExtents.z);
        vertices[7] = _center + new Vector3(-_halfExtents.x, -_halfExtents.y, -_halfExtents.z);

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[36];
        tri[0] = 0; tri[1] = 1; tri[2] = 3; //Top
        tri[3] = 1; tri[4] = 2; tri[5] = 3;
        tri[6] = 3; tri[7] = 2; tri[8] = 7; // Front
        tri[9] = 2; tri[10] = 6; tri[11] = 7;
        tri[12] = 2; tri[13] = 1; tri[14] = 6; // Right
        tri[15] = 1; tri[16] = 5; tri[17] = 6;
        tri[18] = 1; tri[19] = 0; tri[20] = 5; // Back
        tri[21] = 0; tri[22] = 4; tri[23] = 5;
        tri[24] = 0; tri[25] = 3; tri[26] = 4; // Left
        tri[27] = 3; tri[28] = 7; tri[29] = 4;
        tri[30] = 7; tri[31] = 6; tri[32] = 4; // Bottom
        tri[33] = 6; tri[34] = 5; tri[35] = 4;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireAABB : GDKShape
{
    Vector3 _center;
    Vector3 _halfExtents;

    public GDKWireAABB(Vector3 center, Vector3 halfExtents, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _halfExtents = halfExtents;
    }

    public override Mesh GetMesh()
    {
        Vector3[] points = new Vector3[8];
        points[0] = _center + new Vector3(-_halfExtents.x, _halfExtents.y, _halfExtents.z);
        points[1] = _center + new Vector3(_halfExtents.x, _halfExtents.y, _halfExtents.z);
        points[2] = _center + new Vector3(_halfExtents.x, _halfExtents.y, -_halfExtents.z);
        points[3] = _center + new Vector3(-_halfExtents.x, _halfExtents.y, -_halfExtents.z);
        points[4] = _center + new Vector3(-_halfExtents.x, -_halfExtents.y, _halfExtents.z);
        points[5] = _center + new Vector3(_halfExtents.x, -_halfExtents.y, _halfExtents.z);
        points[6] = _center + new Vector3(_halfExtents.x, -_halfExtents.y, -_halfExtents.z);
        points[7] = _center + new Vector3(-_halfExtents.x, -_halfExtents.y, -_halfExtents.z);

        Line[] lines = new Line[12];
        lines[0] = new Line(points[0], points[1]);
        lines[1] = new Line(points[1], points[2]);
        lines[2] = new Line(points[2], points[3]);
        lines[3] = new Line(points[3], points[0]);
        lines[4] = new Line(points[0], points[4]);
        lines[5] = new Line(points[1], points[5]);
        lines[6] = new Line(points[2], points[6]);
        lines[7] = new Line(points[3], points[7]);
        lines[8] = new Line(points[4], points[5]);
        lines[9] = new Line(points[5], points[6]);
        lines[10] = new Line(points[6], points[7]);
        lines[11] = new Line(points[7], points[4]);

        return GetMeshFromLines(lines);
    }
}

public class GDKOBB : GDKShape
{
    Vector3 _center;
    Quaternion _rotation;
    Vector3 _halfExtents;

    public GDKOBB(Vector3 center, Quaternion rotation, Vector3 halfExtents, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _halfExtents = halfExtents;
        _rotation = rotation;
    }

    public override Mesh GetMesh()
    {
        Mesh mesh = new Mesh();

        Matrix4x4 matrix = Matrix4x4.TRS(_center, _rotation, Vector3.one);

        Vector3[] vertices = new Vector3[8];
        vertices[0] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, _halfExtents.y, _halfExtents.z));
        vertices[1] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, _halfExtents.y, _halfExtents.z));
        vertices[2] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, _halfExtents.y, -_halfExtents.z));
        vertices[3] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, _halfExtents.y, -_halfExtents.z));
        vertices[4] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, -_halfExtents.y, _halfExtents.z));
        vertices[5] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, -_halfExtents.y, _halfExtents.z));
        vertices[6] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, -_halfExtents.y, -_halfExtents.z));
        vertices[7] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, -_halfExtents.y, -_halfExtents.z));

        
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -Vector3.forward;
        }

        int[] tri = new int[36];
        tri[0] = 0; tri[1] = 1; tri[2] = 3; //Top
        tri[3] = 1; tri[4] = 2; tri[5] = 3;
        tri[6] = 3; tri[7] = 2; tri[8] = 7; // Front
        tri[9] = 2; tri[10] = 6; tri[11] = 7;
        tri[12] = 2; tri[13] = 1; tri[14] = 6; // Right
        tri[15] = 1; tri[16] = 5; tri[17] = 6;
        tri[18] = 1; tri[19] = 0; tri[20] = 5; // Back
        tri[21] = 0; tri[22] = 4; tri[23] = 5;
        tri[24] = 0; tri[25] = 3; tri[26] = 4; // Left
        tri[27] = 3; tri[28] = 7; tri[29] = 4;
        tri[30] = 7; tri[31] = 6; tri[32] = 4; // Bottom
        tri[33] = 6; tri[34] = 5; tri[35] = 4;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = tri;

        return mesh;
    }
}

public class GDKWireOBB : GDKShape
{
    Vector3 _center;
    Quaternion _rotation;
    Vector3 _halfExtents;

    public GDKWireOBB(Vector3 center, Quaternion rotation, Vector3 halfExtents, Color color, bool useDepthBuffer) : base(color, useDepthBuffer)
    {
        _center = center;
        _halfExtents = halfExtents;
        _rotation = rotation;
    }

    public override Mesh GetMesh()
    {
        Matrix4x4 matrix = Matrix4x4.TRS(_center, _rotation, Vector3.one);

        Vector3[] points = new Vector3[8];
        points[0] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, _halfExtents.y, _halfExtents.z));
        points[1] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, _halfExtents.y, _halfExtents.z));
        points[2] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, _halfExtents.y, -_halfExtents.z));
        points[3] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, _halfExtents.y, -_halfExtents.z));
        points[4] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, -_halfExtents.y, _halfExtents.z));
        points[5] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, -_halfExtents.y, _halfExtents.z));
        points[6] = matrix.MultiplyPoint3x4(new Vector3(_halfExtents.x, -_halfExtents.y, -_halfExtents.z));
        points[7] = matrix.MultiplyPoint3x4(new Vector3(-_halfExtents.x, -_halfExtents.y, -_halfExtents.z));
        
        Line[] lines = new Line[12];
        lines[0] = new Line(points[0], points[1]);
        lines[1] = new Line(points[1], points[2]);
        lines[2] = new Line(points[2], points[3]);
        lines[3] = new Line(points[3], points[0]);
        lines[4] = new Line(points[0], points[4]);
        lines[5] = new Line(points[1], points[5]);
        lines[6] = new Line(points[2], points[6]);
        lines[7] = new Line(points[3], points[7]);
        lines[8] = new Line(points[4], points[5]);
        lines[9] = new Line(points[5], points[6]);
        lines[10] = new Line(points[6], points[7]);
        lines[11] = new Line(points[7], points[4]);

        return GetMeshFromLines(lines);
    }
}