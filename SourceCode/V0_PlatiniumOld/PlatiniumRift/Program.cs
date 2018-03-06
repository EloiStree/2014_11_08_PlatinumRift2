using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

//R:  droneNumber  purchases  BehaviorStorage  FlagSeekerBehavior  PrioritySeekerBehavior  GetZones  linkedZone  mapName  zoneNumber  markedZone  IsItThisMap  TimeStartTurn  TimeSinceStart  platinum  myIndex  pctUsed  GetMyDrones  ownerId  priorityMark  origine  myDronesInZone  GetRandomLinkedZone      newZone  squadsToMove  GetUnityGroup  littleGroup  bigGroup  allGroup  SetBehaviour  DoTheThing  nextPosition  currentPosition  ShortestWayNode  enemyFlag  attackZone  GetNextZone  Behaviour    playerId  _startTime  oldOwner  playerIndex  onArmyMove  newOwner  dronePlayer0  dronePlayer1  visible    description  startZone  behaviour  GoToEnemyBased  currentList  nextList  leavesList  current  linkToCurrent  directions  minCost  existingZone  friendZoneId
//R: MyMath GetPourcentControled
//R:   Player  GameData   DebugLog   MoveDrone   InspectorInfo  FixedMap  MapZone  PlayerInfo  NodeZone  MapStat  MapArea
//R: ZoneType Unknown Plain DeadEnd Path PathStart Edge OneSquareBorder TwoSquareBorder Border Junction
//R: Instance playerCount zoneCount linkCount  players myFlagZone enemyFlagZone enemyDistance
//R:   moves  first   found  zones  player  frame  myMark  income   inputs    Squad  alone   links  start  podsP0  podsP1   escade  target  leaves  isOut  zoneId  center

class Player
{
    
    public static GameData game;
    static void Main(string[] args)
    {

        GameData.Instance = game = new GameData(DateTime.Now.Ticks);
        InitGameAndZoneInfo();

        while (true)
        {
            InitTurnInfoAndCallStartUpdate();
        }
    }


    #region Convert and communication with Server
    public static void DebugLog(string message) { Console.Error.WriteLine(message); }
    private static string moves = "";
    private static void MoveDrone(int droneNumber, int from, int to)
    {
        bool first = moves.Length <= 0;
        moves += String.Format("{0}{1} {2} {3}", first ? "" : " ", droneNumber, from, to);
    }
    private static string purchases = "";
    private static void BuyDrone(int droneNumber, int where)
    {
        bool first = purchases.Length <= 0;
        purchases += String.Format("{0}{1} {2}", first ? "" : " ", droneNumber, where);
    }

    private static void SendTurnInstruction()
    {
        if (moves.Length <= 0) Console.WriteLine("WAIT");
        else Console.WriteLine(moves);
        if (purchases.Length <= 0)
            Console.WriteLine("WAIT");
        else Console.WriteLine(purchases);
    }
    #endregion
    #region Awake, start, Update
    private static void Awake()
    {

        InspectorInfo.incomeSeeked = BehaviorStorage.IncomeSeekerBehavior; // go take station
        InspectorInfo.enemyIncomeSeeked = BehaviorStorage.EnemyIncomeSeekerBehavior; // go take station next to the enemy station
        InspectorInfo.neutralSeeked = BehaviorStorage.NeutralSeekerBehavior; // go take neutral zone
        InspectorInfo.defenderSeeked = BehaviorStorage.DefenderSeekerBehavior; //go take the territory we lost
        InspectorInfo.flagSeeker = BehaviorStorage.FlagSeekerBehavior;
        InspectorInfo.prioritySeeker = BehaviorStorage.PrioritySeekerBehavior;
    }

    private static void Start()
    {
        foreach (Zone z in Zone.GetZones())
            z.zoneType = Zone.GetZoneTypeWithoutBigResearch(z, z.linkedZone);

        bool found;
        FixedMap map = CreateAndCheck_MapRegister(out found);
        game.mapName = map.mapName;
        DebugLog(found ? game.mapName : "No fixed map found");
        if (game.mapName == "SS")
        {
            game.mapStrat = new Map_SS(map);

        }

    }

