using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using NativeWebSocket;

public class WebsocketWithServer : MonoBehaviour
{
    public int portNumber = 3100; // Port number to be used to communicate with server
    public string clientHeader = "HL-"; // header of every message sent by the device. Server will only respond to specific headers.

    public Session session;

    public GameObject expBuilder; 
    public GameObject sessionManager;
    public GameObject blockManager;
    public GameObject trialManager;
    public GameObject stimulusManager;

    public GameObject calibrationManager;

    public bool hololensBuild = false; // Set to "true" when deploying app to device rather than testing it inside Unity.

    private WebSocket websocket;

    private bool parametersReceived; // States whether parameter settings have been received by the device and if they can be modified again.

    // Start is called before the first frame update
    async void Start()
    {
        parametersReceived = false;

        // Deployments only currently work when the server and all devices are on the same network (e.g., hotspot).
        if (hololensBuild)
        {
            websocket = new WebSocket("ws://192.168.137.1:" + portNumber.ToString());
        }
        else
        {
            websocket = new WebSocket("ws://localhost:" + portNumber.ToString());
        }

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            SendMessageToServer("CONNECTED");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // getting the message as a string

            var message = System.Text.Encoding.UTF8.GetString(bytes);
            var header = message.Substring(0, 2).ToUpper();
            // If request is valid, process it.
            if (ValidateHeader(header))
            {
                Debug.Log("Client ID: " + header + "; " + message.Remove(0, 3));

                ReadMessage(message.Remove(0, 3));
            }
        };

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    // Communicates session is ready
    public void SendSessionReadyMessage()
    {
        SendMessageToServer("SessionReady");
    }

    // Communicates trial is ready to begin
    public void SendTrialReadyMessage()
    {
        SendMessageToServer("TrialReady");
    }

    // Communicates block is ready to begin
    public void SendBlockReadyMessage()
    {
        SendMessageToServer("BlockReady");
    }

    // Communicates block is finished.
    public void SendBlockDoneMessage()
    {
        SendMessageToServer("BlockDone");
    }

    // Communicates start time of UXF session
    public void SendStartTimeSessMessage(string timestamp)
    {
        SendMessageToServer("StartTimeSess " + timestamp);
    }

    // Communicates start time of application
    public void SendStartTimeAppMessage(string timestamp)
    {
        SendMessageToServer("StartTimeApp " + timestamp);
    }

    // Communicates session is finished
    public void SendSessionDoneMessage()
    {
        SendMessageToServer("SessionDone");
    }

    // Communicates a new trial has begun
    public void SendTrialStartedMessage()
    {
        SendMessageToServer("TrialStarted");
    }

    // Communicates practice session is ready to begin
    public void SendPracticeReadyMessage()
    {
        SendMessageToServer("PracticeReady");
    }

