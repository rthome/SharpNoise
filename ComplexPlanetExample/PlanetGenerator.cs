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
            //
            // [Base-continent-definition subgroup]: Caches the output value from the
            // clamped-continent module.
            var baseContinent = new Cache
            {
                // [Clamped-continent module]: Finally, a clamp module modifies the
                // carved-continent module to ensure that the output value of this
                // subgroup is between -1.0 and 1.0.
                Source0 = new Clamp
                {
                    LowerBound = -1,
                    UpperBound = 1,
                    // [Carved-continent module]: This minimum-value module carves out chunks
                    // from the continent-with-ranges module.  It does this by ensuring that
                    // only the minimum of the output values from the scaled-carver module
                    // and the continent-with-ranges module contributes to the output value
                    // of this subgroup.  Most of the time, the minimum-value module will
                    // select the output value from the continents-with-ranges module since
                    // the output value from the scaled-carver module is usually near 1.0.
                    // Occasionally, the output value from the scaled-carver module will be
                    // less than the output value from the continent-with-ranges module, so
                    // in this case, the output value from the scaled-carver module is
                    // selected.
                    Source0 = new Min
                    {
                        // [Scaled-carver module]: This scale/bias module scales the output
                        // value from the carver module such that it is usually near 1.0.  This
                        // is required for step 5.
                        Source0 = new ScaleBias
                        {
                            Scale = 0.375,
                            Bias = 0.625,
                            // [Carver module]: This higher-frequency Perlin-noise module will be
                            // used by subsequent noise modules to carve out chunks from the mountain
                            // ranges within the continent-with-ranges module so that the mountain
                            // ranges will not be complely impassible.
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
                        // [Continent-with-ranges module]: Next, a curve module modifies the
                        // output value from the continent module so that very high values appear
                        // near sea level.  This defines the positions of the mountain ranges.
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
                            // [Continent module]: This Perlin-noise module generates the continents.
                            // This noise module has a high number of octaves so that detail is
                            // visible at high zoom levels.
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
            //
            // [Continent-definition group]: Caches the output value from the
            // clamped-continent module.  This is the output value for the entire
            // continent-definition group.
            var continentDefinition = new Cache
            {
                // [Select-turbulence module]: At this stage, the turbulence is applied
                // to the entire base-continent-definition subgroup, producing some very
                // rugged, unrealistic coastlines.  This selector module selects the
                // output values from the (unwarped) base-continent-definition subgroup
                // and the warped-base-continent-definition module, based on the output
                // value from the (unwarped) base-continent-definition subgroup.  The
                // selection boundary is near sea level and has a relatively smooth
                // transition.  In effect, only the higher areas of the base-continent-
                // definition subgroup become warped; the underwater and coastal areas
                // remain unaffected.
                Source0 = new Select
                {
                    EdgeFalloff = 0.0625,
                    LowerBound = Settings.SeaLevel - 0.0375,
                    UpperBound = Settings.SeaLevel + 1000.0375,
                    Control = baseContinent,
                    Source0 = baseContinent,
                    // [Warped-base-continent-definition module]: This turbulence module
                    // warps the output value from the intermediate-turbulence module.  This
                    // turbulence has a higher frequency, but lower power, than the
                    // intermediate-turbulence module, adding some fine detail to it.
                    Source1 = new Turbulence
                    {
                        Seed = Settings.Seed + 12,
                        Frequency = Settings.ContinentFrequency * 95.25,
                        Power = Settings.ContinentFrequency / 1019.75,
                        Roughness = 11,
                        // [Intermediate-turbulence module]: This turbulence module warps the
                        // output value from the coarse-turbulence module.  This turbulence has
                        // a higher frequency, but lower power, than the coarse-turbulence
                        // module, adding some intermediate detail to it.
                        Source0 = new Turbulence
                        {
                            Seed = Settings.Seed + 11,
                            Frequency = Settings.ContinentFrequency * 47.25,
                            Power = Settings.ContinentFrequency / 433.75,
                            Roughness = 12,
                            // [Coarse-turbulence module]: This turbulence module warps the output
                            // value from the base-continent-definition subgroup, adding some coarse
                            // detail to it.
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
            //
            // [Terrain-type-definition group]: Caches the output value from the
            // roughness-probability-shift module.  This is the output value for
            // the entire terrain-type-definition group.
            var terrainTypeDef = new Cache
            {
                // [Roughness-probability-shift module]: This terracing module sharpens
                // the edges of the warped-continent module near sea level and lowers
                // the slope towards the higher-elevation areas.  This shrinks the areas
                // in which the rough terrain appears, increasing the "rarity" of rough
                // terrain.
                Source0 = new Terrace
                {
                    ControlPoints = new List<double>
                    {
                        -1,
                        Settings.ShelfLevel + Settings.SeaLevel / 2.0,
                        1,
                    },
                    // [Warped-continent module]: This turbulence module slightly warps the
                    // output value from the continent-definition group.  This prevents the
                    // rougher terrain from appearing exclusively at higher elevations.
                    // Rough areas may now appear in the the ocean, creating rocky islands
                    // and fjords.
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
            // This subgroup generates the base-mountain elevations.  Other subgroups
            // will add the ridges and low areas to the base elevations.
            //
            // -1.0 represents low mountainous terrain and +1.0 represents high
            // mountainous terrain.
            //
            // [Mountain-base-definition subgroup]: Caches the output value from the
            // warped-mountains-and-valleys module.
            var mountainBaseDefinition = new Cache
            {
                // [Warped-mountains-and-valleys module]: This turbulence module warps
                // the output value from the coarse-turbulence module.  This turbulence
                // has a higher frequency, but lower power, than the coarse-turbulence
                // module, adding some fine detail to it.
                Source0 = new Turbulence
                {
                    Seed = Settings.Seed + 33,
                    Frequency = 21221,
                    Power = 1.0 / 120157.0 * Settings.MountainsTwist,
                    // [Coarse-turbulence module]: This turbulence module warps the output
                    // value from the mountain-and-valleys module, adding some coarse detail
                    // to it.
                    Source0 = new Turbulence
                    {
                        Seed = Settings.Seed + 32,
                        Frequency = 1337,
                        Power = 1.0 / 6730.0 * Settings.MountainsTwist,
                        Roughness = 4,
                        // [Mountains-and-valleys module]: This blender module merges the
                        // scaled-mountain-ridge module and the scaled-river-valley module
                        // together.  It causes the low-lying areas of the terrain to become
                        // smooth, and causes the high-lying areas of the terrain to contain
                        // ridges.  To do this, it uses the scaled-river-valley module as the
                        // control module, causing the low-flat module to appear in the lower
                        // areas and causing the scaled-mountain-ridge module to appear in the
                        // higher areas.
                        Source0 = new Blend
                        {
                            // [Low-flat module]: This low constant value is used by
                            // the Mountains-and-valleys module.
                            Source0 = new Constant
                            {
                                ConstantValue = -1,
                            },
                            // [Scaled-river-valley module]: Next, a scale/bias module applies a
                            // scaling factor of -2.0 to the output value from the river-valley
                            // module.  This stretches the possible elevation values because one-
                            // octave ridged-multifractal noise has a lower range of output values
                            // than multiple-octave ridged-multifractal noise.  The negative scaling
                            // factor inverts the range of the output value, turning the ridges from
                            // the river-valley module into valleys.
                            Source1 = new ScaleBias
                            {
                                Scale = 0.5,
                                Bias = 0.375,
                                // [Mountain-ridge module]: This ridged-multifractal-noise module
                                // generates the mountain ridges.
                                Source0 = new RidgedMulti
                                {
                                    Seed = Settings.Seed + 30,
                                    Frequency = 1723,
                                    Lacunarity = Settings.MountainLacunarity,
                                    OctaveCount = 4,
                                    Quality = NoiseQuality.Standard,
                                },
                            },
                            // [Scaled-mountain-ridge module]: Next, a scale/bias module scales the
                            // output value from the mountain-ridge module so that its ridges are not
                            // too high.  The reason for this is that another subgroup adds actual
                            // mountainous terrain to these ridges.
                            Control = new ScaleBias
                            {
                                Scale = -2,
                                Bias = -0.5,
                                // [River-valley module]: This ridged-multifractal-noise module generates
                                // the river valleys.  It has a much lower frequency than the mountain-
                                // ridge module so that more mountain ridges will appear outside of the
                                // valleys.  Note that this noise module generates ridged-multifractal
                                // noise using only one octave; this information will be important in the
                                // next step.
                                Source0 = new RidgedMulti
                                {
                                    Seed = Settings.Seed + 31,
                                    Frequency = 367,
                                    Lacunarity = Settings.MountainLacunarity,
                                    OctaveCount = 1,
                                    Quality = NoiseQuality.Best,
                                },
                            },
                        },
                    },
                },
            };

            // Generates the mountainous terrain that appears at high
            // elevations within the mountain ridges.
            //
            // -1.0 represents the lowest elevations and +1.0 represents the highest
            // elevations.
            //
            // [Mountain-base-definition subgroup]: Caches the output value from the
            // warped-mountains-and-valleys module.
            var mountainousHigh = new Cache
            {
                // [Warped-mountains-and-valleys module]: This turbulence module warps
                // the output value from the coarse-turbulence module.  This turbulence
                // has a higher frequency, but lower power, than the coarse-turbulence
                // module, adding some fine detail to it.
                Source0 = new Turbulence
                {
                    Seed = Settings.Seed + 42,
                    Frequency = 31511,
                    Power = 1.0 / 180371.0 * Settings.MountainsTwist,
                    Roughness = 4,
                    // [High-mountains module]: Next, a maximum-value module causes more
                    // mountains to appear at the expense of valleys.  It does this by
                    // ensuring that only the maximum of the output values from the two
                    // ridged-multifractal-noise modules contribute to the output value of
                    // this subgroup.
                    Source0 = new Max
                    {
                        // [Mountain-basis-0 module]: This ridged-multifractal-noise module,
                        // along with the mountain-basis-1 module, generates the individual
                        // mountains.
                        Source0 = new RidgedMulti
                        {
                            Seed = Settings.Seed + 40,
                            Frequency = 2371,
                            Lacunarity = Settings.MountainLacunarity,
                            OctaveCount = 3,
                            Quality = NoiseQuality.Best,
                        },
                        // [Mountain-basis-1 module]: This ridged-multifractal-noise module,
                        // along with the mountain-basis-0 module, generates the individual
                        // mountains.
                        Source1 = new RidgedMulti
                        {
                            Seed = Settings.Seed + 41,
                            Frequency = 2341,
                            Lacunarity = Settings.MountainLacunarity,
                            OctaveCount = 3,
                            Quality = NoiseQuality.Best,
                        },
                    },
                }
            };

            // Generates the mountainous terrain that appears at low
            // elevations within the river valleys.
            //
            // -1.0 represents the lowest elevations and +1.0 represents the highest
            // elevations.
            //
            // [Low-mountainous-terrain subgroup]: Caches the output value from the
            // low-moutainous-terrain module.
            var mountainousLow = new Cache
            {
                Source0 = new Multiply
                {
                    // [Lowland-basis-0 module]: This ridged-multifractal-noise module,
                    // along with the lowland-basis-1 module, produces the low mountainous
                    // terrain.
                    Source0 = new RidgedMulti
                    {
                        Seed = Settings.Seed + 50,
                        Frequency = 1381,
                        Lacunarity = Settings.MountainLacunarity,
                        OctaveCount = 8,
                        Quality = NoiseQuality.Best,
                    },
                    // [Lowland-basis-1 module]: This ridged-multifractal-noise module,
                    // along with the lowland-basis-0 module, produces the low mountainous
                    // terrain.
                    Source1 = new RidgedMulti
                    {
                        Seed = Settings.Seed + 51,
                        Frequency = 1427,
                        Lacunarity = Settings.MountainLacunarity,
                        OctaveCount = 8,
                        Quality = NoiseQuality.Best,
                    },
                },
            };

            // Generates the final mountainous terrain by combining the
            // high-mountainous-terrain subgroup with the low-mountainous-terrain
            // subgroup.
            //
            // -1.0 represents the lowest elevations and +1.0 represents the highest
            // elevations.
            //
            // [Mountainous-terrain group]: Caches the output value from the
            // glaciated-mountainous-terrain module.  This is the output value for
            // the entire mountainous-terrain group.
            var mountainousTerrain = new Cache
            {
                // [Glaciated-mountainous-terrain-module]: This exponential-curve module
                // applies an exponential curve to the output value from the scaled-
                // mountainous-terrain module.  This causes the slope of the mountains to
                // smoothly increase towards higher elevations, as if a glacier grinded
                // out those mountains.  This exponential-curve module expects the output
                // value to range from -1.0 to +1.0.
                Source0 = new Exponent
                {
                    Exp = Settings.MountainGlaciation,
                    // [Scaled-mountainous-terrain-module]: This scale/bias module slightly
                    // reduces the range of the output value from the combined-mountainous-
                    // terrain module, decreasing the heights of the mountain peaks.
                    Source0 = new ScaleBias
                    {
                        Scale = 0.8,
                        Bias = 0,
                        // [Combined-mountainous-terrain module]: Note that at this point, the
                        // entire terrain is covered in high mountainous terrain, even at the low
                        // elevations.  To make sure the mountains only appear at the higher
                        // elevations, this selector module causes low mountainous terrain to
                        // appear at the low elevations (within the valleys) and the high
                        // mountainous terrain to appear at the high elevations (within the
                        // ridges.)  To do this, this noise module selects the output value from
                        // the added-high-mountainous-terrain module if the output value from the
                        // mountain-base-definition subgroup is higher than a set amount.
                        // Otherwise, this noise module selects the output value from the scaled-
                        // low-mountainous-terrain module.
                        Source0 = new Select
                        {
                            // [Scaled-low-mountainous-terrain module]: First, this scale/bias module
                            // scales the output value from the low-mountainous-terrain subgroup to a
                            // very low value and biases it towards -1.0.  This results in the low
                            // mountainous areas becoming more-or-less flat with little variation.
                            // This will also result in the low mountainous areas appearing at the
                            // lowest elevations in this subgroup.
                            Source0 = new ScaleBias
                            {
                                Scale = 0.03125,
                                Bias = -0.96875,
                                Source0 = mountainousLow,
                            },
                            // [Added-high-mountainous-terrain module]: This addition module adds the
                            // output value from the scaled-high-mountainous-terrain module to the
                            // output value from the mountain-base-definition subgroup.  Mountains
                            // now appear all over the terrain.
                            Source1 = new Add
                            {
                                // [Scaled-high-mountainous-terrain module]: Next, this scale/bias module
                                // scales the output value from the high-mountainous-terrain subgroup to
                                // 1/4 of its initial value and biases it so that its output value is
                                // usually positive.
                                Source0 = new ScaleBias
                                {
                                    Scale = 0.25,
                                    Bias = 0.25,
                                    Source0 = mountainousHigh,
                                },
                                Source1 = mountainBaseDefinition,
                            },
                            Control = mountainBaseDefinition,
                        },
                    },
                },
            };

            return mountainousTerrain;
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
