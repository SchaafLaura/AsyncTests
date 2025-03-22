namespace Trains;

public class TrackSwitch : TrackPiece
{
    private TrackPiece? OtherNext { get; init; }
    public bool State { get; private set; }

    public override TrackPiece? ReleaseTrain(Train t)
    {
        if (Train != t)
            throw new Exception("Train was not here");
        Train = null;
        
        return State ? Next : OtherNext;
    }

    public void Switch()
    {
        State = !State;
    }
}