using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllerAuDebut : MonoBehaviour
{
    
    private GestionScenes scene;

    void Start()
    {

        scene = GetComponent<GestionScenes>();
        scene.SceneAccueil();       
    }
}
