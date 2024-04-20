using System;

public interface IQuestEventResponder
{
    void OnEventTrigger(string eventName, EventArgs args = null);
}
