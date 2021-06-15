using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : Menu
{
    [SerializeField] RectTransform Anchor;
    [SerializeField] GameObject EntryUIPrefab;

    QuestLog _log;
    List<LogEntryUI> _entries;

    public override void OnOpen()
    {
        _log = Party.Instance.QuestLog;
        _entries = new List<LogEntryUI>();

        float yOffset = 0;
        for (int i = 0; i < _log.Quests.Count; i++)
        {
            GameObject obj = Instantiate(EntryUIPrefab, Anchor);
            obj.transform.position = Anchor.position + Vector3.up * 150.0f + Vector3.down * yOffset;

            LogEntryUI UI = obj.GetComponent<LogEntryUI>();
            UI.Setup(_log.Quests[i].GetCurrentStageDescription());
            _entries.Add(UI);

            yOffset += UI.Height;
        }
    }

    public override void OnClose()
    {
        foreach(var entry in _entries)
        {
            Destroy(entry.gameObject);
        }
        _entries.Clear();
    }
}
