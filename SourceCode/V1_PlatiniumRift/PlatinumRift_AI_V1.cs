using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {

        GameData game = GameData.Instance = new GameData();
        
        Awake();

        DebugLog("Awake game");
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        game.playerCount = int.Parse(inputs[0]); // the amount of players (2 to 4)
        game.myIndex = game.myId = int.Parse(inputs[1]); // my player ID (0, 1, 2 or 3)
        game.zoneCount = int.Parse(inputs[2]); // the amount of zones on the map
        game.linkCount = int.Parse(inputs[3]); // the amount of links between all zones
        
        for (int i = 0; i < game.zoneCount; i++)
        {
            Zone newZone = new Zone();   
            inputs = Console.ReadLine().Split(' ');
            newZone.id = int.Parse(inputs[0]); // this zone's ID (between 0 and zoneCount-1)
            newZone.income = int.Parse(inputs[1]); // the amount of Platinum this zone can provide per game turn
            Zone.Add(newZone.id, newZone);
        }
        for (int i = 0; i < game.linkCount; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Zone zone1 = Zone.Get(int.Parse(inputs[0]));
            Zone zone2 = Zone.Get(int.Parse(inputs[1]));
            zone1.AddLink(zone2);
            zone2.AddLink(zone1);
        }

        foreach (Zone z in Zone.GetZones())
            z.zoneType = Zone.GetZoneTypeWithoutBigResearch(z, z.linkedZone);

        DebugLog("Data initialized");
        
        int iFrame=0;
        // game loop
        while (true)
        {
            game.frame = iFrame;
            if (iFrame == 0)
            {
                DebugLog("Start game");
                Start();
            }
            DebugLog("Frame"+(iFrame));
            UpdateBefore();
 
            game.players[0].platinum = int.Parse(Console.ReadLine()); 
            for (int i = 0; i < Zone.Count; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                Zone zone = Zone.Get(int.Parse(inputs[0]));
                zone.SetArmiesState(int.Parse(inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]), int.Parse(inputs[4]), int.Parse(inputs[5]));
                if (iFrame == 0 && zone.data.ownerId != -1)
                {
                    SetPlayerOrigineZone(zone.data.ownerId,zone);
                    //game.map.SetZone(76, 76, zone);
                }
            }
            
            Update();
            iFrame++;
            //Console.WriteLine("WAIT"); // first line for movement commands, second line for POD purchase (see the protocol in the statement for details)
            //Console.WriteLine("1 73");
        }
    }

    private static void SetPlayerOrigineZone(int playerId, Zone zone)
    {
        if(playerId<0 || playerId>3)return;
        GameData.Instance.players[playerId].origine = zone;
    }

    private static void DebugLog(string message) { Console.Error.WriteLine(message); }
    private static string moves="";
    private static void MoveDrone(int droneNumber, int from, int to) 
    {
        bool first = moves.Length<=0;
        moves += String.Format("{0}{1} {2} {3}", first ? "" : " ", droneNumber, from, to);
    }

    private static string purchases = "";
    private static void BuyDrone(int droneNumber, int where)
    {
        bool first = purchases.Length <= 0;
        purchases += String.Format("{0}{1} {2}", first ? "" : " ", droneNumber, where);
    }
    
    
    private static void Awake()
    {
    }

    private static void Start()
    {
        //
    }

    private static void UpdateBefore()
    {
        moves = "";
        purchases = "";
    }
    private static void Update()
    {
        GameData game = GameData.Instance;
        BuyDrone((int)(game.players[0].platinum / 20f), 1);
        int myId = game.myId;
        int myIndex = game.myIndex;
        PlayerInfo player =game.players[myIndex];
        //player = MoveAllUnityEveryWhere(game, myId, myIndex, player);


        SendTurnInstruction();
    }

    private static PlayerInfo MoveAllUnityEveryWhere(GameData game, int myId, int myIndex, PlayerInfo player)
    {
        foreach (Zone zone in Zone.GetZones())
        {
            //if enemy near, all zone around, move minion on base
            if (zone.GetFirstEnemyAround() != null)
            {

            }

            if (zone == player.origine && game.frame > 8) //should be replace by when income production >15 // or if player enemy next of based
            {
                if (zone.GetMyDrones(myIndex) < 4f)
                    continue;
            }

            int myDronesInZone = zone.GetMyDrones(myIndex);
            if (myDronesInZone > 0) zone.data.myMark++;
            Zone to;


            to = zone.GetNextIncomeZone(myIndex);
            if (to != null && myDronesInZone > 0)
            {
                MoveDrone(1, zone.id, to.id);
                myDronesInZone--;
            }
            //Prioirté aux zone neutre
            to = zone.GetNextNeutralZone();
            if (to != null && myDronesInZone > 0)
            {
                MoveDrone(1, zone.id, to.id);
                myDronesInZone--;
            }
            //Prioirté aux zone neutre
            to = zone.GetNextEnemyZone(myId);
            if (to != null && myDronesInZone > 0)
            {
                MoveDrone(1, zone.id, to.id);
                myDronesInZone--;
            }
            //Ensuite les zone enemy
            to = zone.GetLessMarkZone();
            if (to != null && myDronesInZone > 1)
            {
                int part = myDronesInZone * 2 / 3;
                MoveDrone(1 + part, zone.id, to.id);
                myDronesInZone -= 1 + part;
            }
            //Je répartie l'escade sur toutes les zones autours
            List<Zone> alreadyOneOn = new List<Zone>();
            for (int i = 0; i < zone.linkedZone.Count; i++)
            {
                to = zone.GetRandomLinkedZone();
                if (to != null && myDronesInZone > 0 && !alreadyOneOn.Contains(to))
                {
                    alreadyOneOn.Add(zone);
                    MoveDrone(1, zone.id, to.id);
                    myDronesInZone--;
                }
            }
            if (myDronesInZone > 0)
            {

                to = zone.GetNextEnemyZone(myIndex);
                if (to == null) to = zone.GetLessMarkZone();
                if (to == null) to = zone.GetRandomLinkedZone();
                MoveDrone(myDronesInZone, zone.id, to.id);
            }
        }
        return player;
    }

    private static void SendTurnInstruction()
    {
        ///END FRAME
        //Move
        if (moves.Length <= 0) Console.WriteLine("WAIT");
        else Console.WriteLine(moves);
        //Buy
        if (purchases.Length <= 0)
            Console.WriteLine("WAIT");
        else Console.WriteLine(purchases);
    }

}

