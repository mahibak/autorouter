using System.Collections.Generic;
using UnityEngine;

public class GridTransform
{
    private Point _position;                    //Pivot, bottom left grid pos when unrotated
    private Point _size;                        //Size in grid tiles
    private Rotation _rotation = Rotation.CW0;  //Rotation around pivot

    public GridTransform() {}
    public GridTransform(GridTransform source)
    {
        _position = source._position;
        _size = source._size;
        _rotation = source._rotation;
    }

    public Rotation GetRotation()
    {
        return _rotation;
    }

    public void SetRotation(Rotation rot)
    {
        _rotation = rot;
    }

    public Point GetTile()
    {
        return _position;
    }

    public void SetTile(Point tile)
    {
        _position = tile;
    }
    
    public Vector3 GetPos()
    {
        return _position.ToVector3();
    }

    public Point GetOppositeTile()
    {
        return new Point(_size.X - 1, _size.Y - 1).Rotate(_rotation) + _position;
    }

    public void SetOppositeTile(Point tile)
    {
        _position = tile - new Point(_size.X - 1, _size.Y - 1).Rotate(_rotation);
    }

    public Vector3 GetOppositePos()
    {
        return GetOppositeTile().ToVector3();
    }

    public Point GetCenterTile()
    {
        return (GetTile() + GetOppositeTile()) * 0.5f;
    }

    public void SetCenterTile(Point tile)
    {
        _position += tile - GetCenterTile();
    }

    public Vector3 GetCenterPos()
    {
        return (((GetPos() + GetOppositePos()) * 0.5f)) + new Vector3(0.5f, 0.5f, 0.5f);
    }

    public Point GetBaseSize()
    {
        return _size;
    }

    public Vector3 GetBaseVectorSize()
    {
        return _size.ToVector3();
    }

    public void SetBaseSize(Point size)
    {
        _size = size;
    }

    public Point GetCurrentSize()
    {
        return _size.RotateAbsolute(_rotation);
    }

    public Vector3 GetCurrentVectorSize(float y = 0f)
    {
        return _size.RotateAbsolute(_rotation).ToVector3();
    }

    public IEnumerable<Point> GetOccupiedTiles()
    {
        switch (_rotation)
        {
            case Rotation.CW90:
                return _position.GetPointsInRectangle(_size.Y, -_size.X);
            case Rotation.CW180:
                return _position.GetPointsInRectangle(-_size.X, -_size.Y);
            case Rotation.CW270:
                return _position.GetPointsInRectangle(-_size.Y, _size.X);
            case Rotation.CW0:
            default:
                return _position.GetPointsInRectangle(_size.X, _size.Y);
        }
    }

    public void DrawInWorld(Color color, float height = 1f, float border = 0f)
    {
        Vector3 halfExtents = GetCurrentVectorSize(height) * 0.5f + new Vector3(border, border, border);
        GDK.DrawFilledAABB(GetCenterPos(), halfExtents, color);
    }
}
