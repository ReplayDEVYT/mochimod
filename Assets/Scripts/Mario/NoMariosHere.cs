using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMariosHere : MonoBehaviour
{
    [SerializeField] GameObject _theManHimself;

    bool _isHeHere = false;
    void Update()
    {
        MarioCheck(); //Check if mario
    }

    void MarioCheck(){
        if(_isHeHere) return; // no mario

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.M)){ // yes mario
            _isHeHere = true;
            GameObject _heHasRisen = Instantiate(_theManHimself, Vector3.zero, Quaternion.identity);

            _heHasRisen.GetComponent<ExampleInputProvider>().cameraObject = Camera.main.gameObject;

            Time.timeScale = 0.75f;
        }
    }
}