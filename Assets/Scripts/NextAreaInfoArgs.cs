using System;
public class NextAreaInfoArgs : EventArgs
{
    private int counter;
    private string[] mapInfos;

    public int Counter
    {
        get { return counter; }
    }
    public string[] MapInfos
    {
        get { return mapInfos; }
    }
    public NextAreaInfoArgs(int counter, string[] mapInfos)
    {
        this.counter = counter;
        this.mapInfos = mapInfos;
    }
}
