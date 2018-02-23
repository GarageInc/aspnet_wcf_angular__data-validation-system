using System;

namespace SerializationTest
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class OddService
    {

        /// <remarks/>
        public OddServiceHeader Header { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Event", IsNullable = false)]
        public OddServiceEvent[] Results { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceHeader
    {

        /// <remarks/>
        public byte Status { get; set; }

        /// <remarks/>
        public string Description { get; set; }

        /// <remarks/>
        public uint Timestamp { get; set; }

        /// <remarks/>
        public uint clientsTimestamp { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEvent
    {
        /// <remarks/>
        public OddServiceEventEventID EventID { get; set; }

        /// <remarks/>
        public System.DateTime StartDate { get; set; }

        /// <remarks/>
        public OddServiceEventSportID SportID { get; set; }

        /// <remarks/>
        public OddServiceEventLeagueID LeagueID { get; set; }

        /// <remarks/>
        public OddServiceEventLocationID LocationID { get; set; }

        /// <remarks/>
        public string Status { get; set; }

        /// <remarks/>
        public System.DateTime LastUpdate { get; set; }

        /// <remarks/>
        public OddServiceEventHomeTeam HomeTeam { get; set; }

        /// <remarks/>
        public OddServiceEventAwayTeam AwayTeam { get; set; }

        /// <remarks/>
        public OddServiceEventRace Race { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Participant", IsNullable = false)]
        public OddServiceEventParticipant[] Participants { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Participant", IsNullable = false)]
        public OddServiceEventParticipant1[] RaceResults { get; set; }

        /// <remarks/>
        public OddServiceEventStat Stat { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Card", IsNullable = false)]
        public OddServiceEventCard[] Cards { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Scorer", IsNullable = false)]
        public OddServiceEventScorer[] Scorers { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Outcome", IsNullable = false)]
        public OddServiceEventOutcome[] Outcomes { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventEventID
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string ExternalID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore]
        public bool ExternalIDSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventSportID
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventLeagueID
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventLocationID
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventHomeTeam
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string ID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventAwayTeam
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string ID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventScores
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Score")]
        public OddServiceEventScoresScore[] Score { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string status { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string time { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte IsServing { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsServingSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Description { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventScoresScore
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string period { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string homeScore { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string awayScore { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventScorer
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string isPenalty { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string teamName { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string time { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string score { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string period { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventOutcome
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Bookmaker")]
        public OddServiceEventOutcomeBookmaker[] Bookmaker { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string name { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventOutcomeBookmaker
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Odds")]
        public OddServiceEventOutcomeBookmakerOdds[] Odds { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public System.DateTime lastUpdate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string bookieEventID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string bookieLeagueID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string BookieOfferTypeID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string isResulting { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OddServiceEventOutcomeBookmakerOdds
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string bet { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string startPrice { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string currentPrice { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string line { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string BaseLine { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public System.DateTime LastUpdate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string bookieOutcomeID { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string Status { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProgramNumber { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string isWinner { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public bool isWinnerSpecified { get; set; }
    }
}






/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventRace
{
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte Number { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Title { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Type { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Category { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ushort Distance { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Surface { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Going { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte AgeFrom { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte AgeTo { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte PlaceTerm { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte PlaceOddsFactor { get; set; }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventParticipant
{
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Number { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte Age { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Weight { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Gender { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Jockey { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Trainer { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Form { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Silk { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool IsRunning { get; set; }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventParticipant1
{
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Number { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Position { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Distance { get; set; }
}


/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventStat
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Aces", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Attacks", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("BlockedShots", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("BreakPoints", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("BreakPointsConversion", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Clearance", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Corners", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("DengerousAttacks", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("DoubleFaults", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("FirstServeWinning", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Fouls", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("FreeKicks", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("FreeThrows", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("GoalKicks", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Goals", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Hit", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Lineups", typeof(OddServiceEventStatLineups))]
    [System.Xml.Serialization.XmlElementAttribute("LongestStreak", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Offsides", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("PCT", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Penalties", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("PointsWononServe", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Possession", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("RedCards", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("ServicePoints", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("ShotsOffTarget", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("ShotsOnTarget", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Substitutions", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("ThreePoints", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("ThrowIns", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Throwins", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("TimeOuts", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("TwoPoints", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("WoodworkShots", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("YellowCards", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType[] ItemsElementName { get; set; }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventStatLineups
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Player")]
    public OddServiceEventStatLineupsPlayer[] Player { get; set; }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventStatLineupsPlayer
{
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Team { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string PlayerName { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Position { get; set; }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType
{

    /// <remarks/>
    Aces,

    /// <remarks/>
    Attacks,

    /// <remarks/>
    BlockedShots,

    /// <remarks/>
    BreakPoints,

    /// <remarks/>
    BreakPointsConversion,

    /// <remarks/>
    Clearance,

    /// <remarks/>
    Corners,

    /// <remarks/>
    DengerousAttacks,

    /// <remarks/>
    DoubleFaults,

    /// <remarks/>
    FirstServeWinning,

    /// <remarks/>
    Fouls,

    /// <remarks/>
    FreeKicks,

    /// <remarks/>
    FreeThrows,

    /// <remarks/>
    GoalKicks,

    /// <remarks/>
    Goals,

    /// <remarks/>
    Hit,

    /// <remarks/>
    Lineups,

    /// <remarks/>
    LongestStreak,

    /// <remarks/>
    Offsides,

    /// <remarks/>
    PCT,

    /// <remarks/>
    Penalties,

    /// <remarks/>
    PointsWononServe,

    /// <remarks/>
    Possession,

    /// <remarks/>
    RedCards,

    /// <remarks/>
    ServicePoints,

    /// <remarks/>
    ShotsOffTarget,

    /// <remarks/>
    ShotsOnTarget,

    /// <remarks/>
    Substitutions,

    /// <remarks/>
    ThreePoints,

    /// <remarks/>
    ThrowIns,

    /// <remarks/>
    Throwins,

    /// <remarks/>
    TimeOuts,

    /// <remarks/>
    TwoPoints,

    /// <remarks/>
    WoodworkShots,

    /// <remarks/>
    YellowCards,
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class OddServiceEventCard
{
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string teamName { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte time { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type { get; set; }
}