    // Communicates custom messages
    public void SendMessageToServer(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            websocket.SendText(clientHeader + message);
        }
    }

    // Requests CSV data
    public void SendCsvDataMessage(string filename)
    {
        SendMessageToServer("ProvideCsvData " + filename);
    }

    // Communicates session results in CSV format.
    public void SendCsvResultsMessage(string data)
    {
        Debug.Log("Sending result: " + data);
        SendMessageToServer("ReceiveResults " + data);
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private void ReadMessage(string message)
    {
        message = message.ToUpper();

        // Responds to session-launch request
        if (String.Equals(message, "SESS_BEGIN"))
        {
            sessionManager.GetComponent<sessionController>().LaunchSession();

            SendMessageToServer("SESS_STARTED");
        }
        // Responds to session-end request
        else if (String.Equals(message, "SESS_END"))
        {
            sessionManager.GetComponent<sessionController>().EndSession();
        }
        // Responds to reset-vis-position request
        else if (String.Equals(message, "RESET_VIEW"))
        {
            stimulusManager.GetComponent<stimulusControllerDouble>().ResetVisPose();
        }
        // Responds to hide-vis request
        else if (String.Equals(message, "HIDE_VIS"))
        {
            stimulusManager.GetComponent<stimulusControllerDouble>().HideVisComponents();
        }
        // Responds to show-vis request
        else if (String.Equals(message, "SHOW_VIS"))
        {
            stimulusManager.GetComponent<stimulusControllerDouble>().ShowVisComponents();
        }
        // Responds to hide-ref-plane request
        else if (String.Equals(message, "HIDE_GUIDE"))
        {
            stimulusManager.GetComponent<stimulusControllerDouble>().HidePoseGuide();
        }
        // Responds to reception of CSV data
        else if (String.Equals(message.Substring(0, 1), "{"))
        {
            if (!parametersReceived)
            {
                Debug.Log("Parameters received!");

                var trimmedMessage = message.Remove(0, 1);
                trimmedMessage = trimmedMessage.Remove(trimmedMessage.Length - 1, 1).ToLower();
                char[] delimiterChars = { ':', ',', '\t', '\n' };
                string[] paramArray = trimmedMessage.Split(delimiterChars);

                char[] badChars = { '"' };
                for (int i = 0; i < paramArray.Length; i++)
                {
                    paramArray[i] = paramArray[i].Trim(badChars);
                }

                sessionManager.GetComponent<sessionController>().ReceiveSessionParameters(paramArray);
            }

        }
        // Responds to change-colour request
        else if (String.Equals(message.Substring(0, 8), "VALCOLOR"))
        {
            var trimmedMessage = message.Remove(0, 9).ToLower();
            var fixedTrimmedMessage = trimmedMessage.Replace("\r", "");
            char[] delimiterChars = { '\r', ',' };
            string[] colorData = fixedTrimmedMessage.Split(delimiterChars);

            calibrationManager.GetComponent<calibrationController>().ChangeColorVal((float)(Convert.ToDouble(colorData[0])), (float)(Convert.ToDouble(colorData[1])), (float)(Convert.ToDouble(colorData[2])));
        }
        // Responds to create-experiment request
        else if (String.Equals(message.Substring(0, 8), "CSV_DATA"))
        {
            var trimmedMessage = message.Remove(0, 9).ToLower();
            var fixedTrimmedMessage = trimmedMessage.Replace("\r", "");
            char[] delimiterChars = { '\r', '\n' };
            string[] trialSetupData = fixedTrimmedMessage.Split(delimiterChars);

            foreach (string line in trialSetupData)
            {
                Debug.Log(line);
            }

            expBuilder.GetComponent<CSVExperimentBuilder>().csvData = trialSetupData.Take(trialSetupData.Count() - 1).ToArray();

        }
        // Responds to setup-practice-session request
        else if (String.Equals(message, "SETUP_REQUEST_P"))
        {
            if (!parametersReceived)
            {
                SendMessageToServer("ProvideParametersP");
            }
            else
            {
                SendPracticeReadyMessage();
            }
        }
        // Responds to setup-study-session request
        else if (String.Equals(message, "SETUP_REQUEST_S"))
        {
            if (!parametersReceived)
            {
                SendMessageToServer("ProvideParametersS");
            }
            else
            {
                SendSessionReadyMessage();
            }
        }
        // Responds to start-block request
        else if (String.Equals(message, "BLOCK_BEGIN"))
        {
            blockManager.GetComponent<blockController>().BeginBlock();
        }
        // Responds to start-trial request
        else if (String.Equals(message, "TRIAL_BEGIN"))
        {
            trialManager.GetComponent<trialController>().StartTrial();
        }
        // Responds to end-trial request
        else if (String.Equals(message, "TRIAL_END"))
        {
            trialManager.GetComponent<trialController>().EndTrial();
        }
        // Responds to begin-calibration request
        else if (String.Equals(message, "CALIB_BEGIN"))
        {
            calibrationManager.SetActive(true);
        }
        /// Responds to end-calibration request
        else if (String.Equals(message, "CALIB_END"))
        {
            calibrationManager.SetActive(false);
        }
        // Responds to switch-calibration-stimulus request
        else if (String.Equals(message, "SWITCH_STIM"))
        {
            calibrationManager.GetComponent<calibrationController>().SwitchStim();
        }
    }

    // Validates request was sent by server.
    private bool ValidateHeader(string header)
    {
        return String.Equals(header, "SV");
    }

    // Locks parameters to make sure they cannot be modified again during a session
    public void LockParameters()
    {
        parametersReceived = true;
    }

    // Unlocks parameters to allow them to be modified outside a session
    public void UnlockParameters()
    {
        parametersReceived = false;
    }
}