using QuikGraph;
namespace Trains;
public class Network
{
    public class Track(bool occupied)
    {
        public bool Occupied = occupied;
    }
    public static void Test()
    {
        var g = new AdjacencyGraph<Track, Edge<Track>>();
        
        var tracks = Tracks(11).ToArray();
        foreach (var t in tracks)
            g.AddVertex(t);


        var ef = new EdgeFactory<Track, Edge<Track>>((a, b) => new(a, b));
        var connections = Connections();
        foreach (var e in connections)
            AddEdgeBothDirections(e);

        var switches = Switches();
        
        foreach(var (index, (a, b, sw)) in switches )
            AddEdgeBothDirections(sw ? (index, a) : (index, b));


        var beforeA = g.ContainsEdge(tracks[5], tracks[6]);
        var beforeB = g.ContainsEdge(tracks[5], tracks[7]);
        
        Switch(5);

        var afterA = g.ContainsEdge(tracks[5], tracks[6]);
        var afterB = g.ContainsEdge(tracks[5], tracks[7]);


        var x = 5;
        
        
        return;

        void Switch(int i)
        {
            if (!switches.TryGetValue(i, out (int a, int b, bool sw) trip))
                throw new Exception("Switch does not exist");
            
            g.RemoveOutEdgeIf(tracks[i], _ => true);

            trip.sw = !trip.sw;

            AddEdgeBothDirections(trip.sw ? (i, trip.a) : (i, trip.b));
        }

        void AddEdgeBothDirections((int a, int b) t)
        {
            g.AddEdge(ef(tracks[t.a], tracks[t.b]));
            g.AddEdge(ef(tracks[t.b], tracks[t.a]));
        }
        IEnumerable<Track> Tracks(int num)
        {
            for(var i = 0; i < num; i++)
                yield return new Track(false);
        }

        Dictionary<int, (int a, int b, bool s)> Switches()
        {
            return new()
            {
                { 5, (6, 7, true) },
            };
        }

        (int, int)[] Connections()
        {
            return
            [
                (0, 1), (1, 2), (2, 3), (3, 4), (4, 5),
                
                // switch station at 5, default setting is 6
                
                (6, 8), (8, 10), 
                
                (7, 9), (9, 10)
            ];
        }
    }
}