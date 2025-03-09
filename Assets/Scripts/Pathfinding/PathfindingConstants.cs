namespace PathfindingConstants
{
    public enum Direction : byte
    {
        North = 0b0001,
        East = 0b0010,
        West = 0b0100,
        South = 0b1000,
        NorthEast = North | East,
        SouthEast = South | East,
        SouthWest = South | West,
        NorthWest = North | West
    }

    public enum TerrainType : byte
    {
        None = 0,
        Ground = 0b001,
        Water = 0b010,
        Rock = 0b100
    }

    public static class PathfindingConstantValues
    {
        public static TerrainType[] capabilities = { TerrainType.Ground, TerrainType.Water, TerrainType.Ground | TerrainType.Water };
        public static int[] agentSizes = { 3, 2, 1 }; // descending order
        public static int maxAgentSize = agentSizes[0];
    }
}
