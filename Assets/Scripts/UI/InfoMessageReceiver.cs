using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMessageReceiver : MonoBehaviour
{
    static InfoMessageReceiver _instance;
    void Awake() { _instance = this; }

    [SerializeField] Text InfoMessageLabel;

    bool _showingTempMessage;
    string _currentInfoMessage;
    bool _infoMessageLock;

    private void Start()
    {
        _currentInfoMessage = "";
        _infoMessageLock = false;
        Reset();
    }

    public static void ReleaseLock()
    {
        _instance._infoMessageLock = false;
        _instance._currentInfoMessage = "";
        _instance.InfoMessageLabel.text = "";
    }

    public static void Send(string msg, float duration = 0, bool setLock = false)
    {
        if (setLock)
            _instance._infoMessageLock = true;

        if (duration == 0)
        {
            if (setLock || !_instance._infoMessageLock)
            {
                _instance._currentInfoMessage = msg;
                if (!_instance._showingTempMessage)
                    _instance.Reset();
            }
        }
        else
        {
            _instance.CancelInvoke("Reset");

            _instance.InfoMessageLabel.color = Color.yellow;
            _instance.InfoMessageLabel.text = msg;
            _instance._showingTempMessage = true;

            _instance.Invoke("Reset", duration);
        }
    }

    void Reset()
    {
        InfoMessageLabel.color = Color.white;
        InfoMessageLabel.text = _currentInfoMessage;
        _showingTempMessage = false;
    }
}
