using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using u2vis;
using UXF;

public class trialController : MonoBehaviour
{
    public GameObject stimulusManager;

    public UXF.Session session;

    public GameObject comm;

    private Dictionary<string, int> colorDict = new Dictionary<string, int>(); // Dictionary used to map colour-names to indices
    private int colorInd; // index of the colour. Not used for anything as of data collection.

    // Start is called before the first frame update
    void Start()
    {
        // real colours
        colorDict.Add("red", 0);
        colorDict.Add("green", 1);
        colorDict.Add("blue", 2);
        colorDict.Add("yellow", 3);
        colorDict.Add("white", 4);
        colorDict.Add("random", 5);

        // practice colours
        colorDict.Add("orange", 0);
        colorDict.Add("purple", 1);
        colorDict.Add("grey", 2);
        colorDict.Add("pink", 3);
        colorDict.Add("brown", 4);
    }

    // Starts the next trial of the study
    public void StartTrial()
    {
        ShowStimulus();

        session.BeginNextTrial();

        comm.GetComponent<WebsocketWithServer>().SendTrialStartedMessage();
    }

    // End the current trial of the study
    public void EndTrial()
    {
        session.CurrentTrial.result["colorInd"] = colorInd;
        session.EndCurrentTrial();
        
        HideStimulus();
    }

    // Prepares the next trial (e.g., update data and vis colour)
    public void PrepareNextTrial()
    {
        UpdateStimulus();

        comm.GetComponent<WebsocketWithServer>().SendTrialReadyMessage();
    }

    // Hides the visualization
    public void HideStimulus()
    {
        stimulusManager.GetComponent<stimulusControllerDouble>().HideVisComponents();
    }

    // Shows the visualization
    public void ShowStimulus()
    {
        stimulusManager.GetComponent<stimulusControllerDouble>().ShowVisComponents();
    }

    // Updates the stimulus (vis data & colour)
    public void UpdateStimulus()
    {
        var trialColor = session.NextTrial.settings.GetString("color");
        var trialData = session.NextTrial.settings.GetInt("vis_index");

        colorInd = stimulusManager.GetComponent<stimulusControllerDouble>().ChangeColor(colorDict[trialColor]);

        stimulusManager.GetComponent<stimulusControllerDouble>().ChangeData(trialData);
    }
}
