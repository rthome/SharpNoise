using System.ComponentModel;

namespace ComplexPlanetExample
{
    /// <summary>
    /// Contains all parameters for planet surface generation
    /// </summary>
    public class PlanetGenerationSettings
    {
        /// <summary>
        /// Southernmost coordinate of elevation grid.
        /// </summary>
        [Description("Southernmost coordinate of elevation grid")]
        [Category("Coordinates")]
        public double SouthCoord { get; set; }

        /// <summary>
        /// Northernmost coordinate of elevation grid.
        /// </summary>
        [Description("Northernmost coordinate of elevation grid.")]
        [Category("Coordinates")]
        public double NorthCoord { get; set; }

        /// <summary>
        /// Westernmost coordinate of elevation grid.
        /// </summary>
        [Description("Westernmost coordinate of elevation grid.")]
        [Category("Coordinates")]
        public double WestCoord { get; set; }

        /// <summary>
        /// Easternmost coordinate of elevation grid.
        /// </summary>
        [Description("Easternmost coordinate of elevation grid.")]
        [Category("Coordinates")]
        public double EastCoord { get; set; }

        /// <summary>
        /// Width of elevation grid, in points.
        /// </summary>
        [Description("Width of elevation grid, in points.")]
        [Category("Image Size")]
        public int GridWidth { get; set; }

        /// <summary>
        /// Height of elevation grid, in points.
        /// </summary>
        [Description("Height of elevation grid, in points.")]
        [Category("Image Size")]
        public int GridHeight { get; set; }

        /// <summary>
        /// Planet seed.  Change this to generate a different planet.
        /// </summary>
        [Description("Planet seed.  Change this to generate a different planet.")]
        [Category("Planet Properties")]
        public int Seed { get; set; }

        /// <summary>
        /// Circumference of the planet, in meters.
        /// </summary>
        [Description("Circumference of the planet, in meters.")]
        [Category("Planet Properties")]
        public double PlanetCircumference { get; set; }

        /// <summary>
        /// Minimum elevation on the planet, in meters.  This value is approximate.
        /// </summary>
        [Description("Minimum elevation on the planet, in meters.  This value is approximate.")]
        [Category("Planet Properties")]
        public double MinElev { get; set; }

        /// <summary>
        /// Maximum elevation on the planet, in meters.  This value is approximate.
        /// </summary>
        [Description("Maximum elevation on the planet, in meters.  This value is approximate.")]
        [Category("Planet Properties")]
        public double MaxElev { get; set; }

        /// <summary>
        /// Frequency of the planet's continents.  Higher frequency produces smaller,
        /// more numerous continents.  This value is measured in radians.
        /// </summary>
        [Description("Frequency of the planet's continents.  Higher frequency produces smaller, " +
            "more numerous continents.  This value is measured in radians.")]
        [Category("Terrain Properties")]
        public double ContinentFrequency { get; set; }

        /// <summary>
        /// Lacunarity of the planet's continents.  Changing this value produces
        /// slightly different continents.  For the best results, this value should
        /// be random, but close to 2.0.
        /// </summary>
        [Description("Lacunarity of the planet's continents.  Changing this value produces " +
            "slightly different continents.  For the best results, this value should " +
            "be random, but close to 2.0.")]
        [Category("Terrain Properties")]
        public double ContinentLacunarity { get; set; }

        /// <summary>
        /// Lacunarity of the planet's mountains.  Changing this value produces
        /// slightly different mountains.  For the best results, this value should
        /// be random, but close to 2.0.
        /// </summary>
        [Description("Lacunarity of the planet's mountains.  Changing this value produces " +
            "slightly different mountains.  For the best results, this value should " +
            "be random, but close to 2.0.")]
        [Category("Terrain Properties")]
        public double MountainLacunarity { get; set; }

        /// <summary>
        /// Lacunarity of the planet's hills.  Changing this value produces slightly
        /// different hills.  For the best results, this value should be random, but
        /// close to 2.0.
        /// </summary>
        [Description("Lacunarity of the planet's hills.  Changing this value produces slightly " +
            "different hills.  For the best results, this value should be random, but " +
            "close to 2.0.")]
        [Category("Terrain Properties")]
        public double HillsLacunarity { get; set; }

