using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PourInputs : MonoBehaviour
{
    public Vector2 bouge;
    public Vector2 regarde;
    public bool cours = false;
    public bool saute = false;
    public bool equipe = false;
    public bool camPP = false;
    public bool tire = false;

    public bool cursorLocked = true;


    public void OnMove(InputValue value)
    {
        bouge = value.Get<Vector2>();
    }


    public void OnCourse(InputValue value)
    {
        cours = value.isPressed;
    }


    public void OnSaute(InputValue value)
    {
        saute = value.isPressed;
    }

    public void OnLook(InputValue value)
    {
        regarde = value.Get<Vector2>();
    }

    public void OnEquipe(InputValue value){
        equipe = value.isPressed;
    }

    public void OnCamera(InputValue value){
        camPP = value.isPressed;
    }

    public void OnFire(InputValue value){
        tire = value.isPressed;
    }



    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    public void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }


}
