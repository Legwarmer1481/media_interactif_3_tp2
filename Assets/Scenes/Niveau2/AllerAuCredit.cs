using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllerAuCredit : MonoBehaviour
{
    
    private GestionScenes scene;

    void Start()
    {

        scene = GetComponent<GestionScenes>();
        scene.SceneCredit();       
    }
}
