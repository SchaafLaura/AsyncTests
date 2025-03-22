namespace Trains;
public class TrackPiece
{
    protected TrackPiece? Next { get; init; }
    protected TrackPiece? Previous { get; init; }

    protected Train? Train;
    public bool Occupied => Train is not null; 
    
    public void RecieveTrain(Train t)
    {
        if (Occupied)
            throw new Exception("crash");
        Train = t;
    }

    public virtual TrackPiece? ReleaseTrain(Train t)
    {
        if (Train != t)
            throw new Exception("Train was not here");
        Train = null;
        return Next;
    }
}