using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : Menu
{
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] RectTransform Anchor;
    [SerializeField] GameObject EntryUIPrefab;

    QuestLog _log;
    List<LogEntryUI> _entries;

    List<int> _offsets;
    bool _isFinalPage;

    public override void OnOpen()
    {
        _log = Party.Instance.QuestLog;
        _entries = new List<LogEntryUI>();
        _offsets = new List<int>() { 0 };

        SetupPage(0);
    }

    void SetupPage(int offset)
    {
        foreach (var entry in _entries)
        {
            Destroy(entry.gameObject);
        }
        _entries.Clear();

        _isFinalPage = false;

        int currentOffset = _offsets[_offsets.Count - 1];
        PrevButton.SetActive(currentOffset > 0);

        int index = 0;
        float yOffset = 0f;
        while (true)
        {
            if(index + currentOffset >= _log.Quests.Count)
            {
                break;
            }

            GameObject obj = Instantiate(EntryUIPrefab, Anchor);
            LogEntryUI UI = obj.GetComponent<LogEntryUI>();
            UI.Setup(_log.Quests[index + currentOffset].GetCurrentStageDescription());

            yOffset += UI.Height + 30;

            obj.transform.position = Anchor.position + Vector3.down * yOffset;
            if (yOffset >= Anchor.sizeDelta.y)
            {
                Destroy(obj);
                _offsets.Add(currentOffset + index);
                NextButton.SetActive(true);
                return;
            }

            if (_entries.Count > 0)
                _entries[_entries.Count - 1].ShowSeperator();

            _entries.Add(UI);

            index++;
        }

        _isFinalPage = true;
        NextButton.SetActive(false);
    }

    public void Previous()
    {
        if(!_isFinalPage)
            _offsets.RemoveAt(_offsets.Count - 1);
        _offsets.RemoveAt(_offsets.Count - 1);
        SetupPage(_offsets[_offsets.Count - 1]);
    }

    public void Next()
    {
        SetupPage(_offsets[_offsets.Count - 1]);
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
