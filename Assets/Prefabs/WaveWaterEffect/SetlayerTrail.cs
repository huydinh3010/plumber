using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetlayerTrail : MonoBehaviour {
    enum TypeSort
    {
        line,
        meshrender,
        LineRender
    }
    [SerializeField]
    TypeSort typesortlayer = TypeSort.line;
    [SerializeField]
    int numberoderlayer = 3;
    [SerializeField]
    string namesortLayer = "Player";
    // Use this for initialization
    void Start () {
       
      
        
    }
    private void OnEnable()
    {
        switch (typesortlayer)
        {
            case TypeSort.line:
                TrailRenderer line = GetComponent<TrailRenderer>();
                if (line != null)
                {
                    // print("dont null");
                    line.sortingLayerName = namesortLayer;
                }
                break;
            case TypeSort.meshrender:
                MeshRenderer myMeshRenderer = GetComponent<MeshRenderer>();
                if (myMeshRenderer != null)
                {
                    myMeshRenderer.sortingLayerName = namesortLayer;
                    myMeshRenderer.sortingOrder = numberoderlayer;
                }
                break;
            case TypeSort.LineRender:
                LineRenderer linerender = GetComponent<LineRenderer>();
                {
                    if (linerender != null)
                    {
                        linerender.sortingLayerName = namesortLayer;
                        linerender.sortingOrder = numberoderlayer;
                    }
                }
                break;
        }
    }

}
