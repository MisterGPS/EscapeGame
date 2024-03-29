using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class CableEnd : MonoBehaviour, IInteractable
{
    public delegate void OnConnectionClicked(CableEnd cableEnd);
    public OnConnectionClicked connectionClickedDelegate;

    public Color CableEndColor { get; private set; }

    // This property should be set to an empty placed at the exact spot of where the cable should be attached to
    [SerializeField]
    private GameObject connectionTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (connectionClickedDelegate != null)
            connectionClickedDelegate(this);
    }

    public Vector3 GetConnectionPosition()
    {
        Vector3 connectionCenter = transform.parent.position;
        Vector3 connectionTargetPosition = connectionTarget.transform.position;
        Vector3 relativeEndPosition = connectionCenter - connectionTargetPosition;
        Vector3 scaling = transform.parent.lossyScale;
        relativeEndPosition = new Vector3(relativeEndPosition.x / scaling.x,
                                          relativeEndPosition.y / scaling.y,
                                          relativeEndPosition.z / scaling.z);
        relativeEndPosition.x = relativeEndPosition.z;
        relativeEndPosition.z = relativeEndPosition.y;
        
        return relativeEndPosition;
    }

    public void SetCableEndColor(Color value)
    {
        CableEndColor = value;
        gameObject.GetComponent<SpriteRenderer>().material.color = value;
    }
}