        /// <summary>
        /// Lacunarity of the planet's plains.  Changing this value produces slightly
        /// different plains.  For the best results, this value should be random, but
        /// close to 2.0.
        /// </summary>
        [Description("Lacunarity of the planet's plains.  Changing this value produces slightly " +
            "different plains.  For the best results, this value should be random, but " +
            "close to 2.0.")]
        [Category("Terrain Properties")]
        public double PlainsLacunarity { get; set; }

        /// <summary>
        /// Lacunarity of the planet's badlands.  Changing this value produces
        /// slightly different badlands.  For the best results, this value should be
        /// random, but close to 2.0.
        /// </summary>
        [Description("Lacunarity of the planet's badlands.  Changing this value produces " +
            "slightly different badlands.  For the best results, this value should be " +
            "random, but close to 2.0.")]
        [Category("Terrain Properties")]
        public double BadlandsLacunarity { get; set; }

        /// <summary>
        /// Specifies the "twistiness" of the mountains.
        /// </summary>
        [Description("Specifies the \"twistiness\" of the mountains.")]
        [Category("Terrain Properties")]
        public double MountainsTwist { get; set; }

        /// <summary>
        /// Specifies the "twistiness" of the hills.
        /// </summary>
        [Description("Specifies the \"twistiness\" of the hills.")]
        [Category("Terrain Properties")]
        public double HillsTwist { get; set; }

        /// <summary>
        /// Specifies the "twistiness" of the badlands.
        /// </summary>
        [Description("Specifies the \"twistiness\" of the badlands.")]
        [Category("Terrain Properties")]
        public double BadlandsTwist { get; set; }

        /// <summary>
        /// Specifies the planet's sea level.  This value must be between -1.0
        /// (minimum planet elevation) and +1.0 (maximum planet elevation.)
        /// </summary>
        [Description("Specifies the planet's sea level.  This value must be between -1.0 " +
            "(minimum planet elevation) and +1.0 (maximum planet elevation.)")]
        [Category("Planet Properties")]
        public double SeaLevel { get; set; }

        /// <summary>
        /// Specifies the level on the planet in which continental shelves appear.
        /// This value must be between -1.0 (minimum planet elevation) and +1.0
        /// (maximum planet elevation), and must be less than SeaLevel.
        /// </summary>
        [Description("Specifies the level on the planet in which continental shelves appear. " +
            "This value must be between -1.0 (minimum planet elevation) and +1.0 " +
            "(maximum planet elevation), and must be less than SeaLevel.")]
        [Category("Planet Properties")]
        public double ShelfLevel { get; set; }

        /// <summary>
        /// Determines the amount of mountainous terrain that appears on the
        /// planet.  Values range from 0.0 (no mountains) to 1.0 (all terrain is
        /// covered in mountains).  Mountainous terrain will overlap hilly terrain.
        /// Because the badlands terrain may overlap parts of the mountainous
        /// terrain, setting MountainsAmount to 1.0 may not completely cover the
        /// terrain in mountains.
        /// </summary>
        [Description("Determines the amount of mountainous terrain that appears on the " +
            "planet.  Values range from 0.0 (no mountains) to 1.0 (all terrain is " +
            "covered in mountains).  Mountainous terrain will overlap hilly terrain. " +
            "Because the badlands terrain may overlap parts of the mountainous " +
            "terrain, setting MountainsAmount to 1.0 may not completely cover the " +
            "terrain in mountains.")]
        [Category("Terrain Properties")]
        public double MountainsAmount { get; set; }

