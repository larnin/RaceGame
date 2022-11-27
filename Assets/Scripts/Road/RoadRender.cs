using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct RoadRenderInitialData
{
    public Curve3D curve3D;
    public int segmentSize;
    public RoadShape shape;
}

public class RoadRender
{
    RoadRenderInitialData m_initialData;
    bool m_jobEnded = true;

    readonly object m_meshLock = new object();
    SimpleMeshParam<RoadVertexDefinition> m_meshData = new SimpleMeshParam<RoadVertexDefinition>();
    SimpleMeshParam<RoadColliderVertexDefinition> m_colliderData = new SimpleMeshParam<RoadColliderVertexDefinition>();

    public void Init(RoadRenderInitialData data)
    {
        m_initialData = data;
    }

    public RoadRenderInitialData GetInitialData()
    {
        return m_initialData;
    }

    public void StartJob()
    {
        if (!m_jobEnded)
            return;

        m_jobEnded = false;
        ThreadPool.StartJob(this, DoJob, EndJob);
    }

    public bool IsJobEnded()
    {
        return m_jobEnded;
    }

    public int GetMeshCount()
    {
        if (!m_jobEnded)
            return 0;
        lock (m_meshLock)
        {
            return m_meshData.GetMeshCount();
        }
    }

    public void MakeMesh(Mesh mesh, int index)
    {
        mesh.Clear();

        if(!m_jobEnded)
        {
            return;
        }

        lock (m_meshLock)
        {
            if (index < 0 || index >= m_meshData.GetMeshCount())
                return;

            var data = m_meshData.GetMesh(index);

            MeshEx.SetRoadMeshParams(mesh, data.verticesSize, data.indexesSize);

            mesh.SetVertexBufferData(data.vertices, 0, 0, data.verticesSize);
            mesh.SetIndexBufferData(data.indexes, 0, 0, data.indexesSize);

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new UnityEngine.Rendering.SubMeshDescriptor(0, data.indexesSize, MeshTopology.Triangles));

            //full chunk layer
            mesh.bounds = m_initialData.shape.GetBounds(m_initialData.curve3D);
        }
    }

    public int GetCollisionMeshCount()
    {
        if (!m_jobEnded)
            return 0;
        lock (m_meshLock)
        {
            return m_colliderData.GetMeshCount();
        }
    }

    public void MakeCollisionMesh(Mesh mesh, int index)
    {
        mesh.Clear();

        if (!m_jobEnded)
        {
            return;
        }

        lock (m_meshLock)
        {
            if (index < 0 || index >= m_colliderData.GetMeshCount())
                return;

            var data = m_colliderData.GetMesh(index);

            MeshEx.SetRoadColliderMeshParams(mesh, data.verticesSize, data.indexesSize);

            mesh.SetVertexBufferData(data.vertices, 0, 0, data.verticesSize);
            mesh.SetIndexBufferData(data.indexes, 0, 0, data.indexesSize);

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new UnityEngine.Rendering.SubMeshDescriptor(0, data.indexesSize, MeshTopology.Triangles));

            //full chunk layer
            mesh.bounds = m_initialData.shape.GetBounds(m_initialData.curve3D);

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }
    }

    //called from an other thread
    void DoJob()
    {
        lock(m_meshLock)
        {
            m_meshData.ResetSize();

            float totalLen = m_initialData.curve3D.GetLength();
            int segmentNb = (int)(totalLen / m_initialData.segmentSize + 0.5f);
            float segmentSize = 1.0f / segmentNb;
            int nbSquare = segmentNb * m_initialData.shape.points.Count;

            var data = m_meshData.Allocate(nbSquare * 4, nbSquare * 6);

            for(int i = 0; i < segmentNb; i++)
            {
                var p1 = m_initialData.curve3D.Get(i * segmentSize);
                var p2 = m_initialData.curve3D.Get((i + 1) * segmentSize);

                DrawSegment(data, p1, p2);
            }

            DrawCollider();
        }
    }

    void EndJob()
    {
        m_jobEnded = true;
    }

    void DrawSegment(MeshParamData<RoadVertexDefinition> mesh, Point3D p1, Point3D p2)
    {
        for(int i = 0; i < m_initialData.shape.points.Count; i++)
        {
            int i2 = i == m_initialData.shape.points.Count - 1 ? 0 : i + 1;

            Vector3 s1 = m_initialData.shape.points[i];
            Vector3 s2 = m_initialData.shape.points[i2];

            Vector3 o1, o2, o3, o4;
            o1 = p1.pos + p1.rot * s1;
            o2 = p1.pos + p1.rot * s2;
            o3 = p2.pos + p2.rot * s1;
            o4 = p2.pos + p2.rot * s2;

            if (mesh.verticesSize + 4 > mesh.vertices.Length)
                return;
            if (mesh.indexesSize + 6 > mesh.indexes.Length)
                return;

            mesh.vertices[mesh.verticesSize].pos = o1;
            mesh.vertices[mesh.verticesSize + 1].pos = o3;
            mesh.vertices[mesh.verticesSize + 2].pos = o4;
            mesh.vertices[mesh.verticesSize + 3].pos = o2;

            ushort vertexIndex = (ushort)mesh.verticesSize;

            mesh.indexes[mesh.indexesSize] = vertexIndex;
            mesh.indexes[mesh.indexesSize + 1] = (ushort)(vertexIndex + 1);
            mesh.indexes[mesh.indexesSize + 2] = (ushort)(vertexIndex + 3);
            mesh.indexes[mesh.indexesSize + 3] = (ushort)(vertexIndex + 1);
            mesh.indexes[mesh.indexesSize + 4] = (ushort)(vertexIndex + 2);
            mesh.indexes[mesh.indexesSize + 5] = (ushort)(vertexIndex + 3);

            MakeNormal(mesh, mesh.verticesSize);

            mesh.verticesSize += 4;
            mesh.indexesSize += 6;
        }
    }

    void MakeNormal(MeshParamData<RoadVertexDefinition> mesh, int verticeIndex)
    {
        Vector3 p1 = mesh.vertices[verticeIndex].pos;
        Vector3 p2 = mesh.vertices[verticeIndex + 1].pos;
        Vector3 p3 = mesh.vertices[verticeIndex + 2].pos;
        Vector3 p4 = mesh.vertices[verticeIndex + 3].pos;

        Vector3 center = (p1 + p2 + p3 + p4) / 4;

        var n1 = GetNormal(p1, p4, center);
        var n2 = GetNormal(p3, p2, center);

        mesh.vertices[verticeIndex].normal = n1;
        mesh.vertices[verticeIndex + 1].normal = n2;
        mesh.vertices[verticeIndex + 2].normal = n2;
        mesh.vertices[verticeIndex + 3].normal = n1;

        /*
        int i1 = data.indexes[index + i * 3];
            int i2 = data.indexes[index + i * 3 + 1];
            int i3 = data.indexes[index + i * 3 + 2];

            var p1 = data.vertices[i1].pos;
            var p2 = data.vertices[i2].pos;
            var p3 = data.vertices[i3].pos;

            

            data.vertices[i1].normal = n;
            data.vertices[i2].normal = n;
            data.vertices[i3].normal = n;
        */
    }

    Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var v = p2 - p1;
        var w = p3 - p1;

        var n = new Vector3(v.y * w.z - v.z * w.y, v.z * w.x - v.x * w.z, v.x * w.y - v.y * w.x);

        return n;
    }

    void DrawCollider()
    {
        for(int i = 0; i < m_meshData.m_data.Count; i++)
        {
            var meshData = m_meshData.m_data[i];

            var data =  m_colliderData.Allocate(meshData.verticesSize, meshData.indexesSize);

            for(int j = 0; j < meshData.verticesSize; j++)
            {
                data.vertices[data.verticesSize + j].pos = meshData.vertices[j].pos;
                data.vertices[data.verticesSize + j].normal = meshData.vertices[j].normal;
            }

            for(int j = 0; j < meshData.indexesSize; j++)
                data.indexes[data.indexesSize + j] = meshData.indexes[j];

            data.verticesSize += meshData.verticesSize;
            data.indexesSize += meshData.indexesSize;
        }
    }
}
