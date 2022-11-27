using NRand;
using System.Collections.Generic;
using UnityEngine;

public class RoadSection : MonoBehaviour
{
    [SerializeField] RoadShape tempShape;

    RoadRenderInitialData m_initialDatas;

    RoadRender m_renderer = new RoadRender();

    class RenderObject
    {
        public GameObject obj;
        public MeshFilter meshFilter;
        public MeshRenderer renderer;
    }

    class CollisionObject
    {
        public GameObject obj;
        public MeshCollider collider;
    }

    List<RenderObject> m_renderList = new List<RenderObject>();
    List<CollisionObject> m_collisionList = new List<CollisionObject>();

    bool m_generationStarted = false;
    bool m_generated = false;

    void SetData(Curve3D curve3D, float startDistance, RoadShape shape)
    {
        m_initialDatas.curve3D = curve3D;
        m_initialDatas.segmentSize = 1;
        m_initialDatas.shape = shape;

        m_generationStarted = true;

        m_renderer.Init(m_initialDatas);
        m_renderer.StartJob();
    }

    private void Start()
    {
        //debug
        float radius = 100;

        var d1 = new UniformVector3SphereDistribution(radius);
        var d2 = new UniformFloatDistribution(radius);
        var d3 = new SimpleQuaternionDistribution();
        var gen = new MT19937(5);

        SetData(new Curve3D(new Point3D(d1.Next(gen), d3.Next(gen)), d2.Next(gen), new Point3D(d1.Next(gen), d3.Next(gen)), d2.Next(gen)), 0, tempShape);
    }

    void Update()
    {
        if(!m_generated && m_generationStarted)
        {
            if (m_renderer.IsJobEnded())
                MakeMesh();
        }
    }

    void MakeMesh()
    {
        UpdateMeshAndColliderCount();
        
        for(int i = 0; i < m_renderer.GetMeshCount(); i++)
        {
            m_renderList[i].renderer.material = m_initialDatas.shape.material;
            var mesh = m_renderList[i].meshFilter.sharedMesh;

            m_renderer.MakeMesh(mesh, i);
        }

        for(int i = 0; i < m_renderer.GetCollisionMeshCount(); i++)
        {
            var mesh = m_collisionList[i].collider.sharedMesh;

            m_renderer.MakeCollisionMesh(mesh, i);
        }
    }

    void UpdateMeshAndColliderCount()
    {
        int nbMesh = m_renderer.GetMeshCount();

        while (nbMesh < m_renderList.Count)
        {
            var mesh = m_renderList[m_renderList.Count - 1].meshFilter.sharedMesh;
            if(mesh != null)
                Destroy(mesh);
            Destroy(m_renderList[m_renderList.Count - 1].obj);
            m_renderList.RemoveAt(m_renderList.Count - 1);
        }

        while (nbMesh > m_renderList.Count)
            m_renderList.Add(MakeMeshObject());

        int nbCollider = m_renderer.GetCollisionMeshCount();

        while(nbCollider < m_collisionList.Count)
        {
            var mesh = m_collisionList[m_collisionList.Count - 1].collider.sharedMesh;
            if (mesh != null)
                Destroy(mesh);
            Destroy(m_collisionList[m_collisionList.Count - 1].obj);
            m_collisionList.RemoveAt(m_collisionList.Count - 1);
        }

        while (nbCollider > m_collisionList.Count)
            m_collisionList.Add(MakeColliderObject());
    }

    GameObject MakeObject()
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        return obj;
    }

    RenderObject MakeMeshObject()
    {
        var obj = new RenderObject();
        obj.obj = MakeObject();

        obj.meshFilter = obj.obj.AddComponent<MeshFilter>();
        obj.meshFilter.mesh = new Mesh();
        obj.renderer = obj.obj.AddComponent<MeshRenderer>();

        return obj;
    }

    CollisionObject MakeColliderObject()
    {
        var obj = new CollisionObject();
        obj.obj = MakeObject();

        obj.collider = obj.obj.AddComponent<MeshCollider>();
        obj.collider.sharedMesh = new Mesh();

        return obj;
    }

    private void OnDestroy()
    {
        foreach(var obj in m_renderList)
        {
            var mesh = obj.meshFilter.sharedMesh;
            if(mesh != null)
                Destroy(mesh);
        }
    }
}