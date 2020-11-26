using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class driverAvatar : MonoBehaviour
{
    Animator driver;
    [HideInInspector]
    public enum driverType
    {
        AttentiveDriver,
        InattentiveDriver
    };
    CarUserControl carControl;
    GameObject carObject;
    public driverType driverStyle;

    [HideInInspector]

    void Start()
    {
        driver = GetComponent<Animator>();
        driver.SetBool("Inattentive", false);
        driver.SetInteger("Stop", 0);
        carObject = GameObject.FindGameObjectWithTag("Player");
        carControl = (CarUserControl)carObject.GetComponent("CarUserControl");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

                    if (carControl.carSpeed() == 0 && carObject.transform.position.z < 20.0)
                    {
                        driver.SetInteger("Stop", 1);
                    }
                    else if (carControl.carSpeed() != 0)
                    {
                        driver.SetInteger("Stop", 2);
                    }
                   

                    
              

                // If we have an inattentive driver, show the phone and act distracted
                //case driverType.InattentiveDriver: {
                //phone.GetComponent<Renderer>().enabled = true;
                /* if (driver.GetBool("Inattentive") == false) {
                     driver.SetBool("Inattentive", true);
                 }
                 break;*/
                //}
        
    }
}
