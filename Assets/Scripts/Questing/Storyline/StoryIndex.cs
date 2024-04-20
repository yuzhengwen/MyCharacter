[System.Serializable]
public struct StoryIndex
{
    public int part;
    public int index;
    public StoryIndex(int part, int index)
    {
        this.part = part;
        this.index = index;
    }
    public static bool operator ==(StoryIndex a, StoryIndex b)
    {
        return a.part == b.part && a.index == b.index;
    }
    public static bool operator !=(StoryIndex a, StoryIndex b)
    {
        return a.part != b.part || a.index != b.index;
    }
    public static bool operator >(StoryIndex a, StoryIndex b)
    {
        return a.part > b.part || a.part == b.part && a.index > b.index;
    }
    public static bool operator <(StoryIndex a, StoryIndex b)
    {
        return a.part < b.part || a.part == b.part && a.index < b.index;
    }

    public override readonly bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override readonly string ToString()
    {
        return part + "." + index;
    }
    public static StoryIndex Zero => new StoryIndex(0, 0);
}