using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////
////   WIP   ////
///////////////// 
[RequireComponent(typeof(SpriteMask))]
public class BasePuzzleObject : MonoBehaviour
{
    // Automatic default ordering: Front, Top, Back
    [SerializeField]
    public List<Sprite> faces;

    private PlayerController playerController;

    private void Reset()
    {
        
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = Camera.current.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
