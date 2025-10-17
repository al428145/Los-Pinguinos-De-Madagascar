public enum StateEvent
{
    None,
    NoiseHeard,
    PlayerSeen,
    AlertTimeout,
    InvestigateDone,
    StartChase,
    LostPlayer,
    returnRute,
    PlayerHeard = NoiseHeard, // alias para gallina
    AlertEnd = AlertTimeout,   // alias opcional
    SCAlerted
}
