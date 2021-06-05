using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationMenu : Menu
{
    [SerializeField] protected Transform DialogAnchor;
    [SerializeField] protected GameObject DialogOptionPrefab;
    [SerializeField] protected GameObject InputOptionPrefab;

    protected List<OptionButton> DialogOptions;

    public virtual void DisplayDialog(string dialog)
    {

    }

    public override void OnOpen()
    {
        base.OnOpen();
        TimeManagement.Instance.SetTimeControl(TimeControl.Manual);
    }

    public override void OnClose()
    {
        base.OnClose();
        TimeManagement.Instance.SetTimeControl(TimeControl.Auto);
    }

    protected void StaggerOptions()
    {
        bool usesSmallButtons = DialogOptions.Count > 4;
        float spacing = usesSmallButtons ? 40 : 80;
        int yOffset = usesSmallButtons ? 15 * DialogOptions.Count : 30 * DialogOptions.Count;
        for (int i = 0; i < DialogOptions.Count; i++)
        {
            DialogOptions[i].transform.position = DialogAnchor.position + Vector3.down * spacing * i + Vector3.up * yOffset;

            float height = 50f;
            if (DialogOptions[i] is InputOptionButton)
            {
                height = 75f;
            }
            else if (DialogOptions.Count == 1)
            {
                height = 150f;
            }
            else if (DialogOptions.Count > 4)
            {
                height = 40f;
            }

            RectTransform rect = DialogOptions[i].transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
    }
}
