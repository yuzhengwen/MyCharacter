using System;
using UnityEngine;

public class ObjectiveLocationReached : BaseObjective, IQuestEventResponder
{
    private readonly Vector3 location;
    private readonly Collider2D collider;

    public ObjectiveLocationReached(BaseQuest parent, GameObject go) : base(parent, "Reach Location")
    {
        collider = go.GetComponent<Collider2D>();
    }

    public void OnQuestEvent(string eventName, EventArgs args)
    {
        if (eventName == "PlayerEnterLocation")
        {
            if ((args as LocationArgs).collider == collider)
                Complete();
        }
    }
}
public class LocationArgs : EventArgs
{
    public Vector3 location;
    public Collider2D collider;
    public LocationArgs(Vector3 location)
    {
        this.location = location;
    }
    public LocationArgs(Collider2D collider)
    {
        this.collider = collider;
    }
}
