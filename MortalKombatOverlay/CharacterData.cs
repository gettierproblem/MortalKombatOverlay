using System.Collections.Generic;

namespace MortalKombatOverlay;

public class CharacterData
{
    public Dictionary<string, List<MovePart>> SpecialMoves { get; set; } = new();
    public Dictionary<string, List<MovePart>> Finishers { get; set; } = new();
}

public class MovePart
{
    public string Type { get; set; }
    public string Value { get; set; }
}