        /// <summary>
        /// Determines the amount of hilly terrain that appears on the planet.
        /// Values range from 0.0 (no hills) to 1.0 (all terrain is covered in
        /// hills).  This value must be less than MountainsAmount.  Because the
        /// mountainous terrain will overlap parts of the hilly terrain, and
        /// the badlands terrain may overlap parts of the hilly terrain, setting
        /// HillsAmount to 1.0 may not completely cover the terrain in hills.
        /// </summary>
        [Description("Determines the amount of hilly terrain that appears on the planet. " +
            "Values range from 0.0 (no hills) to 1.0 (all terrain is covered in " +
            "hills).  This value must be less than MountainsAmount.  Because the " +
            "mountainous terrain will overlap parts of the hilly terrain, and " +
            "the badlands terrain may overlap parts of the hilly terrain, setting " +
            "HillsAmount to 1.0 may not completely cover the terrain in hills.")]
        [Category("Terrain Properties")]
        public double HillsAmount { get; set; }

        /// <summary>
        /// Determines the amount of badlands terrain that covers the planet.
        /// Values range from 0.0 (no badlands) to 1.0 (all terrain is covered in
        /// badlands.)  Badlands terrain will overlap any other type of terrain.
        /// </summary>
        [Description("Determines the amount of badlands terrain that covers the planet. " +
            "Values range from 0.0 (no badlands) to 1.0 (all terrain is covered in " +
            "badlands.)  Badlands terrain will overlap any other type of terrain.")]
        [Category("Terrain Properties")]
        public double BadlandsAmount { get; set; }

        /// <summary>
        /// Offset to apply to the terrain type definition.  Low values (less than 1.0) cause
        /// the rough areas to appear only at high elevations.  High values (greater than 2.0)
        /// cause the rough areas to appear at any elevation.  The percentage of
        /// rough areas on the planet are independent of this value.
        /// </summary>
        [Description("Offset to apply to the terrain type definition.  Low values (less than 1.0) cause " +
            "the rough areas to appear only at high elevations.  High values (greater than 2.0) " +
            "cause the rough areas to appear at any elevation.  The percentage of " +
            "rough areas on the planet are independent of this value.")]
        [Category("Terrain Properties")]
        public double TerrainOffset { get; set; }

        /// <summary>
        /// Specifies the amount of "glaciation" on the mountains.  This value
        /// should be close to 1.0 and greater than 1.0.
        /// </summary>
        [Description("Specifies the amount of \"glaciation\" on the mountains.  This value " +
            "should be close to 1.0 and greater than 1.0.")]
        [Category("Terrain Properties")]
        public double MountainGlaciation { get; set; }

        /// <summary>
        /// Scaling to apply to the base continent elevations, in planetary elevation
        /// units.
        /// </summary>
        [Description("Scaling to apply to the base continent elevations, in planetary elevation units.")]
        [Category("Terrain Properties")]
        public double ContinentHeightScale { get; set; }

        /// <summary>
        /// Maximum depth of the rivers, in planetary elevation units.
        /// </summary>
        [Description("Maximum depth of the rivers, in planetary elevation units.")]
        [Category("Terrain Properties")]
        public double RiverDepth { get; set; }

        public PlanetGenerationSettings()
        {
            NorthCoord = 90;
            WestCoord = -180;
            EastCoord = 180;
            GridWidth = 4096;
            GridHeight = 2048;
            Seed = 0;
            PlanetCircumference = 44236800;
            MinElev = -8192;
            MaxElev = 8192;
            ContinentFrequency = 1;
            ContinentLacunarity = 2.208984375;
            MountainLacunarity = 2.142578125;
            HillsLacunarity = 2.162109375;
            PlainsLacunarity = 2.314453125;
            BadlandsLacunarity = 2.212890625;
            MountainsTwist = 1;
            HillsTwist = 1;
            BadlandsTwist = 1;
            SeaLevel = 0;
            ShelfLevel = -0.375;
            MountainsAmount = 0.5;
            HillsAmount = (1.0 + MountainsAmount) / 2.0;
            BadlandsAmount = 0.03125;
            TerrainOffset = 1;
            MountainGlaciation = 1.375;
            ContinentHeightScale = (1.0 - SeaLevel) / 4.0;
            RiverDepth = 0.0234375;
        }
    }
}