public struct PlayerInfo 
{
    public int id;
    public int platinum;
    public Zone origine;
}
public class GameData {
    public static GameData Instance;
    public  HexagonalMap<Zone> map =new HexagonalMap<Zone>();
    public int playerCount=2;
    public int myId ; // my player ID (0, 1, 2 or 3)
    public int myIndex;
    public int zoneCount ; // the amount of zones on the map
    public int linkCount; // the amount of links between all zones

    public PlayerInfo[] players = new PlayerInfo[4];
    public int frame;
    
}



public interface IZoneData {

}
public class ZoneData :IZoneData
{
   public int ownerId; // the player who owns this zone (-1 otherwise)
   public int podsP0; // player 0's PODs on this zone
   public int podsP1; // player 1's PODs on this zone
   public int podsP2; // player 2's PODs on this zone (always 0 for a two player game)
   public int podsP3; // player 3's PODs on this zone (always 0 for a two or three player game)

   public int enemyMark;
   public int myMark;

}





public class Zone 
{
    public int id;
    public int income = 0;
    public ZoneType zoneType = ZoneType.Unknown;
    public ZoneData data = new ZoneData();

    public List<Zone> linkedZone = new List<Zone>();

    public delegate void OnZoneConquer(int oldOwner, int newOWner);
    public OnZoneConquer onNewOwnerDetected;

    public delegate void OnArmyMove(int playerIndex, Zone zone, int oldDroneNumber, int newDroneNumber);
    public OnArmyMove onArmyMove;

