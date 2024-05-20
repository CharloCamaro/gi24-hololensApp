using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using u2vis;
using UXF;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;

public class stimulusControllerDouble : MonoBehaviour
{
    private GameObject activeVisLeft,activeVisRight; // Visualizations shown in left and right eyes
    public GameObject axes; // Custom axes of the visualization

    public GameObject visualization;
    public GameObject calibrationManager;

    public GameObject testColorsLeft, testColorsRight; // Game objects containing the colours used in the practice phase of the study (for left and right eyes)
    public GameObject practiceColorsLeft, practiceColorsRight; // Game objects containing the colours used in the study (for left and right eyes)

    public GameObject testVisLeft, testVisRight; // Visualizations to be used in study (left and right eyes')
    public GameObject practiceVisLeft, practiceVisRight; // Visualizations to be used in the practice phase of the study (left and right eyes')

    public GameObject poseGuide; // Reference planar object used to verify used is position properly w.r.t. physical space.

    public int nbPoints = 40; // Number of points per visualization. Overwritten when loading UXF settings.
    public int nbTrials = 10; // Number of trials per block. Overwritten when loading UXF settings.

    public UXF.Session session;

    private GameObject activeColorsLeft, activeColorsRight; // Colours to be used in active visualizations

    // Start is called before the first frame update
    void Start()
    {

        activeColorsLeft = testColorsLeft;
        activeColorsRight = testColorsRight;

        activeVisLeft = testVisLeft;
        activeVisRight = testVisRight;

        HideVisComponents();
        HidePoseGuide();

        ResetVisPose();
        Invoke("HidePoseGuide", 0.5f);

        // Following line needs to be commented out at run time.
        //ShowVisComponents();

        InitializePoints();
    }

    // Update data to be shown in visualization (same in both eyes)
    public void ChangeData(int dataIndex)
    {
        activeVisLeft.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(dataIndex * nbPoints, (dataIndex + 1) * nbPoints);
        activeVisRight.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(dataIndex * nbPoints, (dataIndex + 1) * nbPoints);
    }

    // Changes colour of the marks in both eyes.
    public void ChangeVisColor(Color newColor)
    {

        activeVisLeft.GetComponent<Renderer>().material.SetColor("_Color", newColor);
        activeVisRight.GetComponent<Renderer>().material.SetColor("_Color", newColor);
    }

    // Initialize points in visualization with corrent amount of points.
    public void InitializePoints()
    {
        testVisLeft.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(0 * nbPoints, (0 + 1) * nbPoints);
        testVisRight.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(0 * nbPoints, (0 + 1) * nbPoints);
    }

    // Changes the colours of the visualization from the list of predefined colours used in study.
    public int ChangeColor(int colorInd)
    {
            activeVisLeft.GetComponent<Renderer>().material.SetColor("_Color", activeColorsLeft.GetComponent<ColorHolder>().visColors[colorInd]);
            activeVisRight.GetComponent<Renderer>().material.SetColor("_Color", activeColorsRight.GetComponent<ColorHolder>().visColors[colorInd]);
            return -1;
    }

    // Updates the session parameters from UXF settings
    public void UpdateParameterValues()
    {
        nbPoints = session.settings.GetInt("nb points");

        nbTrials = session.settings.GetInt("nb trials per block") * session.settings.GetInt("nb blocks");

        ResetVis();
    }

    // Resets the points in visualization to default.
    void ResetVis()
    {
        activeVisLeft.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(0, nbPoints);
        activeVisRight.GetComponent<GenericDataPresenter>().SetSelectedItemIndices(0, nbPoints);
    }

    // Updates the position of the visualization in space based on the user's direction (will be centered on their FoV). 
    public void ResetVisPose()
    {
        HideVisComponents();
        HidePoseGuide();

        visualization.GetComponent<RadialView>().enabled = true;

        Invoke("StopPoseAdjustments", 0.5f);

    }

    // Hides the reference plane to show where the visualization will be located.
    void StopPoseAdjustments()
    {
        visualization.GetComponent<RadialView>().enabled = false;
        if (!session.InTrial)
        {
            ShowPoseGuide();
        }
        else
        {
            ShowVisComponents();
        }
    }


    // Hides the visualization
    public void HideVisComponents()
    {
        axes.SetActive(false);
        activeVisLeft.SetActive(false);
        activeVisRight.SetActive(false);
    }

    // Shows the visualization
    public void ShowVisComponents()
    {
        axes.SetActive(true);
        activeVisLeft.SetActive(true);
        activeVisRight.SetActive(true);
    }

    // Hides the reference plane
    public void HidePoseGuide()
    {
        poseGuide.SetActive(false);
    }

    // Shows the reference plane
    public void ShowPoseGuide()
    {
        poseGuide.SetActive(true);
    }

    // Updates source of visualization colours (practice vs study modes)
    public void UpdateColorSource()
    {
        if (string.Equals(session.experimentName, "practice"))
        {
            activeColorsLeft = practiceColorsLeft;
            activeColorsRight = practiceColorsRight;
        }
        else
        {
            activeColorsLeft = testColorsLeft;
            activeColorsRight = testColorsRight;
        }
    }

    // Updates source of data to use in visualizations (practice vs study modes)
    public void UpdateDataSource()
    {
        if (string.Equals(session.experimentName, "practice"))
        {
            activeVisLeft = practiceVisLeft;
            activeVisRight = practiceVisRight;
        }
        else
        {
            activeVisLeft = testVisLeft;
            activeVisRight = testVisRight;
        }
    }
}
