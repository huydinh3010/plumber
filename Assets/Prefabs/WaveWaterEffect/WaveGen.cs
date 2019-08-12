using UnityEngine;
using System.Collections;

public class WaveGen : MonoBehaviour
{
	public float scaleX = 0f;
	public float scaleZ = 0f;
	float speed = 10.0f;

	private Vector3[] baseHeight;

    public Renderer meshRen;
    Mesh mesh;
    Vector3[] vertices;
    void Start()
    {
        //meshRen.GetComponent<Renderer>().sortingLayerName = "Map";
        //meshRen.GetComponent<Renderer>().sortingOrder = 10;
         mesh = GetComponent<MeshFilter>().mesh;
    }

	void Update () {
	

		if (baseHeight == null)
			baseHeight = mesh.vertices;

		 vertices = new Vector3[baseHeight.Length];
		for (int i=0;i<vertices.Length;i++)
		{
			Vector3 vertex = baseHeight[i];
			vertex.x += Mathf.Sin(Time.time * speed+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scaleX;
			vertex.z+=Mathf.Sin(Time.time * speed+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scaleZ;
			vertices[i] = vertex;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();


	}
	public void Move(){
	
	}
}
