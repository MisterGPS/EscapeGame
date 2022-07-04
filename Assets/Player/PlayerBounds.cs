using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerBounds : MonoBehaviour
{
    private BoxCollider bounds;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameManager.PlayerController;
    }

    private void FixedUpdate()
    {
        if (playerController.viewMode == ViewMode.TopDown)
        {
            // WIP with playerController.GetCameraViewBoxWorldSpace
        }
    }
}
