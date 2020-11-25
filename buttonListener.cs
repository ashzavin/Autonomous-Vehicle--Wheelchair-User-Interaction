using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using UnityEngine.Events;
using UnityStandardAssets.Vehicles.Car;

public class buttonListener : MonoBehaviour
{
    public UnityEvent proximityEvent;
    public UnityEvent contactEvent;
    public UnityEvent actionEvent;
    public UnityEvent defaultEvent;

    CarUserControl carControl;
    TextMesh DebugText;
    string debugMessage = "Default message";

    //code by Asha
    public Material[] material;
    Renderer rend;
    int flag;



    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ButtonController>().InteractableStateChanged.AddListener(InitiateEvent);

        //Get debug message refrence
        GameObject DebugObject = GameObject.FindGameObjectWithTag("Debug");
        DebugText = (TextMesh)DebugObject.GetComponent("TextMesh");

        //Get vehicle refrence
        GameObject carObject = GameObject.FindGameObjectWithTag("Player");
        carControl = (CarUserControl)carObject.GetComponent("CarUserControl");

        //keeping the button material green after pressing the button
        rend = GetComponent<Renderer>();
        flag = 0;
    }

    void InitiateEvent(InteractableStateArgs state)
    {
        
        if (state.NewInteractableState == InteractableState.ProximityState)
        {
            proximityEvent.Invoke();
        }
            
        else if (state.NewInteractableState == InteractableState.ContactState)
        {
            debugMessage = "Button pressed";
            DebugText.text = debugMessage;
            rend.sharedMaterial = material[0];
            contactEvent.Invoke();
            flag = 1;

            carControl.buttonInteraction();
        }
        else if (state.NewInteractableState == InteractableState.ActionState)
        {
            actionEvent.Invoke();
        }
        else
            defaultEvent.Invoke();

        DebugText.text = debugMessage;
    }

    // Update is called once per frame
    //code added by Asha
    void Update()
    {
        if (flag==1)
        {
            
            Invoke("changefvalue", 5);
           
        }
         
    }
    void changefvalue()
    {
        flag = 0;
        rend.sharedMaterial = material[1];
    }

}

