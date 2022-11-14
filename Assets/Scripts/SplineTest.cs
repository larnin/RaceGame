using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SplineTest : MonoBehaviour
{
    Curve3D spline;

    private void Start()
    {
        Vector3 pos1 = UnityEngine.Random.insideUnitSphere * 100;
        Vector3 pos2 = UnityEngine.Random.insideUnitSphere * 100;
        Quaternion rot1 = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere, Vector3.up);
        Quaternion rot2 = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere, Vector3.up);
        float len1 = UnityEngine.Random.value * 100;
        float len2 = UnityEngine.Random.value * 100;

        spline = new Curve3D(new Point3D(pos1, rot1), len1, new Point3D(pos2, rot2), len2);
    }

    private void Update()
    {
        int nbPart = 50;

        for (int i = 0; i < nbPart; i++)
        {
            float percent1 = i / (nbPart + 1.0f);
            float percent2 = (i + 1) / (nbPart + 1.0f);

            Vector3 pos1 = spline.GetPos(percent1);
            Vector3 pos2 = spline.GetPos(percent2);

            Debug.DrawLine(pos1, pos2, Color.red);
        }
    }
}