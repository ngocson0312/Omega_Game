﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SensorPlayer : MonoBehaviour
{
    [SerializeField] float distance = 10;
    [SerializeField] float angle = 30;
    [SerializeField] float height = 1.0f;
    [SerializeField] Color meshColor = Color.red;
    [SerializeField] int ScanFrequency = 30;// tần suất quét
    [SerializeField] LayerMask layers;
    [SerializeField] LayerMask wallLayMask;
    public List<GameObject> objects = new List<GameObject>(); //ds đối tượng có khả năng bị quét

    Collider[] colliders = new Collider[50];
    Mesh mesh;

    int count;
    float scanInterval;
    float scanTimer;

    public void Start()
    {
        scanInterval = 1.0f / ScanFrequency;
    }

    public void Update()
    {
        scanTimer -= Time.deltaTime;
        if(scanTimer<0)
        {
            scanTimer += scanInterval;
            Scan();
        }    
    }

    private void Scan()
    {

        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        objects.Clear();

        //kiểm tra, nếu quét thấy thì cho vào list
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if(IsInsight(obj))
            {
                Debug.Log("1");
                objects.Add(obj);
            }    
        }
    }    

    //kiểm tra xem có trong vòng quét không 
    public bool IsInsight( GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 dir = dest - origin;
        if(dir.y<0 || dir.y>height)
        {
            return false;
        }


        //kiểm tra xem nó có thực sự nằm trong vòng quét không
        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, transform.forward);
        if(deltaAngle >angle)
        {
            return false;
        }
        // kiểm tra khi vòng tác động với tường wall

        origin.y += height / 2;
        dest.y = origin.y;
        if(Physics.Linecast(origin,dest,wallLayMask))
        {
            return false;
        }    
        return true;
    }    

    //Hàm này được gọi khi tập lệnh được tải hoặc một giá trị được thay đổi trong Trình kiểm tra 
    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / ScanFrequency;// quét lại tần suất

    }


    //tạo ra một lưới mesh
    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        //tạo tam giác
        int segemts = 10;
        int numTriangles = (segemts * 4) + 2 + 2;
        int numVertices = numTriangles * 3;// 3 cạnh

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        //tạo ra 3 điểm dưới
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        //tạo ra 3 điểm trên
        Vector3 TopCenter = bottomCenter + Vector3.up* height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;


        int vert = 0;

        //bên trái
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;

        vertices[vert++] = TopCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;

        //bên phải
        vertices[vert++] = bottomCenter;
        vertices[vert++] = TopCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float DeltaAngle = (angle * 2) / segemts;
        for(int i=0;i<segemts;i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle+DeltaAngle, 0) * Vector3.forward * distance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // độ xa
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //trên
            vertices[vert++] = TopCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //dưới

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += DeltaAngle;
        }    

       

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }


    //hàm này giúp vẽ màu sắc
    private void OnDrawGizmos()
    {
        if(mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

       /* Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position,0.2f);
        }
       */
        Gizmos.color = Color.white;
        foreach (var obj in objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }


}
