using System;
using UnityEngine;
using UnityStandardAssets;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    
    public class CarUserControl : MonoBehaviour
    {
        //Scene 1 objects
        GameObject lightStandBulb;
        GameObject audioScene;
        GameObject BLEContainer;
        GameObject crossingLightObject;
        public bulbLogic lightBulbScript;
        public MyWorkingScanScene BLEContainerScript;
        public countdownLight countDownScript;

        bool hapticsTriggered = false;

        //Scene 2 objects
        GameObject carLaserProjection;
        GameObject rectangleLaserContact;

        //Scene 3 objects (and rectangleLaserContact)
        GameObject wcLaserProjection;

        //Scene 4 Objects for Audio Scene
        public GameObject p;
        public GameObject q;
        public GameObject r;
        public GameObject a;
        public GameObject b;
        

        TextMesh DebugText;
        string debugMessage = "Default message";

        private CarController m_Car; // the car controller we want to use
        float steering;
        float accel;
        float footbrake;
        float handbrake;
        int flg=1;
        public Vector3 angletoRotate;

        public bool drive = false;
        public float startTime = 0.0f;

        float flag = 0;        //to check if the button has been pressed or not
        int crosswalkCounterFlag = 0; //This flag starts at 0, becomes 1 after the button is first pressed, and 2 after a second press that extends the timer

        //Change these to limit when the interaction is allowable
        public float triggerAllowStart = 0.0f;
        public float triggerAllowEnd = 4.8f; 

        //For a yet unknown reason, the quest will use the last value of this number used, not the current one
        //If you update the trigger point value you need to change it to the desired value, recompile, change it to something close, and recompile again.
        public float interactionTriggerPoint = 4.8f;

        private void Awake()
        {
            // Get references to Light stand object and bulb if scene 1
            lightStandBulb = GameObject.FindGameObjectWithTag("LightStandBulb");
            if(lightStandBulb != null)
                lightBulbScript = (bulbLogic)lightStandBulb.GetComponent("bulbLogic");

            // Get references to haptics if scene 1 or 5
            BLEContainer = GameObject.FindGameObjectWithTag("BLEContainer");
            if (BLEContainer != null)
            {
                BLEContainerScript = (MyWorkingScanScene)BLEContainer.GetComponent("MyWorkingScanScene");
                Debug.Log("BLEContainerScript was set");
            }

            crossingLightObject = GameObject.FindGameObjectWithTag("crossingLight");
            if (crossingLightObject != null)
                countDownScript = (countdownLight)crossingLightObject.GetComponent("countdownLight");

            //Get references to Laser projection and Laser image if scene 2
            carLaserProjection = GameObject.FindGameObjectWithTag("CarLaserProjection");
            rectangleLaserContact = GameObject.FindGameObjectWithTag("RectangleLaserContact");

            //Get references to Laser projection if scene 3
            wcLaserProjection = GameObject.FindGameObjectWithTag("WCLaserProjection");

            //Get references to audio scene object if scene4
            audioScene = GameObject.FindGameObjectWithTag("AudioText");

            ResetLaser();


            GameObject DebugObject = GameObject.FindGameObjectWithTag("Debug");
            DebugText = (TextMesh)DebugObject.GetComponent("TextMesh");

            // get the car controller
            m_Car = GetComponent<CarController>();
            
            //Initialize values
            steering = 0.0f;
            accel = 0.0f;
            footbrake = 0.0f;
            handbrake = 0.0f;

            //DebugText.text = debugMessage;
        }


        private void FixedUpdate()
        {
            debugMessage = "Drive = " + drive;
            DebugText.text = debugMessage;
            //SetLaserVisibility(false);
            if (drive)
            {
                debugMessage = "Time: " + Time.time + "  Trigger: " + (startTime + interactionTriggerPoint);
                DebugText.text = debugMessage;

                m_Car.setTopSpeed(10);
                accel = 1.0f;
            }
            else
            {
                if(Time.time > (startTime+interactionTriggerPoint))
                {
                    if (carLaserProjection != null && flag == 1)
                    {
                        SetLaserVisibility(true);
                        //SetCrosswalkLight(1); // making the streetlight green alongwith the laser projection
                    }

                    if (BLEContainerScript != null && flag == 1 && !hapticsTriggered)
                    {
                        BLEContainerScript.TriggerHaptics();
                        hapticsTriggered = true;
                    }


                    debugMessage = "Time: " + Time.time + "  Trigger: " + (startTime + interactionTriggerPoint);
                    DebugText.text = debugMessage;

                    accel = 0.0f;

                    //Top speed starts at 10 and decreases by this amount per frame (fast stop: 0.5f, medium stop:0.1f)
                    //Note: You may need to adjust the "interactionTriggerPoint" variable to make the car stop before the crosswalk
                    m_Car.decreaseTopSpeed(0.1f);

                }

                //Added by Asha to restart the car again after stopping in crosswalk
                if (m_Car.m_Topspeed == 0 && flag==1)
                {
                    if (countDownScript != null && crosswalkCounterFlag == 1) //Delay the countdown until the vehicle comes to a stop
                    {
                            countDownScript.triggerCountdown();
                            crosswalkCounterFlag = 2;
                    }


                    ResetLaser();
                    SetLaserVisibility(true);
                    SetCrosswalkLight(1);   //Once car has come to a stop, set crosswalk light to green

                    // making text visible when the vehicle has stopped in front of the crosswalk
                    if (audioScene != null && flag == 1)
                    {
                        p.SetActive(true);
                        if(a!=null && b!=null)
                        {
                            b.SetActive(false);
                            a.SetActive(true);
                        }
                        
                    }

                    Invoke("ResetBulb", 9); //calling this function to blink the red streetlight

                    if (carLaserProjection != null)
                        Invoke("RedLaserColour", 9);

                    //changed the timing
                    Invoke("audioReset", 12);  //new function for audio
                    Invoke("ResetLaser", 15);

                    if (countDownScript == null) //Have car start driving after 15 seconds exept for the countdown timer scenario
                        Invoke("carStart", 15);
                    else if(countDownScript.m_iCurrentLightIndex == 0 && countDownScript.m_flChangeLightTime >= 2) //If timer scenario, wait for timer to finish
                        carStart();
                }
            }
     
            m_Car.Move(steering, accel, footbrake, handbrake);
        }

        public void buttonInteraction()
        {
            if(crosswalkCounterFlag == 0) 
                crosswalkCounterFlag = 1;
            if(crosswalkCounterFlag == 2 && (countDownScript.m_flChangeLightTime-Time.time) >= 10) //If the button is pressed again after coming to a stop with the crosswalk timer
            {
                countDownScript.addTime(10);
                crosswalkCounterFlag = 3; //Stops the button from being pressed again
            }



            if (wcLaserProjection != null)
            {
                RedLaserColour();
                SetLaserVisibility(true);
            }

            if ((Time.time > (startTime + triggerAllowStart)) && 
               (Time.time < (startTime + triggerAllowEnd)))
            {
                interactWithVehicle();

                //audio scene
                if(audioScene!=null)
                {
                    
                        p.SetActive(false);
                        q.SetActive(true);
                    
                }
            }

            else
            {
                //Audio Scene
                if (audioScene != null)
                {
                    
                        p.SetActive(false);
                        q.SetActive(true);

                    if (a != null && b != null)
                    {
                        a.SetActive(false);
                        b.SetActive(true);
                    }


                }
            }
        }

        public void interactWithVehicle()
        {
            drive = false;
            flag = 1;
        }

        public void audioReset()      //Making the 3rd audio active when the car starts driving again from the crosswalk
        {
            if (audioScene!=null)
            {
                r.SetActive(true);
                if (a != null && b != null)
                {
                    b.SetActive(true);
                    a.SetActive(false);
                }
                  
            }
        }

        public void carStart()      //Making drive true again for restarting the car
        {
            if (lightStandBulb != null)
            {
                lightBulbScript.GetComponent<Renderer>().enabled = true;
                SetCrosswalkLight(0); //While car is driving, set crosswalk light to red (aka 0)

            }

            drive = true;
           
        }

        //Changes the colour of light between red (safeToCross = 0) and green (safeToCross = 1)
        public void SetCrosswalkLight(int safeToCross)
        {
            if (lightStandBulb != null)
            {
                lightBulbScript.SetCrosswalkLight(safeToCross);
            }
        }


        //Sets visibility of car laser projection
        public void SetLaserVisibility(bool enabled)
        {
            if ((carLaserProjection != null || wcLaserProjection != null) && rectangleLaserContact != null)
            {
                rectangleLaserContact.GetComponent<Renderer>().enabled = enabled;

                if (carLaserProjection != null)
                { }
                //carLaserProjection.GetComponent<Renderer>().enabled = enabled;
                else
                { }
                   // wcLaserProjection.GetComponent<Renderer>().enabled = enabled;
            }
        }

        //Sets colour of laser projection to red
        public void RedLaserColour()
        {
            if ((carLaserProjection != null || wcLaserProjection != null) && rectangleLaserContact != null)
            {
                rectangleLaserContact.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.4f, 0.0f, 0.5f));

                if (carLaserProjection != null)
                { }
                    //carLaserProjection.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.4f, 0.0f, 0.3f));
                else
                { }
                    //wcLaserProjection.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.4f, 0.0f, 0.3f));
            }
        }

        //Resets laser projection to invisible and green
        public void ResetLaser()
        {
            if ((carLaserProjection != null || wcLaserProjection != null) && rectangleLaserContact != null)
            {
                SetLaserVisibility(false);
                rectangleLaserContact.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.5f));

                if (carLaserProjection != null)
                    carLaserProjection.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.3f));
                else
                    wcLaserProjection.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.3f));
            }
            

        }

        public void ResetBulb()
        {
            SetCrosswalkLight(0);
            bool oddeven = Mathf.FloorToInt(Time.time) % 2 == 0;
            if (lightStandBulb != null)
            {
                lightBulbScript.GetComponent<Renderer>().enabled= oddeven;
            }

            // Enable renderer accordingly
            
        }

        public void reset()
        {
            drive = false;
            hapticsTriggered = false;
            flag = 0;
            crosswalkCounterFlag = 0;
            accel = 0.0f;
            m_Car.decreaseTopSpeed(0.5f);

            if (m_Car.m_Topspeed==0)
            {
                this.transform.position = new Vector3(4f, 0f, 33f);
                
                //Audio Scene
                if(audioScene!=null)
                {
                    
                        p.SetActive(false);
                        q.SetActive(false);
                        r.SetActive(false);

                    if(a!=null && b!=null)
                    {
                        a.SetActive(false);
                        b.SetActive(true);
                    }
                }
            }
        }
    }
}


