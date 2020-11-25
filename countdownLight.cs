using UnityEngine;
using System.Collections;

public class countdownLight : MonoBehaviour
{
    public GameObject   []lights;
    public float        []lightDuration;

    public Material     []countdownTimerMaterials;
    public MeshRenderer countdownTimer = null;
    public int          Element_LightToCountdownOn = -1;
    public int          countdownTimerStartingIndex = 0;

    public int          startingLightIndex = 0;

    public int          m_iCurrentLightIndex;
    int                 m_iCurrentCountdownTimerIndex;
    public float        m_flChangeLightTime;
    float               m_flChangeCountdownNumberTime;
    float               m_flCountdownTimerDelta;     // time between changing each countdown number

	void Start ()
    {
	    m_iCurrentLightIndex = startingLightIndex;
        m_iCurrentCountdownTimerIndex = countdownTimerStartingIndex;

        if (countdownTimer != null)
        {
            countdownTimer.material = countdownTimerMaterials[m_iCurrentCountdownTimerIndex];
            countdownTimer.gameObject.SetActive( false );
        }

        m_flChangeLightTime =  Time.time + lightDuration[m_iCurrentLightIndex];
        

        if (Element_LightToCountdownOn == m_iCurrentLightIndex)
        {
            m_flCountdownTimerDelta = 1; //lightDuration[ m_iCurrentLightIndex ] / ( countdownTimerStartingIndex + 1);
            m_flChangeCountdownNumberTime = Time.time + m_flCountdownTimerDelta;
            m_iCurrentCountdownTimerIndex = countdownTimerStartingIndex;

            if (countdownTimer != null)
            {
                countdownTimer.material = countdownTimerMaterials[m_iCurrentCountdownTimerIndex];
                countdownTimer.gameObject.SetActive(true);
            }
        }

        SetCurrentLight();
	}

	void SetCurrentLight()
    {
        foreach (GameObject light in lights)
            light.SetActive(false);

        lights[ m_iCurrentLightIndex ].SetActive ( true );

        if (Element_LightToCountdownOn == m_iCurrentLightIndex)
            countdownTimer.gameObject.SetActive( true );
    }

    void UpdateCountdownTimer()
    {
        if (m_flChangeCountdownNumberTime <= Time.time)
        {
            m_flChangeCountdownNumberTime = Time.time + m_flCountdownTimerDelta;

            if (m_iCurrentCountdownTimerIndex > 0)
            {
                m_iCurrentCountdownTimerIndex--;
                countdownTimer.material = countdownTimerMaterials[ m_iCurrentCountdownTimerIndex ];
            }
        }        
    }

    //Added - Triggers the crosswalk signal
    public void triggerCountdown()
    {
        m_flChangeLightTime = 1;
    }

    //Added - Adds 10 seconds to the timer
    public void addTime(int duration)
    {
        m_iCurrentCountdownTimerIndex += duration;
        m_flChangeLightTime += duration;
    }


	void Update ()
    {
        // change to next light
        if (lights.Length > 1)
        {
            //Added - Stop hand will never end until triggerCountdown() is called
            if (m_iCurrentLightIndex == 0) 
            {
                m_flChangeLightTime += 1;
            }


            if (m_flChangeLightTime <= Time.time)
            {
                m_iCurrentLightIndex++;

                if (m_iCurrentLightIndex >= lights.Length)
                    m_iCurrentLightIndex = 0;

                SetCurrentLight();

                m_flChangeLightTime = Time.time + lightDuration[m_iCurrentLightIndex];

                if (Element_LightToCountdownOn == m_iCurrentLightIndex)
                {
                    m_flCountdownTimerDelta = lightDuration[m_iCurrentLightIndex] / (countdownTimerStartingIndex + 1);
                    m_flChangeCountdownNumberTime = Time.time + m_flCountdownTimerDelta;
                    m_iCurrentCountdownTimerIndex = countdownTimerStartingIndex;
                    countdownTimer.material = countdownTimerMaterials[m_iCurrentCountdownTimerIndex];
                    countdownTimer.gameObject.SetActive(true);
                }
                else
                {
                    if ( countdownTimer != null )
                        countdownTimer.gameObject.SetActive(false);
                }
            }
        }

        if ( Element_LightToCountdownOn == m_iCurrentLightIndex )
            UpdateCountdownTimer();
        	
	}
}
