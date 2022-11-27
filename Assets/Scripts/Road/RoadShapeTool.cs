using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Platform Tool", typeof(RoadShape))]
public class RoadShapeTool : EditorTool, IDrawSelectedHandles
{
    public override GUIContent toolbarIcon 
    { 
        get
        {
            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EditorResources/RoadTool.png", typeof(Texture2D));

            return new GUIContent(t, "Road tool");
        }
    }

    void OnEnable()
    {
        ToolManager.SetActiveTool(this);
    }

    void OnDisable()
    {

    }

    public override void OnActivated()
    {
        
    }

    public override void OnWillBeDeactivated()
    {
        
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView sceneView))
            return;

        var pos = GetMouseProjection();

    }

    public void OnDrawHandles()
    {
        var shape = target as RoadShape;
        if (shape == null)
            return;

        if (shape.points.Count >= 2)
        {
            Handles.color = Color.green;
            for (int i = 0; i < shape.points.Count; i++)
            {
                int i2 = i >= shape.points.Count - 1 ? 0 : i + 1;

                Vector3 p1 = shape.points[i];
                Vector3 p2 = shape.points[i2];

                Handles.DrawLine(p1, p2, 1f);
            }
        }
    }

    void MoveCamera()
    {
        var view = SceneView.lastActiveSceneView;
        view.pivot = new Vector3(0, 0, -10);
        view.rotation = Quaternion.identity;
    }

    Vector2 GetMouseProjection()
    {
        var mousePos = Event.current.mousePosition;
        //mousePos.y = 1 - mousePos.y;
        Debug.Log(mousePos);
        var cam = SceneView.lastActiveSceneView.camera;

        var ray = cam.ScreenPointToRay(mousePos);

        float dist = 0;
        bool interact = new Plane(new Vector3(0, 0, -1), new Vector3(0, 0, 0)).Raycast(ray, out dist);

        if (!interact)
            return Vector2.zero;

        var endPoint = ray.GetPoint(dist);

        return new Vector2(endPoint.x, endPoint.y);
    }

}