    private static FixedMap CreateAndCheck_MapRegister(out bool found)
    {
        //Init map
        found = false;
        string name = "";
        List<FixedMap> maps = new List<FixedMap>();
        FixedMap map;
        // Create map
        map = new FixedMap()
        {
            mapName = "TheCross",
            zoneNumber = 149,
            markedZone = new MapZone[] { new MapZone(94, new int[] { 93, 100, 101, 95, 87, 86 }), new MapZone(55, new int[] { 48, 49, 56, 54, 62, 63 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }
        map = new FixedMap()
        {
            mapName = "Black Hole",
            zoneNumber = 367,
            markedZone = new MapZone[] { new MapZone(226, new int[] { 225, 239, 240, 227, 216, 215 }), new MapZone(141, new int[] { 140, 151, 152, 127, 128, 142 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }
        map = new FixedMap()
        {
            mapName = "Square",
            zoneNumber = 199,
            markedZone = new MapZone[] { new MapZone(188, new int[] { 197, 198, 189, 187, 177, 178 }), new MapZone(93, new int[] { 92, 94, 103, 104, 83, 84 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }

        map = new FixedMap()
        {
            mapName = "PR2",
            zoneNumber = 405,
            markedZone = new MapZone[] { new MapZone(305, new int[] { 295, 296, 306, 304, 316, 317 }), new MapZone(100, new int[] { 99, 101, 88, 89, 109, 110 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }
        map = new FixedMap()
        {
            mapName = "Leaf",
            zoneNumber = 159,
            markedZone = new MapZone[] { new MapZone(131, new int[] { 130, 121, 122, 138, 139 }), new MapZone(51, new int[] { 43, 44, 52, 61, 60 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }

        map = new FixedMap()
        {
            mapName = "SS",
            zoneNumber = 23,
            markedZone = new MapZone[] { new MapZone(15, new int[] { 13, 14, 16, 18, 19 }), new MapZone(8, new int[] { 5, 4, 7, 9, 10 }) }
        };
        //Check it is this one
        if (map.IsItThisMap(ref Zone.zones, out name))
        {
            found = true;
            return map;
        }
        //None was found
        return map;
    }

    static void UpdateBefore()
    {
        moves = "";
        purchases = "";
        ZoneProcessing();
        game.TimeStartTurn = game.TimeSinceStart;
    }
    static void UpdateEnd()
    {
        DebugLogStateGame_EndUpdate();
    }
    static void Update()
    {

        DebugLogStateGame_StartUpdate();
        GameData game = GameData.Instance;
        BuyDrone((int)(game.players[0].platinum / 20f), 1);
        int myId = game.myId;
        int myIndex = game.myIndex;
        PlayerInfo player = game.players[myIndex];
        //player = MoveAllUnityEveryWhere(game, myId, myIndex, player);
        if (game.enemyDistance > 6)
        {
            if (game.frame > 1)
                SquadMissionManagement();
            else DispertionMission();
        }
        else
        {
            Rush();
        }

        SendTurnInstruction();
    }

    private static void DebugLogStateGame_StartUpdate()
    {
        DebugLog(string.Format("Game state: (f:{0})  Time: {1}", game.frame, game.TimeSinceStart / 1000000000.0));
        DebugLog(string.Format("----------- Start-------------"));
        DebugLog(string.Format("Map: {0} ({1}) ", game.mapName, game.zoneCount));
        DebugLog(string.Format("Start id, Me: {0}  Enemy : {1}", game.myFlagZone.id, game.enemyFlagZone.id));
        DebugLog(string.Format("##  Battle State  ## "));
        DebugLog(string.Format("Pct conq. {0} % ", (int)(game.GetPourcentControled(true) * 100f)));

    }
    private static void DebugLogStateGame_EndUpdate()
    {
        DebugLog(string.Format("______________________________"));
        double pctUsed = (double)game.TimeStartTurn / (double)game.TurnMaxTime;
        pctUsed = MyMath.Clamp((int)(pctUsed * 100.0), 0, 100);

        DebugLog(string.Format("Time used: {0} ({1}%)", game.TimeSinceStartTurn / 1000000000.0, pctUsed));
        DebugLog(string.Format("-------------END--------------"));
    }




    private static void ZoneProcessing()
    {
        int myId = game.myId;

        game.EnemyZoneCount = 0;
        game.MyZoneCount = 0;
        game.NeutralZoneCount = 0;

        foreach (PlayerInfo p in game.players)
            p.globalIncome = 0;
        foreach (Zone z in Zone.GetZones())
        {
            if (z.IsOwnedByEnemy())
                game.EnemyZoneCount++;
            else if (z.IsOwnedByMe())
                game.MyZoneCount++;
            else game.NeutralZoneCount++;

            #region Mark and priority
            //Permet de savoir par où l'enemi et mes unitées passes majoritairement
            //Ne marche plus depuis le fog wars.
            if (z.GetEnemiesDrone(myId) > 0)
                z.data.enemyMark++;
            if (z.GetMyDrones(myId) > 0)
                z.data.myMark++;
            // Définir des prioritées sur les zone
            // Marche moins depuis le fog wars
            if (z.data.ownerId >= 0)
                game.players[z.data.ownerId].globalIncome += z.income;

            //Si le territoire m'appartient priorité est null
            if (z.data.ownerId == myId)
                z.data.priorityMark = 0;
            else if (IsEnemyStartPoint(z))
                z.data.priorityMark = 50;
            else
            {
                //J'ajoute l'income de la zone comme pririté
                z.data.priorityMark += 1 + z.income;
                //Je signale que les zone annexes sont prioirtaires
                foreach (Zone link in z.linkedZone)
                {
                    //Si l'income est sup à 3 la zone est étendue encore une fois
                    link.data.priorityMark += z.income > 3 ? 2 : 1;
                    if (z.income > 3)
                        foreach (Zone sublink in link.linkedZone)
                        {
                            sublink.data.priorityMark += 1;
                        }
                }

            }
            #endregion
        }
    }

    private static bool IsEnemyStartPoint(Zone zone)
    {
        if (zone == null) return false;
        if (zone.data.ownerId == game.myId) return false;
        foreach (PlayerInfo p in game.players)
            if (p.origine != null && p.origine.id == zone.id)
                return true;
        return false;
    }


    #endregion
    #region First IA Version
    private static PlayerInfo MoveAllUnityEveryWhere(GameData game, int myId, int myIndex, PlayerInfo player)
    {
        foreach (Zone zone in Zone.GetZones())
        {
            //if enemy near, all zone around, move minion on base
            if (zone.GetFirstEnemyAround() != null) { }

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

    #endregion
    #region InitData : Start Turn

    public static void InitGameAndZoneInfo()
    {
        Awake();
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        game.playerCount = int.Parse(inputs[0]);
        game.myIndex = game.myId = int.Parse(inputs[1]);
        game.zoneCount = int.Parse(inputs[2]);
        game.linkCount = int.Parse(inputs[3]);

        for (int i = 0; i < game.zoneCount; i++)
        {
            Zone newZone = new Zone();
            inputs = Console.ReadLine().Split(' ');
            newZone.id = int.Parse(inputs[0]);
            newZone.income = int.Parse(inputs[1]);
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

    }

    public static void InitTurnInfoAndCallStartUpdate()
    {
        GameData game = GameData.Instance;
        string[] inputs;
        int iFrame = game.frame;
        if (iFrame == 0)
        {
            Start();
        }
        UpdateBefore();
        game.players[0].platinum = int.Parse(Console.ReadLine());
        for (int i = 0; i < Zone.Count; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Zone zone = Zone.Get(int.Parse(inputs[0]));
            zone.SetArmiesState(int.Parse(inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]), int.Parse(inputs[4]), int.Parse(inputs[5]));
            if (iFrame == 0 && zone.data.ownerId != -1)
            {
                game.SetPlayerOrigineZone(zone.data.ownerId, zone);
            }
        }
        Update();
        UpdateEnd();
        game.frame++;
    }


    #endregion


    private static List<Squad> squadsToMove = new List<Squad>();
    private static void SquadMissionManagement()
    {

        GetUnityGroup(game.myId, ref alone, ref duo, ref  littleGroup, ref  bigGroup, ref  army, ref allGroup);
        squadsToMove.Clear();
        int i = 0;
        foreach (Zone z in alone)
        {
            Squad sq = new Squad(1, z);
            if (i % 3 == 0)
                //sq.SetBehaviour(BehaviorStorage.NeutralSeekerBehavior);
                sq.SetBehaviour(BehaviorStorage.PrioritySeekerBehavior);
            else
                //sq.SetBehaviour(BehaviorStorage.IncomeSeekerBehavior);
                sq.SetBehaviour(BehaviorStorage.PrioritySeekerBehavior);
            squadsToMove.Add(sq);
            i++;
        }
        i = 0;
        foreach (Zone z in duo)
        {
            Squad sq = new Squad(2, z);
            //sq.SetBehaviour(BehaviorStorage.EnemyIncomeSeekerBehavior);
            sq.SetBehaviour(BehaviorStorage.PrioritySeekerBehavior);
            squadsToMove.Add(sq);
            i++;
        }
        i = 0;
        foreach (Zone z in littleGroup)
        {

            Squad sq = new Squad(z.GetMyDrones(game.myId), z);
            if (i < 2 + (game.frame / 30))
                sq.SetBehaviour(BehaviorStorage.PrioritySeekerBehavior);
            else
                sq.SetBehaviour(BehaviorStorage.FlagSeekerBehavior);
            squadsToMove.Add(sq);
            i++;
        }
        i = 0;
        foreach (Zone z in bigGroup)
        {
            Squad sq = new Squad(z.GetMyDrones(game.myId), z);
            if (i < 1 + (game.frame / 100))
                sq.SetBehaviour(BehaviorStorage.PrioritySeekerBehavior);
            else
                sq.SetBehaviour(BehaviorStorage.FlagSeekerBehavior);
            squadsToMove.Add(sq);
            i++;
        }
        i = 0;
        foreach (Zone z in army)
        {
            Squad sq = new Squad(z.GetMyDrones(game.myId), z);
            sq.SetBehaviour(BehaviorStorage.FlagSeekerBehavior);
            squadsToMove.Add(sq);
            i++;
        }

        foreach (Squad s in squadsToMove)
        {
            s.DoTheThing();
            if (s.nextPosition != null)
            {
                MoveDrone(s.droneNumber, s.currentPosition.id, s.nextPosition.id);
            }
        }

    }
    private static void DispertionMission()
    {
        GetUnityGroup(game.myId, ref alone, ref duo, ref  littleGroup, ref  bigGroup, ref  army, ref allGroup);
        squadsToMove.Clear();

        foreach (Zone z in allGroup)
        {
            List<Zone> links = z.linkedZone;
            for (int i = 0, c = 0; i < z.GetMyDrones(game.myId); i++, c++)
            {
                c = c % (links.Count - 1);

                if (c > 0 && links.Count > 0)
                {
                    Squad sq = new Squad(1, z);
                    sq.SetBehaviour(BehaviorStorage.NoOrder);
                    sq.nextPosition = links[c];
                    squadsToMove.Add(sq);
                }
            }
        }

        foreach (Squad s in squadsToMove)
        {
            s.DoTheThing();
            if (s.nextPosition != null)
            {
                MoveDrone(s.droneNumber, s.currentPosition.id, s.nextPosition.id);
            }
        }
    }

    public static void Rush()
    {
        GetUnityGroup(game.myId, ref alone, ref duo, ref  littleGroup, ref  bigGroup, ref  army, ref allGroup);

        if (game.frame == 0)
        {
            Zone start = game.myFlagZone;
            ShortestWayNode swn = null;
            game.enemyFlag.TryGetValue(start.id, out swn);

            Zone attackZone = swn.GetNextZone();
            Zone seekerZone = start.GetOtherLinkedZone(attackZone);

            MoveDrone(5, start.id, attackZone.id);
            if (seekerZone != null)
                MoveDrone(1, start.id, seekerZone.id);
        }

        else if (game.frame < game.enemyDistance + 2)
        {
            foreach (Zone z in allGroup)
            {
                if (z.GetMyDrones(game.myId) == 1)
                {
                    Squad sq = new Squad(1, z);
                    sq.SetBehaviour(BehaviorStorage.IncomeSeekerBehavior);
                    squadsToMove.Add(sq);
                }
                else if (z.id == game.myFlagZone.id)
                {
                }
                else
                {
                    Squad sq = new Squad(z.GetMyDrones(game.myId), z);
                    sq.SetBehaviour(BehaviorStorage.GoDirectlyToBase);
                    squadsToMove.Add(sq);
                }
            }
            foreach (Squad s in squadsToMove)
            {
                s.DoTheThing();
                if (s.nextPosition != null)
                {
                    MoveDrone(s.droneNumber, s.currentPosition.id, s.nextPosition.id);
                }
            }
        }
        else SquadMissionManagement();

    }

    public static List<Zone> alone = new List<Zone>();
    public static List<Zone> duo = new List<Zone>();
    public static List<Zone> littleGroup = new List<Zone>();
    public static List<Zone> bigGroup = new List<Zone>();
    public static List<Zone> army = new List<Zone>();
    public static List<Zone> allGroup = new List<Zone>();
    private static void GetUnityGroup(int myid, ref  List<Zone> alone, ref  List<Zone> duo, ref  List<Zone> littleGroup, ref  List<Zone> bigGroup, ref List<Zone> army, ref List<Zone> allGroup)
    {
        alone.Clear();
        duo.Clear();
        littleGroup.Clear();
        bigGroup.Clear();
        army.Clear();
        allGroup.Clear();

        foreach (Zone z in Zone.GetZones())
        {
            if (z.data.ownerId == myid && z.GetMyDrones(myid) == 1)
            {
                alone.Add(z); allGroup.Add(z);
            }
            else if (z.data.ownerId == myid && z.GetMyDrones(myid) == 2)
            {
                duo.Add(z); allGroup.Add(z);
            }
            else if (z.data.ownerId == myid && z.GetMyDrones(myid) <= 5)
            {
                littleGroup.Add(z); allGroup.Add(z);
            }
            else if (z.data.ownerId == myid && z.GetMyDrones(myid) <= 30)
            {
                bigGroup.Add(z); allGroup.Add(z);
            }
            else { army.Add(z); allGroup.Add(z); }
        }
    }

}
class InspectorInfo
{
    public int incomeSeekerNumber = 5; //1
    public int neutraZoneSeekerNumber = 2; //1
    public int defenderIncomeZoneNumber = 5; //2
    public int flagSeekerNumber = 2; //30
    public int seekerNumber = 7;

    public static Squad.Behaviour incomeSeeked; // go take station
    public static Squad.Behaviour enemyIncomeSeeked; // go take station next to the enemy station
    public static Squad.Behaviour neutralSeeked; // go take neutral zone
    public static Squad.Behaviour defenderSeeked; //go take the territory we lost
    public static Squad.Behaviour flagSeeker; // attack the enemy flag
    public static Squad.Behaviour prioritySeeker; // attack the enemy flag
}

public class GameData
{
    public static GameData Instance;
    public int playerCount = 2;
    public int myId = 0;
    public int enemyId = 1;
    public int neutralId = -1;
    public int zoneCount;
    public int linkCount;
    public int frame;


    public Dictionary<int, ShortestWayNode> enemyFlagPath;
    public Dictionary<int, ShortestWayNode> myFlagPath;
    public Zone myFlagLocalisation;
    public Zone enemyFlagLocalisation;
    public int enemyDistance;

    public void SetPlayerOrigineZone(bool isEnemy, Zone zone)
    {


        if (isEnemy)
        {
            enemyFlagLocalisation = zone;
            enemyFlagPath = CentralMap.GetMap(zone);
        }
        else
        {
            myFlagLocalisation = zone;
            myFlagPath = CentralMap.GetMap(zone);
        }


        if (myFlagLocalisation != null && enemyFlagPath != null)
        {
            ShortestWayNode swn = null;
            enemyFlagPath.TryGetValue(myFlagLocalisation.id, out swn);
            NodeZone nz = swn.GetNextNodeZone();
            enemyDistance = nz.cost;
        }
    }


    /// ############ ADD Station REGISTER
    ////public void Add(int platinum, Zone zone)
    ////{
    ////    platinum--;
    ////    if (platinum < 0) platinum = 0;
    ////    if (platinum > 5) platinum = 5;
    ////    if (zone != null)
    ////        stations[platinum].Add(zone);
    ////}
    ////public List<Zone> Get(int platinum)
    ////{
    ////    platinum--;
    ////    if (platinum < 0) platinum = 0;
    ////    if (platinum > 5) platinum = 5;
    ////    return stations[platinum];
    ////}

    ////private List<Zone>[] stations = new List<Zone>[]
    ////        {
    ////            new List<Zone>(),
    ////            new List<Zone>(),
    ////            new List<Zone>(),
    ////            new List<Zone>(),
    ////            new List<Zone>(),
    ////            new List<Zone>()
    ////        };
    public string mapName;
    public MapStat mapStrat;

}



 
public class Squad
{

    public int droneNumber;
    //public int startDroneNumber;
    public Zone startZone;
    public Zone currentPosition;
    public Zone nextPosition;
    //public Zone lastPosition;
    //public Queue<Zone> destinationPath = new Queue<Zone>();



    public delegate void Behaviour(Squad escade);
    private Behaviour behaviour;


    public Squad(int number, Zone startZone)
    {
        currentPosition = this.startZone = startZone;
        droneNumber = number;
        //--//  Squad.squads.Add(this);
    }
    void Destroy()
    {
        //       destinationPath.Clear();
        behaviour = null;
        //--//  Squad.squads.Remove(this);

    }
    public void SetBehaviour(Behaviour beh) { behaviour = beh; }
    //public void AddZoneToDestination(Zone zone)
    //{
    //    if (zone != null)
    //        destinationPath.Enqueue(zone);
    //}
    //public void CancelDestination() { destinationPath.Clear(); }


    public void DoTheThing()
    {
        if (behaviour != null)
            behaviour(this);
        if (droneNumber <= 0)
            Destroy();

    }
    //--// public static List<Squad> squads = new List<Squad>();
}

public class BehaviorStorage
{

    public static void IncomeSeekerBehavior(Squad escade)
    {
        RandomMovement(escade);

    }



    public static void EnemyIncomeSeekerBehavior(Squad escade)
    {
        GoToEnemyBased(escade);
    }

    public static void PrioritySeekerBehavior(Squad escade)
    {
        Zone z = escade.currentPosition;
        int higherPriority = 0;
        Zone next = null;
        foreach (Zone nz in z.linkedZone)
        {
            // Console.Error.WriteLine("NZ:" + nz.data.priorityMark);
            if (nz.data.priorityMark > higherPriority)
            {
                higherPriority = nz.data.priorityMark;
                next = nz;
            }
        }
        if (next == null)
            GoToEnemyBased(escade);
        else
            escade.nextPosition = next;
    }

    public static void NeutralSeekerBehavior(Squad escade)
    {
        RandomMovement(escade);
    }

    public static void DefenderSeekerBehavior(Squad escade)
    {
        GoToEnemyBased(escade);
    }

    public static void FlagSeekerBehavior(Squad escade)
    {
        GoToEnemyBased(escade);


    }
    public static void NoOrder(Squad escade)
    {
    }



    public static void RandomMovement(Squad escade)
    {
        Zone z = escade.currentPosition;
        escade.nextPosition = z.GetRandomLinkedZone();
    }
    public static void GoToEnemyBased(Squad escade)
    {
        Zone z = escade.currentPosition;
        Zone income = z.GetNextIncomeZone(GameData.Instance.myId);
        if (income != null)
        {
            escade.nextPosition = income;
            return;
        }
        ShortestWayNode swn;
        GameData.Instance.enemyFlag.TryGetValue(z.id, out swn);
        if (swn != null)
            escade.nextPosition = swn.GetNextZone();

    }
    public static void GoDirectlyToBase(Squad escade)
    {
        Zone z = escade.currentPosition;
        ShortestWayNode swn;
        GameData.Instance.enemyFlag.TryGetValue(z.id, out swn);
        if (swn != null)
            escade.nextPosition = swn.GetNextZone();
    }

}

 
public struct FixedMap
{
    public string mapName;
    public int zoneNumber;
    public MapZone[] markedZone;

    public bool IsItThisMap(ref Dictionary<int, Zone> existingZone, out string name)
    {
        name = "";
        if (existingZone.Count - 1 != zoneNumber) return false;
        foreach (MapZone mz in markedZone)
            if (!mz.IsZoneBelongToMap(ref existingZone))
                return false;
        name = this.mapName;
        return true;
    }
    public override string ToString()
    {
        string description = string.Format("{0} ({1})", mapName, zoneNumber);
        foreach (MapZone zone in markedZone)
            description += " " + zone;
        return description;
    }
}
public struct MapZone
{
    public int zoneId;
    public int[] friendZoneId;

    public MapZone(int mainZoneId, int[] linkedZoneId)
    {
        // TODO: Complete member initialization
        this.zoneId = mainZoneId;
        this.friendZoneId = linkedZoneId;
    }
    public bool IsZoneBelongToMap(ref Dictionary<int, Zone> existingZone)
    {
        Zone center = null;
        if (!existingZone.ContainsKey(zoneId)) return false;
        existingZone.TryGetValue(zoneId, out center);
        if (center == null) return false;

        foreach (int izone in friendZoneId)
        {
            if (!existingZone.ContainsKey(izone)) return false;
            if (center.GetLinked(izone) == null) return false;
        }
        return true;
    }
    public override string ToString()
    {
        string description = "ID " + zoneId + " (";
        foreach (int id in friendZoneId)
            description += " " + id;
        return description + ")";
    }
}

public abstract class MapStat
{

    private FixedMap _map;
    public FixedMap Map
    {
        get { return _map; }
        private set { _map = value; }
    }
    public MapStat(FixedMap map)
    {
        Map = map;
    }
    public abstract void GiveSquadResponsability();

}
public class Map_SS : MapStat
{
    MapArea left;
    MapArea right;
    ShortestWayNode centerDefensePoint;

    //TODO
    //Relay point = zone comme la base qui affect des squads et n'est pas affecter par les ordes générals


    public Map_SS(FixedMap map)
        : base(map)
    {
        left = new MapArea(new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11 });
        right = new MapArea(new int[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 });
    }

    public override void GiveSquadResponsability()
    {

    }

    public override string ToString()
    {
        return "Map SS";
    }
}
public class MapArea
{
    public int[] zoneId;

    public MapArea(int[] zoneId) { this.zoneId = zoneId; }

    public bool Contains(int wantedId)
    {
        foreach (int id in zoneId)
            return true;
        return false;
    }

    public float GetPourcentControledBy(int playerId)
    {
        //TODO
        return 1f;
    }
    public override string ToString()
    {
        string description = "Map Area: ";
        foreach (int id in zoneId)
            description += " " + id;
        return description;
    }
}