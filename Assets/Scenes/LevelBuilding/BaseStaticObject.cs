using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BaseStaticObject : MonoBehaviour
{
    // Automatic default ordering: Front, Top, Back
    [SerializeField]
    protected List<Material> faceMaterials = new List<Material>();

    [SerializeField]
    protected List<GameObject> faces = new List<GameObject>();

    public string objectName = "BaseObject";

    // Build tools
    public bool createFaces = false;
    public bool autoSize = true;
    public bool getSize = false;
    public bool lockObject = true;
    public Vector3 size;

    protected PlayerController playerController;

    public BaseStaticObject()
    {
        for (int i = 0; i < Mathf.Max(faceMaterials.Count, 3); i++)
        {
            faces.Add(null);
        }
    }

    protected virtual void Reset()
    {
        if (faces[0] != null)
        {
            BuildFaces();
        }
    }

    protected virtual void Start()
    {
        playerController = GameManager.GetPlayerController();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!lockObject)
        {
            if (createFaces)
            {
                BuildFaces();
            }
            if (getSize)
            {
                size.x = faces[0].GetComponent<BoxCollider>().size.x;
                size.y = faces[0].GetComponent<BoxCollider>().size.y;
                size.z = faces[1].GetComponent<BoxCollider>().size.y;
            }
        }
        createFaces = getSize = false;
    }

    protected virtual void BuildFaces()
    {
        Debug.Log("Build Faces called");

        for (int i = 0; i < faces.Count; i++)
        {
            DestroyImmediate(faces[i]);
        }

        faces.Clear();

        for (int i = 0; i < Mathf.Max(faceMaterials.Count, 3); i++)
        {
            GameObject newFace = new GameObject();
            newFace.name = objectName + i;

            newFace.transform.position = transform.position;
            newFace.transform.rotation = transform.rotation;
            newFace.transform.localScale = transform.localScale;
            newFace.transform.parent = transform;

            newFace.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newFace.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials[0] = faceMaterials.Count > i ? faceMaterials[i] : null;
            
            BoxCollider newCollider = newFace.AddComponent<BoxCollider>();
            newCollider.size = new Vector3(newCollider.size.x, newCollider.size.y, 0);

            CustomiseAddSide(newFace);

            BaseObjectSide optSide = newFace.GetComponent<BaseObjectSide>();
            if (!optSide)
            {
                optSide = newFace.AddComponent<BaseObjectSide>();
            }
            optSide.orientation = i;
            optSide.parent = this;
            faces.Add(newFace);
        }

        faces[0].transform.Rotate(new Vector3(90.0f, 0.0f, 180.0f));
        faces[1].transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        faces[2].transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        
        // Setup face sides individually
        if (!autoSize)
        {
            faces[0].GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0);
            faces[1].GetComponent<BoxCollider>().size = new Vector3(size.x, size.z, 0);
            faces[2].GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0);
        }

        float objectHeight = faces[0].GetComponent<BoxCollider>().size.y;
        float objectDepth = faces[1].GetComponent<BoxCollider>().size.y;

        faces[0].transform.localPosition -= new Vector3(0, 0, objectDepth / 2.0f);
        faces[1].transform.localPosition += new Vector3(0, objectHeight / 2.0f, 0);
        faces[2].transform.localPosition += new Vector3(0, 0, objectDepth / 2.0f);
    }

    protected virtual GameObject CustomiseAddSide(GameObject side)  { return side; }
}
