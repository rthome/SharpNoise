using SharpNoise;
using SharpNoise.Modules;
using System;
using System.Collections.Generic;

namespace ComplexPlanetExample
{
    public class PlanetGenerator
    {
        public PlanetGenerationSettings Settings { get; set; }

        public PlanetGenerator(PlanetGenerationSettings settings)
        {
            Settings = settings;
        }

        #region Module Groups

        Module CreateContinentDefinition()
        {
            // Roughly defines the positions and base elevations of the planet's continents.
            //
            // The "base elevation" is the elevation of the terrain before any terrain
            // features (mountains, hills, etc.) are placed on that terrain.
            //
            // -1.0 represents the lowest elevations and +1.0 represents the highest
            // elevations.
            var baseContinent = new Cache
            {
                Source0 = new Clamp
                {
                    LowerBound = -1,
                    UpperBound = 1,
                    Source0 = new Min
                    {
                        Source0 = new ScaleBias
                        {
                            Scale = 0.375,
                            Bias = 0.625,
                            Source0 = new Perlin
                            {
                                Seed = Settings.Seed + 1,
                                Frequency = Settings.ContinentFrequency * 4.34375,
                                Persistence = 0.5,
                                Lacunarity = Settings.ContinentLacunarity,
                                OctaveCount = 11,
                                Quality = NoiseQuality.Standard,
                            },
                        },
                        Source1 = new Curve
                        {
                            ControlPoints = new List<Curve.ControlPoint>
                            {
                                new Curve.ControlPoint(-2.0000 + Settings.SeaLevel, -1.625 + Settings.SeaLevel),
                                new Curve.ControlPoint(-1.0000 + Settings.SeaLevel, -1.375 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.0000 + Settings.SeaLevel, -0.375 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.0625 + Settings.SeaLevel,  0.125 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.1250 + Settings.SeaLevel,  0.250 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.2500 + Settings.SeaLevel,  1.000 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.5000 + Settings.SeaLevel,  0.250 + Settings.SeaLevel),
                                new Curve.ControlPoint( 0.7500 + Settings.SeaLevel,  0.250 + Settings.SeaLevel),
                                new Curve.ControlPoint( 1.0000 + Settings.SeaLevel,  0.500 + Settings.SeaLevel),
                                new Curve.ControlPoint( 2.0000 + Settings.SeaLevel,  0.500 + Settings.SeaLevel),
                            },
                            Source0 = new Perlin
                            {
                                Seed = Settings.Seed + 0,
                                Frequency = Settings.ContinentFrequency,
                                Persistence = 0.5,
                                Lacunarity = Settings.ContinentLacunarity,
                                OctaveCount = 14,
                                Quality = NoiseQuality.Standard,
                            },
                        },
                    },
                },
            };

            // Warps the output value from the the base-continent-
            // definition module, producing more realistic terrain.
            //
            // Warping the base continent definition produces lumpier terrain with
            // cliffs and rifts.
            //
            // -1.0 represents the lowest elevations and +1.0 represents the highest
            // elevations.
            var continentDefinition = new Cache
            {
                Source0 = new Select
                {
                    EdgeFalloff = 0.0625,
                    LowerBound = Settings.SeaLevel - 0.0375,
                    UpperBound = Settings.SeaLevel + 1000.0375,
                    Control = baseContinent,
                    Source0 = baseContinent,
                    Source1 = new Turbulence
                    {
                        Seed = Settings.Seed + 12,
                        Frequency = Settings.ContinentFrequency * 95.25,
                        Power = Settings.ContinentFrequency / 1019.75,
                        Roughness = 11,
                        Source0 = new Turbulence
                        {
                            Seed = Settings.Seed + 11,
                            Frequency = Settings.ContinentFrequency * 47.25,
                            Power = Settings.ContinentFrequency / 433.75,
                            Roughness = 12,
                            Source0 = new Turbulence
                            {
                                Seed = Settings.Seed + 10,
                                Frequency = Settings.ContinentFrequency * 15.25,
                                Power = Settings.ContinentFrequency / 113.75,
                                Roughness = 12,
                                Source0 = baseContinent,
                            },
                        },
                    },
                },
            };

            return continentDefinition;
        }


        Module CreateTerrainTypeDefinition(Module continentDefinition)
        {
            // Defines the positions of the terrain types on the planet.
            //
            // Terrain types include, in order of increasing roughness, plains, hills,
            // and mountains.
            //
            // This subgroup's output value is based on the output value from the
            // continent-definition group.  Rougher terrain mainly appears at higher
            // elevations.
            //
            // -1.0 represents the smoothest terrain types (plains and underwater) and
            // +1.0 represents the roughest terrain types (mountains).
            var terrainTypeDef = new Cache
            {
                Source0 = new Terrace
                {
                    ControlPoints = new List<double>
                    {
                        -1,
                        Settings.ShelfLevel + Settings.SeaLevel / 2.0,
                        1,
                    },
                    Source0 = new Turbulence
                    {
                        Seed = Settings.Seed + 20,
                        Frequency = Settings.ContinentFrequency * 18.125,
                        Power = Settings.ContinentFrequency / 20.59375 * Settings.TerrainOffset,
                        Roughness = 3,
                        Source0 = continentDefinition,
                    },
                },
            };

            return terrainTypeDef;
        }

        Module CreateMountainousTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateHillyTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreatePlainsTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateBadlandsTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateRiverPositions()
        {
            throw new NotImplementedException();
        }

        Module CreateScaledMountainousTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateScaledHillyTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateScaledPlainsTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateScaledBadlandsTerrain()
        {
            throw new NotImplementedException();
        }

        Module CreateFinalPlanet()
        {
            throw new NotImplementedException();
        }

        #endregion

        public Module CreatePlanetModule()
        {
            throw new NotImplementedException();
        }
    }
}
