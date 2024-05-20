using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHolder : MonoBehaviour
{
    public Color[] visColors = new Color[5];
    public bool leftDisplay; // States whether the gameObject holds vis colours for the right (false) or left (true) displays
    public bool practiceMode; // states whether the gameObject holds vis colours for the test/study (false) or practice (true) modes.

    // Start is called before the first frame update
    void Start()
    {
        if (leftDisplay && practiceMode)
        {
            loadLeftPracticeColours();
        }
        else if (leftDisplay && !practiceMode)
        {
            loadLeftTestColours();
        }
        else if (!leftDisplay && practiceMode)
        {
            loadRightPracticeColours();
        }
        else if (!leftDisplay && !practiceMode)
        {
            loadRightTestColours();
        }
    }

    private void loadLeftTestColours()
    {
        // red green blue yellow white
        visColors[0] = new Color(0.585f, 0.0f, 0.215f, 1f); // F2
        visColors[1] = new Color(0.24f, 0.305f, 0.14f, 1f); // F9
        visColors[2] = new Color(0.235f, 0.26f, 0.56f, 1f); // F15
        visColors[3] = new Color(0.595f, 0.465f, 0.07f, 1f); // C6
        visColors[4] = new Color(0.48f, 0.45f, 0.51f, 1f); // C0
    }

    private void loadRightTestColours()
    {
        // red green blue yellow white
        visColors[0] = new Color(0.47f, 0.065f, 0.155f, 1f); // F2
        visColors[1] = new Color(0.01f, 0.29f, 0.1f, 1f); // F9
        visColors[2] = new Color(0f, 0.25f, 0.525f, 1f); // F15
        visColors[3] = new Color(0.5f, 0.445f, 0f, 1f); // C6
        visColors[4] = new Color(0.365f, 0.425f, 0.47f, 1f); // C0
    }

    private void loadLeftPracticeColours()
{
    // orange purple grey pink brown
    visColors[0] = new Color(0.858f, 0.462f, 0.173f, 1f); // E4
    visColors[1] = new Color(0.573f,0.213f,0.617f,1f); // G18
    visColors[2] = new Color(0.370f, 0.370f, 0.370f, 1f); // G0
    visColors[3] = new Color(0.994f, 0.693f, 0.800f, 1f); // C20
    visColors[4] = new Color(0.452f, 0.207f, 0.056f, 1f); // H4
}

private void loadRightPracticeColours()
{
    // orange purple grey pink brown
    visColors[0] = new Color(0.858f, 0.462f, 0.173f, 1f); // E4
    visColors[1] = new Color(0.573f, 0.213f, 0.617f, 1f); // G18
    visColors[2] = new Color(0.370f, 0.370f, 0.370f, 1f); // G0
    visColors[3] = new Color(0.994f, 0.693f, 0.800f, 1f); // C20
    visColors[4] = new Color(0.452f, 0.207f, 0.056f, 1f); // H4
}
    }
