using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "Road", menuName = "Game/Road")]
public class RoadShape : ScriptableObject
{
    [SerializeField] public List<Vector2> points;
    [SerializeField] public Material material;

    public Bounds GetBounds(Curve3D curve)
    {
        var b = curve.GetBounds();

        Vector2 maxSize = Vector2.zero;

        for(int i = 0; i < points.Count; i++)
        {
            float x = MathF.Abs(points[i].x);
            float y = MathF.Abs(points[i].y);

            if (x > maxSize.x)
                maxSize.x = x;
            if (y > maxSize.y)
                maxSize.y = y;
        }

        float fLen = maxSize.magnitude;
        b.Expand(fLen);

        return b;
    }
}
