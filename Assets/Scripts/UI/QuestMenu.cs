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
    [SerializeField] GameObject SeperatorPrefab;

    QuestLog _log;
    List<LogEntryUI> _entries;
    List<GameObject> _dividers;

    List<int> _offsets;
    bool _isFinalPage;

    public override void OnOpen()
    {
        _log = Party.Instance.QuestLog;
        _entries = new List<LogEntryUI>();
        _dividers = new List<GameObject>();
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

        foreach (var div in _dividers)
        {
            Destroy(div);
        }
        _dividers.Clear();


        _isFinalPage = false;

        int currentOffset = _offsets[_offsets.Count - 1];
        PrevButton.SetActive(currentOffset > 0);

        int index = 0;
        float yOffset = 0f;
        while (true)
        {
            if (index + currentOffset >= _log.Quests.Count)
            {
                break;
            }

            GameObject obj = Instantiate(EntryUIPrefab, Anchor);
            LogEntryUI UI = obj.GetComponent<LogEntryUI>();
            UI.Setup(_log.Quests[index + currentOffset].GetCurrentStageDescription());

            bool outOfSpace = yOffset + UI.Height / 2f + 25 >= Anchor.sizeDelta.y;
            if (index > 0 && !outOfSpace)
            {
                GameObject divider = Instantiate(SeperatorPrefab, Anchor);
                divider.transform.position = Anchor.position + Vector3.down * yOffset;
                _dividers.Add(divider);
            }

            yOffset += UI.Height / 2f + 25;
            obj.transform.position = Anchor.position + Vector3.down * yOffset;
            if (outOfSpace)
            {
                Destroy(obj);
                _offsets.Add(currentOffset + index);
                NextButton.SetActive(true);
                return;
            }

            _entries.Add(UI);

            yOffset += UI.Height / 2f + 25;
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