    public void AddLink(Zone zone) {

        linkedZone.Add(zone);
    }


    public void SetArmiesState(int newOwner, int dronePlayer0, int dronePlayer1, int dronePlayer2, int dronePlayer3)
    {

        int oldOwner = data.ownerId;
        if (onNewOwnerDetected != null && oldOwner != newOwner)
        {
            onNewOwnerDetected(oldOwner, newOwner);
        }
        data.ownerId = newOwner;


        //Player 1;
        if (onArmyMove != null)
        {
            if (data.podsP0 != dronePlayer0)
                onArmyMove(0, this, data.podsP0, dronePlayer0);
        }
        data.podsP0 = dronePlayer0;
        //Player 2;
        if (onArmyMove != null)
        {
            if (data.podsP1 != dronePlayer1)
                onArmyMove(1, this, data.podsP1, dronePlayer1);
        }
        data.podsP1 = dronePlayer1;
        //Player 3;
        if (onArmyMove != null)
        {
            if (data.podsP2 != dronePlayer2)
                onArmyMove(2, this, data.podsP2, dronePlayer2);
        }
        data.podsP2 = dronePlayer2;
        //Player 4;
        if (onArmyMove != null)
        {
            if (data.podsP3 != dronePlayer3)
                onArmyMove(3, this, data.podsP3, dronePlayer3);
        }
        data.podsP3 = dronePlayer3;
    }







    public static int Count { get { return zones.Count; } }
    public static Dictionary<int, Zone> zones = new Dictionary<int, Zone>();
    public static void Add(int id, Zone zone)
    {
        zones.Add(id, zone);
    }
    public static Zone Get(int id)
    {
        Zone val;
        zones.TryGetValue(id, out val);
        return val;
    }


    public static Zone [] GetZones()
    {
        Zone[] array = new Zone[zones.Values.Count];
        zones.Values.CopyTo(array, 0);
        return array;
    }

    public int GetMyDrones(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return data.podsP0;
            case 1: return data.podsP1;
            case 2: return data.podsP2;
            case 3: return data.podsP3; 
        }
        return 0;
    }
    public int GetEnemiesDrone(int playerIndex)
    {
        int num = 0;
        if (playerIndex != 0) num += data.podsP0;
        if (playerIndex != 1) num += data.podsP1;
        if (playerIndex != 2) num += data.podsP2;
        if (playerIndex != 3) num += data.podsP3;
        return num;
    }

    private Random rand = new Random(DateTime.Now.Millisecond);
    public Zone GetRandomLinkedZone()
    {
        rand = new Random(DateTime.Now.Millisecond);
        int num = rand.Next(0, linkedZone.Count);
        return linkedZone[num];
    }
    public Zone GetNextNeutralZone()
    {
        foreach (Zone z in linkedZone)
            if (z.data.ownerId == -1)
                return z;
        return null;
    }
    public Zone GetNextEnemyZone(int myId)
    {
        foreach (Zone z in linkedZone)
            if (z.data.ownerId != myId && z.data.ownerId!=-1)
                return z;
        return null;
    }

    internal Zone GetNextIncomeZone(int userId)
    {
        foreach (Zone z in linkedZone)
            if (z.income>0 && z.data.ownerId !=userId)
                return z;
        return null;
    }
    public Zone GetLessMarkZone()
    {
        int mark = int.MaxValue;
        Zone zone = null;
        foreach (Zone z in linkedZone) {
            if (z.data.myMark < mark) 
            {
                zone = z; mark = z.data.myMark;
            }
        }
        return zone;
    }

    public Zone GetFirstEnemyAround()
    {

        foreach (Zone z in linkedZone)
            if (z.GetEnemiesDrone(GameData.Instance.myId) > 0)
                return z;
        return null;
    }


    /// <summary>
    /// Plain = 6 link -- na
    /// DeadEnd = 1 link --
    /// Path = 2 link -- not connected
    /// Edge = 2 link -- connected
    /// OneSquareBorder = 5 link 
    /// TwoSquareBorder = 4 other in a linkedlist
    /// Border = 3 other in a linkedlist
    /// </summary>
    public enum ZoneType { Unknown, Plain, DeadEnd, Path, PathStart, Edge, /*OneSquareBorder, TwoSquareBorder,*/ Border, Junction }

    public static ZoneType GetZoneTypeWithoutBigResearch(Zone zone, List<Zone> linkedZone) {
        if (linkedZone.Count == 6) return ZoneType.Plain;
        if (linkedZone.Count == 1) return ZoneType.DeadEnd;
        if (linkedZone.Count == 2)
        {
            if (linkedZone[0].IsLinkedTo(linkedZone[1]))
                return ZoneType.DeadEnd;
            else return ZoneType.Edge;
        }
        if (linkedZone.Count == 3)
        {
            Zone z1, z2, z3;
            z1 = linkedZone[0];
            z2 = linkedZone[1];
            z3 = linkedZone[2];
            if (z1.IsLinkedTo(z2) || z1.IsLinkedTo(z3) || z2.IsLinkedTo(z1) || z1.IsLinkedTo(z3) || z3.IsLinkedTo(z1) || z3.IsLinkedTo(z2))
                return ZoneType.Junction;
        }

        return ZoneType.Unknown;
    }


    private bool IsLinkedTo(Zone zone)
    {
       return linkedZone.Contains(zone);
    }




}



