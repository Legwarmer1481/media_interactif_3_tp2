using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Niveau00", menuName = "ScriptableObjects/Niveau")]
public class GestionNiveau : ScriptableObject
{

    public int objectif01;
    public int progresObjectif01;
    public int objectif02;
    public int progresObjectif02;
    public bool objetAcqui = false;
    
}
