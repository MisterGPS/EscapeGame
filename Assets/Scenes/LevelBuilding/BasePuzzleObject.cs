using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////
////   WIP   ////
///////////////// 
[ExecuteInEditMode]
public class BasePuzzleObject : MonoBehaviour
{
    // Automatic default ordering: Front, Top, Back
    [SerializeField]
    public List<Sprite> spriteFaces;
    private List<GameObject> Faces = new List<GameObject>();

    private PlayerController playerController;

    public bool createFaces = false;
    public string PuzzleName = "BasePuzzle";

    [SerializeField]
    public GameObject PlayerCamera;

    private void Reset()
    {
        name = PuzzleName;
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerCamera.GetComponent<PlayerController>();
        ViewMode currentView = playerController.GetViewMode();
    }

    void BuildFaces()
    {
        Debug.Log("Build Faces called");

        for (int i = 0; i < Faces.Count; i++)
        {
            DestroyImmediate(Faces[i].gameObject);
        }

        Faces.Clear();
        
        for (int i = 0; i < spriteFaces.Count; i++)
        {
            GameObject newFace = new GameObject();
            newFace.transform.position = transform.position;
            newFace.transform.rotation = transform.rotation;
            newFace.transform.localScale = transform.localScale;
            newFace.transform.parent = transform;

            SpriteRenderer renderer = newFace.AddComponent<SpriteRenderer>();
            renderer.sprite = spriteFaces[i];
            newFace.AddComponent<BoxCollider>();
            newFace.AddComponent<BasePuzzleSide>();
            newFace.name = PuzzleName + i;
            Faces.Add(newFace);
        }

        // Setup face sides individually
        Vector3 frontFaceSize = Faces[0].GetComponent<BoxCollider>().size;
        Vector3 topFaceSize = Faces[1].GetComponent<BoxCollider>().size;

        Faces[0].transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        Faces[1].transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        Faces[2].transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        Faces[0].transform.localPosition -= new Vector3(0, 0, topFaceSize.x / 2.0f);
        Faces[1].transform.localPosition += new Vector3(0, frontFaceSize.y / 2.0f, 0);
        Faces[2].transform.localPosition += new Vector3(0, 0, topFaceSize.x / 2.0f);

    }

    // Update is called once per frame
    void Update()
    {
        if (createFaces)
        {
            BuildFaces();
            createFaces = false;
        }
    }
}