public class Squad 
{

    public int droneNumber;
    public int startDroneNumber;
    public Zone startZone;
    public Zone currentPosition;
    public Zone lastPosition;
    public Queue<Zone> destinationPath = new Queue<Zone>();

   

    public delegate void Behaviour(Squad escade);
    private Behaviour behaviour;


    public Squad(int number, Zone startZone) {
        currentPosition = this.startZone = startZone;
        droneNumber = startDroneNumber = number;
        Squad.squads.Add(this);
    }
    void Destroy() {
        destinationPath.Clear();
        behaviour = null;
        Squad.squads.Remove(this);
        
    }
    public void SetBehaviour(Behaviour beh) { behaviour = beh; }
    public void AddZoneToDestination(Zone zone) 
    {
        if (zone != null)
            destinationPath.Enqueue(zone);
    }
    public void CancelDestination() { destinationPath.Clear(); }

    
    public void DoTheThing() 
    {
        if (behaviour != null)
            behaviour(this);
        if (droneNumber<=0)
            Destroy();

    }
    public static List<Squad> squads = new List<Squad>();
}


public class HexagonalMap <T>
{
    // X, Y, ID <=> [][][]
    public T[,] map = new T[151, 151];
    public enum HexagoneSide {LeftUp, Up,RightUp,RightDown, Down,LeftDown}

    public T GetDataAt(int x, int y)
    {
        return map[x, y];
    }
    public T GetDataAt(int x, int y, HexagoneSide atSide)
    {
        MoveFromAt(ref x, ref y, atSide);
        return map[x, y];
    }
    public bool IsOutOfArray(int x, int y)
    {
        return x < 0 || x >= 151 || y < 0 || y >= 151;
    }
    public bool IsOutOfArray(int x, int y, HexagoneSide atSide)
    {
        MoveFromAt(ref x, ref y, atSide);
        return x < 0 || x >= 151 || y < 0 || y >= 151;
    }

    public void MoveFromAt(ref int x, ref int y, HexagoneSide atSide)
    {
        switch (atSide)
        {
            case HexagoneSide.LeftUp: y--; return;
            case HexagoneSide.Up: x--; return;
            case HexagoneSide.RightUp: y++; return;

            case HexagoneSide.RightDown: x++; y++; return;
            case HexagoneSide.Down: x++; return;
            case HexagoneSide.LeftDown: x++; y--; return;
        }
        return;
    }

    public void SetZone(int x, int y, T zone)
    {
        if (zone != null && ! IsOutOfArray(x,y))
            map[x, y] = zone;
    }
}