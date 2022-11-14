using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Point3D
{
    public Vector3 pos;
    public Quaternion rot;

    public Point3D(Vector3 _pos)
    {
        pos = _pos;
        rot = Quaternion.identity;
    }

    public Point3D(Vector3 _pos, Quaternion _rot)
    {
        pos = _pos;
        rot = _rot;
    }
}

public class Curve3D
{
    public Point3D start;
    public float startLength;
    public Point3D end;
    public float endLength;

    public Curve3D(Point3D _start, Point3D _end)
    {
        start = _start;
        end = _end;
        startLength = 1;
        endLength = 1;
    }

    public Curve3D(Point3D _start, float _startLength, Point3D _end, float _endLength)
    {
        start = _start;
        end = _end;
        startLength = _startLength;
        endLength = _endLength;
    }

    public Vector3 GetPos(float percent)
    {
        if (percent <= 0)
            return start.pos;
        if (percent >= 1)
            return end.pos;

        Vector3 startPoint = start.pos + start.rot * Vector3.forward * startLength;
        Vector3 endPoint = end.pos - end.rot * Vector3.forward * endLength;

        Vector3 p1 = start.pos * (1 - percent) + startPoint * percent;
        Vector3 p2 = startPoint * (1 - percent) + endPoint * percent;
        Vector3 p3 = endPoint * (1 - percent) + end.pos * percent;

        Vector3 pp1 = p1 * (1 - percent) + p2 * percent;
        Vector3 PP2 = p2 * (1 - percent) + p3 * percent;

        return pp1 * (1 - percent) + PP2 * percent;
    }

    public Point3D Get(float percent)
    {
        if (percent <= 0)
            return start;
        if (percent >= 1)
            return end;

        Vector3 pos = GetPos(percent);

        Quaternion rot = Quaternion.Slerp(start.rot, end.rot, percent);

        return new Point3D(pos, rot);
    }

    public float GetLength(int precision = 10)
    {
        float length = 0;

        Vector3 previousPos = start.pos;
        for(int i = 0; i <= precision; i++)
        {
            Vector3 endPos;
            if (i == precision)
                endPos = end.pos;
            else
            {
                float percent = (i + 1) / (precision + 1);
                endPos = GetPos(percent);
            }
            length += (endPos - previousPos).magnitude;
            previousPos = endPos;
        }

        return length;
    }
}
