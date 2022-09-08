using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ShadowCasterController shadowCasterController;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.IsLeftMouseDown)
        {
            shadowCasterController.RotateY(inputManager.MousePositionDelta.x);
        }
    }
}
