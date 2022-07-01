using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableEnd : MonoBehaviour, IInteractable
{
    public delegate void OnConnectionClicked(CableEnd cableEnd);
    public OnConnectionClicked connectionClickedDelegate;

    public Color cableEndColor { get; private set; }

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
        print(connectionCenter);
        print(connectionTargetPosition);
        Vector3 relativeEndPosition = (connectionTargetPosition - connectionCenter) * 10;
        relativeEndPosition.x = relativeEndPosition.z;
        relativeEndPosition.z = relativeEndPosition.y;
        
        return relativeEndPosition;
    }

    public void SetCableEndColor(Color value)
    {
        cableEndColor = value;
        gameObject.GetComponent<SpriteRenderer>().material.color = value;
    }
}
