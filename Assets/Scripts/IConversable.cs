using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConversable
{
    void ProgressOption(int option);
    int GetOptionProgress(int option);
}