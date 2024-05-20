using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using u2vis;

public class calibrationController : MonoBehaviour
{

    public GameObject leftPlane; // FoV-sized planed shown on the left eye.
    public GameObject rightPlane; // FoV-sized planed shown on the right eye.
    public GameObject leftVis; // visualization (scatter plot) on the left eye.
    public GameObject rightVis; // visualization (scatter plot) on the right eye.
    public GameObject axes; // custom axes of the visualization
    public GameObject stimulusManager;

    private Dictionary<string, int> colorDict = new Dictionary<string, int>();
    private bool planeActive = true; // states whether a FoV-sized plane or a vis is shown

    // Start is called before the first frame update
    void Start()
    {
        stimulusManager.GetComponent<stimulusControllerDouble>().InitializePoints();
    }

    // Changes the colour of the stimulus on left/right display to a new colour
    public void ChangeColor(Color newColor, int displayInd)
    {
        if (displayInd == 1)
        {
            leftPlane.GetComponent<Renderer>().material.color = newColor;
        }
        else if (displayInd == 2)
        {
            rightPlane.GetComponent<Renderer>().material.color = newColor;
        }
    }


    // Changes the colour of all stimulus (both eyes) to an RGB triplet [0,1]
    public void ChangeColorVal(float R, float G, float B)
    {
        Debug.Log("R: " + R);
        Debug.Log("G: " + G);
        Debug.Log("B: " + B);

        Color newColor = new Color(R, G, B);
        leftPlane.GetComponent<Renderer>().material.color = newColor;
        rightPlane.GetComponent<Renderer>().material.color = newColor;


        stimulusManager.GetComponent<stimulusControllerDouble>().ChangeVisColor(newColor);
    }

    // Switches from a uniformly-coloured plane to a scatter plot
    public void SwitchStim()
    {
        if (planeActive)
        {
            planeActive = false;
            leftPlane.SetActive(false);
            rightPlane.SetActive(false);
            axes.SetActive(true);
            leftVis.SetActive(true);
            rightVis.SetActive(true);
        }
        else
        {
            planeActive = true;
            leftPlane.SetActive(true);
            rightPlane.SetActive(true);
            axes.SetActive(false);
            leftVis.SetActive(false);
            rightVis.SetActive(false);
        }
    }
}
