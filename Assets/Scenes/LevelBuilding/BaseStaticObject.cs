using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BaseStaticObject : MonoBehaviour
{
    // Automatic default ordering: Front, Top, Back
    [SerializeField]
    public List<Sprite> spriteFaces = new List<Sprite>();
    protected List<GameObject> Faces = new List<GameObject>();

    public string ObjectName = "BaseObject";

    // Build tools
    public bool createFaces = false;
    public bool autoSize = true;
    public bool getSize = false;
    public Vector3 size;

    [SerializeField]
    public GameObject PlayerCamera;

    protected PlayerController playerController;

    public BaseStaticObject()
    {
        for (int i = 0; i < Mathf.Max(spriteFaces.Count, 3); i++)
        {
            Faces.Add(null);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerController = PlayerCamera.GetComponent<PlayerController>();
        ViewMode currentView = playerController.GetViewMode();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (createFaces)
        {
            BuildFaces();
            createFaces = false;
        }
        if (getSize)
        {
            size.x = Faces[0].GetComponent<BoxCollider>().size.x;
            size.y = Faces[0].GetComponent<BoxCollider>().size.y;
            size.z = Faces[1].GetComponent<BoxCollider>().size.y;
            getSize = false;
        }
    }

    protected virtual void BuildFaces()
    {
        Debug.Log("Build Faces called");

        for (int i = 0; i < Faces.Count; i++)
        {
            DestroyImmediate(Faces[i]);
        }

        Faces.Clear();

        for (int i = 0; i < Mathf.Max(spriteFaces.Count, 3); i++)
        {
            GameObject newFace = new GameObject();

            newFace.transform.position = transform.position;
            newFace.transform.rotation = transform.rotation;
            newFace.transform.localScale = transform.localScale;
            newFace.transform.parent = transform;

            SpriteRenderer renderer = newFace.AddComponent<SpriteRenderer>();
            renderer.sprite = spriteFaces.Count > i ? spriteFaces[i] : null;
            BoxCollider newCollider = newFace.AddComponent<BoxCollider>();
            newCollider.size = new Vector3(newCollider.size.x, newCollider.size.y, 0);

            newFace.name = ObjectName + i;

            Faces.Add(newFace);
        }

        // Setup face sides individually
        if (!autoSize)
        {
            Faces[0].GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0);
            Faces[1].GetComponent<BoxCollider>().size = new Vector3(size.x, size.z, 0);
            Faces[2].GetComponent<BoxCollider>().size = new Vector3(size.x, size.y, 0);
        }

        float ObjectHeight = Faces[0].GetComponent<BoxCollider>().size.y;
        float ObjectDepth = Faces[1].GetComponent<BoxCollider>().size.y;

        Faces[0].transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        Faces[1].transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        Faces[2].transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        Faces[0].transform.localPosition -= new Vector3(0, 0, ObjectDepth / 2.0f);
        Faces[1].transform.localPosition += new Vector3(0, ObjectHeight / 2.0f, 0);
        Faces[2].transform.localPosition += new Vector3(0, 0, ObjectDepth / 2.0f);
    }
}
