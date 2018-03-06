using System; using System.Linq; using System.IO; using System.Text; using System.Collections; using System.Collections.Generic; class _59 { public static int Clamp(int value, int min, int max) { return (value < min) ? min : (value > max) ? max : value; } } class _61 { public static _62 game; static void Main(string[] args) { _62._83 = game = new _62(DateTime.Now.Ticks); InitGameAnd_112Info(); while (true) { InitTurnInfoAndCallStartUpdate(); } } public static void _63(string message) { Console.Error.WriteLine(message); } static string _91 = ""; static void _64(int _0, int from, int to) { bool _92 = _91.Length <= 0; _91 += String.Format("{0}{1} {2} {3}", _92 ? "" : " ", _0, from, to); } static string _1 = ""; static void BuyDrone(int _0, int where) { bool _92 = _1.Length <= 0; _1 += String.Format("{0}{1} {2}", _92 ? "" : " ", _0, where); } static void SendTurnInstruction() { if (_91.Length <= 0) Console.WriteLine("WAIT"); else Console.WriteLine(_91); if (_1.Length <= 0) Console.WriteLine("WAIT"); else Console.WriteLine(_1); } static void Awake() { _65._98Seeked = _2.IncomeSeekerBehavior; _65.enemyIncomeSeeked = _2.EnemyIncomeSeekerBehavior; _65.neutralSeeked = _2.NeutralSeekerBehavior; _65.defenderSeeked = _2.DefenderSeekerBehavior; _65.flagSeeker = _2._3; _65.prioritySeeker = _2._4; } static void Start() { foreach (_112 z in _112._5()) z.zoneType = _112.Get_72WithoutBigResearch(z, z._6); bool _93; _66 map = CreateAndCheck_MapRegister(out _93); game._7 = map._7; _63(_93 ? game._7 : "No fixed map _93"); if (game._7 == "SS") { game.mapStrat = new Map_SS(map); } } static _66 CreateAndCheck_MapRegister(out bool _93) { _93 = false; string name = ""; List<_66> maps = new List<_66>(); _66 map; map = new _66() { _7 = "TheCross", _8 = 149, _9 = new _67[] { new _67(94, new int[] { 93, 100, 101, 95, 87, 86 }), new _67(55, new int[] { 48, 49, 56, 54, 62, 63 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } map = new _66() { _7 = "Black Hole", _8 = 367, _9 = new _67[] { new _67(226, new int[] { 225, 239, 240, 227, 216, 215 }), new _67(141, new int[] { 140, 151, 152, 127, 128, 142 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } map = new _66() { _7 = "Square", _8 = 199, _9 = new _67[] { new _67(188, new int[] { 197, 198, 189, 187, 177, 178 }), new _67(93, new int[] { 92, 94, 103, 104, 83, 84 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } map = new _66() { _7 = "PR2", _8 = 405, _9 = new _67[] { new _67(305, new int[] { 295, 296, 306, 304, 316, 317 }), new _67(100, new int[] { 99, 101, 88, 89, 109, 110 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } map = new _66() { _7 = "Leaf", _8 = 159, _9 = new _67[] { new _67(131, new int[] { 130, 121, 122, 138, 139 }), new _67(51, new int[] { 43, 44, 52, 61, 60 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } map = new _66() { _7 = "SS", _8 = 23, _9 = new _67[] { new _67(15, new int[] { 13, 14, 16, 18, 19 }), new _67(8, new int[] { 5, 4, 7, 9, 10 }) } }; if (map._10(ref _112._94, out name)) { _93 = true; return map; } return map; } static void UpdateBefore() { _91 = ""; _1 = ""; _112Processing(); game._11 = game._12; } static void UpdateEnd() { _63StateGame_EndUpdate(); } static void Update() { _63StateGame_StartUpdate(); _62 game = _62._83; BuyDrone((int)(game._87[0]._13 / 20f), 1); int myId = game.myId; int _14 = game._14; _61Info _95 = game._87[_14]; if (game._90 > 6) { if (game._96 > 1) _100MissionManagement(); else DispertionMission(); } else { Rush(); } SendTurnInstruction(); } static void _63StateGame_StartUpdate() { _63(string.Format("Game state: (f:{0}) Time: {1}", game._96, game._12 / 1000000000.0)); _63(string.Format("----------- Start-------------")); _63(string.Format("Map: {0} ({1}) ", game._7, game._85)); _63(string.Format("Start id, Me: {0} Enemy : {1}", game._88.id, game._33_112.id)); _63(string.Format("## Battle State ## ")); _63(string.Format("Pct conq. {0} % ", (int)(game._60(true) * 100f))); } static void _63StateGame_EndUpdate() { _63(string.Format("______________________________")); double _15 = (double)game._11 / (double)game.TurnMaxTime; _15 = _59.Clamp((int)(_15 * 100.0), 0, 100); _63(string.Format("Time used: {0} ({1}%)", game._12Turn / 1000000000.0, _15)); _63(string.Format("-------------END--------------")); } static void _112Processing() { int myId = game.myId; game.Enemy_112Count = 0; game.My_112Count = 0; game.Neutral_112Count = 0; foreach (_61Info p in game._87) p.globalIncome = 0; foreach (_112 z in _112._5()) { if (z.IsOwnedByEnemy()) game.Enemy_112Count++; else if (z.IsOwnedByMe()) game.My_112Count++; else game.Neutral_112Count++;  if (z.GetEnemiesDrone(myId) > 0) z.data.ene_97++; if (z._16(myId) > 0) z.data._97++;  if (z.data._17 >= 0) game._87[z.data._17].globalIncome += z._98; if (z.data._17 == myId) z.data._18 = 0; else if (IsEnemyStartPoint(z)) z.data._18 = 50; else {  z.data._18 += 1 + z._98;  foreach (_112 link in z._6) {  link.data._18 += z._98 > 3 ? 2 : 1; if (z._98 > 3) foreach (_112 sublink in link._6) { sublink.data._18 += 1; } } } } } static bool IsEnemyStartPoint(_112 zone) { if (zone == null) return false; if (zone.data._17 == game.myId) return false; foreach (_61Info p in game._87) if (p._19 != null && p._19.id == zone.id) return true; return false; } static _61Info MoveAllUnityEveryWhere(_62 game, int myId, int _14, _61Info _95) { foreach (_112 zone in _112._5()) { if (zone.GetFirstEnemyAround() != null) { } if (zone == _95._19 && game._96 > 8) { if (zone._16(_14) < 4f) continue; } int _20 = zone._16(_14); if (_20 > 0) zone.data._97++; _112 to; to = zone.GetNextIncome_112(_14); if (to != null && _20 > 0) { _64(1, zone.id, to.id); _20--; } to = zone.GetNextNeutral_112(); if (to != null && _20 > 0) { _64(1, zone.id, to.id); _20--; } to = zone.GetNextEnemy_112(myId); if (to != null && _20 > 0) { _64(1, zone.id, to.id); _20--; } to = zone.GetLessMark_112(); if (to != null && _20 > 1) { int part = _20 * 2 / 3; _64(1 + part, zone.id, to.id); _20 -= 1 + part; } List<_112> alreadyOneOn = new List<_112>(); for (int i = 0; i < zone._6.Count; i++) { to = zone._21(); if (to != null && _20 > 0 && !alreadyOneOn.Contains(to)) { alreadyOneOn.Add(zone); _64(1, zone.id, to.id); _20--; } } if (_20 > 0) { to = zone.GetNextEnemy_112(_14); if (to == null) to = zone.GetLessMark_112(); if (to == null) to = zone._21(); _64(_20, zone.id, to.id); } } return _95; } public static void InitGameAnd_112Info() { Awake(); string[] _99; _99 = Console.ReadLine().Split(' '); game._84 = int.Parse(_99[0]); game._14 = game.myId = int.Parse(_99[1]); game._85 = int.Parse(_99[2]); game._86 = int.Parse(_99[3]); for (int i = 0; i < game._85; i++) { _112 _22 = new _112(); _99 = Console.ReadLine().Split(' '); _22.id = int.Parse(_99[0]); _22._98 = int.Parse(_99[1]); _112.Add(_22.id, _22); } for (int i = 0; i < game._86; i++) { _99 = Console.ReadLine().Split(' '); _112 zone1 = _112.Get(int.Parse(_99[0])); _112 zone2 = _112.Get(int.Parse(_99[1])); zone1.AddLink(zone2); zone2.AddLink(zone1); } } public static void InitTurnInfoAndCallStartUpdate() { _62 game = _62._83; string[] _99; int iFrame = game._96; if (iFrame == 0) { Start(); } UpdateBefore(); game._87[0]._13 = int.Parse(Console.ReadLine()); for (int i = 0; i < _112.Count; i++) { _99 = Console.ReadLine().Split(' '); _112 zone = _112.Get(int.Parse(_99[0])); zone.SetArmiesState(int.Parse(_99[1]), int.Parse(_99[2]), int.Parse(_99[3]), int.Parse(_99[4]), int.Parse(_99[5])); if (iFrame == 0 && zone.data._17 != -1) { game.Set_61Origine_112(zone.data._17, zone); } } Update(); UpdateEnd(); game._96++; } static List<_100> _23 = new List<_100>(); static void _100MissionManagement() { _24(game.myId, ref _101, ref duo, ref _25, ref _26, ref army, ref _27); _23.Clear(); int i = 0; foreach (_112 z in _101) { _100 sq = new _100(1, z); if (i % 3 == 0)  sq._28(_2._4); else  sq._28(_2._4); _23.Add(sq); i++; } i = 0; foreach (_112 z in duo) { _100 sq = new _100(2, z); sq._28(_2._4); _23.Add(sq); i++; } i = 0; foreach (_112 z in _25) { _100 sq = new _100(z._16(game.myId), z); if (i < 2 + (game._96 / 30)) sq._28(_2._4); else sq._28(_2._3); _23.Add(sq); i++; } i = 0; foreach (_112 z in _26) { _100 sq = new _100(z._16(game.myId), z); if (i < 1 + (game._96 / 100)) sq._28(_2._4); else sq._28(_2._3); _23.Add(sq); i++; } i = 0; foreach (_112 z in army) { _100 sq = new _100(z._16(game.myId), z); sq._28(_2._3); _23.Add(sq); i++; } foreach (_100 s in _23) { s._29(); if (s._30 != null) { _64(s._0, s._31.id, s._30.id); } } } static void DispertionMission() { _24(game.myId, ref _101, ref duo, ref _25, ref _26, ref army, ref _27); _23.Clear(); foreach (_112 z in _27) { List<_112> _102 = z._6; for (int i = 0, c = 0; i < z._16(game.myId); i++, c++) { c = c % (_102.Count - 1); if (c > 0 && _102.Count > 0) { _100 sq = new _100(1, z); sq._28(_2.NoOrder); sq._30 = _102[c]; _23.Add(sq); } } } foreach (_100 s in _23) { s._29(); if (s._30 != null) { _64(s._0, s._31.id, s._30.id); } } } public static void Rush() { _24(game.myId, ref _101, ref duo, ref _25, ref _26, ref army, ref _27); if (game._96 == 0) { _112 _103 = game._88; _32 swn = null; game._33.TryGetValue(_103.id, out swn); _112 _34 = swn._35(); _112 seeker_112 = _103.GetOtherLinked_112(_34); _64(5, _103.id, _34.id); if (seeker_112 != null) _64(1, _103.id, seeker_112.id); } else if (game._96 < game._90 + 2) { foreach (_112 z in _27) { if (z._16(game.myId) == 1) { _100 sq = new _100(1, z); sq._28(_2.IncomeSeekerBehavior); _23.Add(sq); } else if (z.id == game._88.id) { } else { _100 sq = new _100(z._16(game.myId), z); sq._28(_2.GoDirectlyToBase); _23.Add(sq); } } foreach (_100 s in _23) { s._29(); if (s._30 != null) { _64(s._0, s._31.id, s._30.id); } } } else _100MissionManagement(); } public static List<_112> _101 = new List<_112>(); public static List<_112> duo = new List<_112>(); public static List<_112> _25 = new List<_112>(); public static List<_112> _26 = new List<_112>(); public static List<_112> army = new List<_112>(); public static List<_112> _27 = new List<_112>(); static void _24(int myid, ref List<_112> _101, ref List<_112> duo, ref List<_112> _25, ref List<_112> _26, ref List<_112> army, ref List<_112> _27) { _101.Clear(); duo.Clear(); _25.Clear(); _26.Clear(); army.Clear(); _27.Clear(); foreach (_112 z in _112._5()) { if (z.data._17 == myid && z._16(myid) == 1) { _101.Add(z); _27.Add(z); } else if (z.data._17 == myid && z._16(myid) == 2) { duo.Add(z); _27.Add(z); } else if (z.data._17 == myid && z._16(myid) <= 5) { _25.Add(z); _27.Add(z); } else if (z.data._17 == myid && z._16(myid) <= 30) { _26.Add(z); _27.Add(z); } else { army.Add(z); _27.Add(z); } } } } class _65 { public int _98SeekerNumber = 5; public int neutra_112SeekerNumber = 2; public int defenderIncome_112Number = 5; public int flagSeekerNumber = 2; public int seekerNumber = 7; public static _100._36 _98Seeked; public static _100._36 enemyIncomeSeeked; public static _100._36 neutralSeeked; public static _100._36 defenderSeeked; public static _100._36 flagSeeker; public static _100._36 prioritySeeker; } public class _61Info { public int id; public int _13; public _112 _19; public int globalIncome; } public class _62 { public static _62 _83; public int _84 = 2; public int myId; public int _14; public int _85; public int _86; public _61Info[] _87 = new _61Info[] { new _61Info(), new _61Info(), new _61Info(), new _61Info() }; public int _96; public Dictionary<int, _32> _33; public Dictionary<int, _32> myFlag; public _112 _88; public _112 _33_112; public int _90; public void Set_61Origine_112(int _37, _112 zone) { _62 game = _62._83; if (_37 < 0 || _37 > 3) return; _87[_37]._19 = zone; if (_37 == _62._83.myId) { game.myFlag = CentralMap.GetMap(zone); game._88 = zone; } else { game._33 = CentralMap.GetMap(zone); game._33_112 = zone; } if (game.myFlag != null && game._33 != null) { _32 swn = null; game._33.TryGetValue(game._88.id, out swn); _69 nz = swn.GetNext_69(); game._90 = nz.cost; _61._63("Dist based:" + game._90); } } public void Add(int _13, _112 zone) { _13--; if (_13 < 0) _13 = 0; if (_13 > 5) _13 = 5; if (zone != null) stations[_13].Add(zone); } public List<_112> Get(int _13) { _13--; if (_13 < 0) _13 = 0; if (_13 > 5) _13 = 5; return stations[_13]; } List<_112>[] stations = new List<_112>[] { new List<_112>(), new List<_112>(), new List<_112>(), new List<_112>(), new List<_112>(), new List<_112>() }; public string _7; public _70 mapStrat; long _38; long p; public _62(long _103Time) { _38 = _103Time; } public long StartTime { get { return _38; } set { _38 = value; } } public long Time { get { return DateTime.Now.Ticks; } } public long _12 { get { return Time - StartTime; } } public long _12Turn { get { return _12 - _11; } } long _timeStartTurn; public long _11 { get { return _timeStartTurn; } set { _timeStartTurn = value; } } public long TurnMaxTime { get { return 100000000; } } public int Enemy_112Count { get; set; } public int My_112Count { get; set; } public int Neutral_112Count { get; set; } public float _60(bool my) { if (_62._83._85 == 0) return 0f; float pct =(float)_62._83.My_112Count / (float) _62._83._85; if (!my) pct = (float)_62._83.Enemy_112Count / (float)_62._83._85; return pct; } } public class _112 { public int id; public int _98 = 0; public _72 zoneType = _72._73; public _112Data data = new _112Data(); public List<_112> _6 = new List<_112>(); public delegate void On_112Conquer(int _39, int newOWner); public On_112Conquer _113; public delegate void OnArmyMove(int _40, _112 zone, int oldDroneNumber, int newDroneNumber); public OnArmyMove _41; public delegate void _115(_112 zone); public _115 _114; public void AddLink(_112 zone) { _6.Add(zone); } public void SetArmiesState(int _42, int _43, int _44, int _45, int _13) { int _39 = data._17; if (_113 != null && _39 != _42) { _113(_39, _42); } data._17 = _42; if (_41 != null) { if (data._104 != _43) _41(0, this, data._104, _43); } data._104 = _43; if (_41 != null) { if (data._105 != _44) _41(1, this, data._105, _44); } data._105 = _44; data._45 = _45 == 0; if (_45 == 0 && _13 > 0) { if (_114 != null) { _114(this); } data._13 = _13; _62._83.Add(_13, this); } } public static int Count { get { return _94.Count; } } public static Dictionary<int, _112> _94 = new Dictionary<int, _112>(); public static void Add(int id, _112 zone) { _94.Add(id, zone); } public static _112 Get(int id) { _112 val; _94.TryGetValue(id, out val); return val; } public static _112[] _5() { _112[] array = new _112[_94.Values.Count]; _94.Values.CopyTo(array, 0); return array; } public int _16(int _40) { switch (_40) { case 0: return data._104; case 1: return data._105; } return 0; } public int GetEnemiesDrone(int _40) { int num = 0; if (_40 != 0) num += data._104; if (_40 != 1) num += data._105; return num; } Random rand = new Random(DateTime.Now.Millisecond); public _112 _21() { rand = new Random(DateTime.Now.Millisecond); int num = rand.Next(0, _6.Count); return _6[num]; } public _112 GetNextNeutral_112() { foreach (_112 z in _6) if (z.data._17 == -1) return z; return null; } public _112 GetNextEnemy_112(int myId) { foreach (_112 z in _6) if (z.data._17 != myId && z.data._17 != -1) return z; return null; } internal _112 GetNextIncome_112(int userId) { foreach (_112 z in _6) if (z._98 > 0 && z.data._17 != userId) return z; return null; } public _112 GetLessMark_112() { int mark = int.MaxValue; _112 zone = null; foreach (_112 z in _6) { if (z.data._97 < mark) { zone = z; mark = z.data._97; } } return zone; } public _112 GetFirstEnemyAround() { foreach (_112 z in _6) if (z.GetEnemiesDrone(_62._83.myId) > 0) return z; return null; } public enum _72 { _73, _74, _75, _76, _76Start, _78, _81, _82 } public static _72 Get_72WithoutBigResearch(_112 zone, List<_112> _6) { if (_6.Count == 6) return _72._74; if (_6.Count == 1) return _72._75; if (_6.Count == 2) { if (_6[0]._116(_6[1])) return _72._75; else return _72._78; } if (_6.Count == 3) { _112 z1, z2, z3; z1 = _6[0]; z2 = _6[1]; z3 = _6[2]; if (z1._116(z2) || z1._116(z3) || z2._116(z1) || z1._116(z3) || z3._116(z1) || z3._116(z2)) return _72._82; } return _72._73; } bool _116(_112 zone) { return _6.Contains(zone); } public _112 GetOtherLinked_112(_112 _34) { _112 val = null; foreach (_112 z in _6) if (z != _34) { if (val == null) val = z; else if (val.data._13 < z.data._13) val = z; } return val; } internal _112 GetLinked(int id_112) { _112 z = null; foreach (_112 zone in _6) if (zone.id == id_112) return zone; return z; } public override string ToString() { string _46 = string.Format("_112 {0} ({1}$ _61 {2})", this.id, this._98, this.data._17); return _46; } internal bool IsOwnedByEnemy() { return !IsNeutral_112() && !IsOwnedByMe(); } internal bool IsOwnedByMe() { return data._17 == _62._83.myId; } internal bool IsNeutral_112() { return data._17 == -1; } } public class _112Data { public int _17; public int _104; public int _105; public int _13; public bool visited; public bool _45; public int stolenCount; public int ene_97; public int _97; public int _18; public _112 enemyBaseDirection; public _112 myBaseDirection; } public class _100 { public int _0; public _112 _47; public _112 _31; public _112 _30; public delegate void _36(_100 _106); _36 _48; public _100(int number, _112 _47) { _31 = this._47 = _47; _0 = number; } void Destroy() { _48 = null; } public void _28(_36 beh) { _48 = beh; }  public void _29() { if (_48 != null) _48(this); if (_0 <= 0) Destroy(); } } public class _2 { public static void IncomeSeekerBehavior(_100 _106) { RandomMovement(_106); } public static void EnemyIncomeSeekerBehavior(_100 _106) { _49(_106); } public static void _4(_100 _106) { _112 z = _106._31; int higherPriority = 0; _112 next = null; foreach (_112 nz in z._6) { if (nz.data._18 > higherPriority) { higherPriority = nz.data._18; next = nz; } } if (next == null) _49(_106); else _106._30 = next; } public static void NeutralSeekerBehavior(_100 _106) { RandomMovement(_106); } public static void DefenderSeekerBehavior(_100 _106) { _49(_106); } public static void _3(_100 _106) { _49(_106); } public static void NoOrder(_100 _106) { } public static void RandomMovement(_100 _106) { _112 z = _106._31; _106._30 = z._21(); } public static void _49(_100 _106) { _112 z = _106._31; _112 _98 = z.GetNextIncome_112(_62._83.myId); if (_98 != null) { _106._30 = _98; return; } _32 swn; _62._83._33.TryGetValue(z.id, out swn); if (swn != null) _106._30 = swn._35(); } public static void GoDirectlyToBase(_100 _106) { _112 z = _106._31; _32 swn; _62._83._33.TryGetValue(z.id, out swn); if (swn != null) _106._30 = swn._35(); } } public class CentralMap { public static Dictionary<int, _32> GetMap(_112 _107) { if (_107 == null) return null; Dictionary<int, _32> map = new Dictionary<int, _32>(); List<_112> _108 = new List<_112>(); Dictionary<int, _112> _109 = new Dictionary<int, _112>(); Explore(_107, ref _108, ref _109, ref map); return map; } static void Explore(_112 _107, ref List<_112> _108, ref Dictionary<int, _112> _109, ref Dictionary<int, _32> map) { int turnCount = 0; List<_112> _50 = new List<_112>(); List<_112> _51 = new List<_112>(); List<_112> _52 = new List<_112>(); _50.Add(_107); while (_50.Count > 0) { turnCount++; foreach (_112 _53 in _50) _109.Add(_53.id, _53); foreach (_112 _53 in _50) { _52.Clear(); Get_112Leaf(_53, ref _52, ref _51, ref _109); foreach (_112 leaf in _52) { AddLeafToMap(_53, leaf, turnCount, ref map); } } ClearCurrentAndInverse(ref _50, ref _51); } } static void ClearCurrentAndInverse(ref List<_112> _50, ref List<_112> _51) { _50.Clear(); List<_112> tmp = _50; _50 = _51; _51 = tmp; } static void AddLeafToMap(_112 parent, _112 leaf, int _53Cost, ref Dictionary<int, _32> map) { _32 swn = null; map.TryGetValue(leaf.id, out swn); if (swn == null) { swn = new _32(leaf); map.Add(leaf.id, swn); } _69 nz = new _69(parent, _53Cost); swn.Add(nz); } static void Get_112Leaf(_112 _53, ref List<_112> _108, ref List<_112> next, ref Dictionary<int, _112> _109) { foreach (_112 _54 in _53._6) if (!_109.ContainsKey(_54.id)) { _108.Add(_54); if (!next.Contains(_54)) next.Add(_54); } } } public class _32 { public _112 zone; public List<_69> _55 = new List<_69>(); public _32(_112 zone) { this.zone = zone; } public void Add(_69 node) { if (node != null) _55.Add(node); } public _112 _35() { int _56 = 10000; _112 next = null; foreach (_69 nz in _55) if (nz.cost < _56) { next = nz.zone; _56 = nz.cost; } return next; } public _69 GetNext_69() { int _56 = 10000; _69 next = null; foreach (_69 nz in _55) if (nz.cost < _56) { next = nz; _56 = nz.cost; } return next; } } public class _69 { public _112 zone; public int cost; public _69(_112 zone, int cost) { this.zone = zone; this.cost = cost; } } public struct _66 { public string _7; public int _8; public _67[] _9; public bool _10(ref Dictionary<int, _112> _57, out string name) { name = ""; if (_57.Count - 1 != _8) return false; foreach (_67 mz in _9) if (!mz.Is_112BelongToMap(ref _57)) return false; name = this._7; return true; } public override string ToString() { string _46 = string.Format("{0} ({1})", _7, _8); foreach (_67 zone in _9) _46 += " " + zone; return _46; } } public struct _67 { public int _110; public int[] _58; public _67(int main_112Id, int[] _6Id) { this._110 = main_112Id; this._58 = _6Id; } public bool Is_112BelongToMap(ref Dictionary<int, _112> _57) { _112 _111 = null; if (!_57.ContainsKey(_110)) return false; _57.TryGetValue(_110, out _111); if (_111 == null) return false; foreach (int izone in _58) { if (!_57.ContainsKey(izone)) return false; if (_111.GetLinked(izone) == null) return false; } return true; } public override string ToString() { string _46 = "ID " + _110 + " ("; foreach (int id in _58) _46 += " " + id; return _46 + ")"; } } public abstract class _70 { _66 _map; public _66 Map { get { return _map; } set { _map = value; } } public _70(_66 map) { Map = map; } public abstract void Give_100Responsability(); } public class Map_SS : _70 { _71 left; _71 right; _32 _111DefensePoint; public Map_SS(_66 map) : base(map) { left = new _71(new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11 }); right = new _71(new int[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }); } public override void Give_100Responsability() { } public override string ToString() { return "Map SS"; } } public class _71 { public int[] _110; public _71(int[] _110) { this._110 = _110; } public bool Contains(int wantedId) { foreach (int id in _110) return true; return false; } public float _60By(int _37) { return 1f; } public override string ToString() { string _46 = "Map Area: "; foreach (int id in _110) _46 += " " + id; return _46; } } 