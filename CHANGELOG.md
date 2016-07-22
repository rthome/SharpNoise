# SharpNoise Changelog

## 0.11.0 (in development)

- Moved project to GitHub (from Bitbucket)
- Rewrite and clean up unit tests with xUnit instead of MSTest, making them run on Mono.
- Improve test coverage with new unit tests
- Continuous Integration with Travis CI for Linux/Mono and AppVeyor for Windows/.NET
- Some code refactoring to use C# 6 features
- Move sample applications into a separate solution
- Cache module: Works now after deserialization and implements IDisposable

## 0.10.0

- NoiseMapBuilders now run builds in parallel, utilizing all available CPUs & cores. 
- New 'OpenGLExample' example application, which shows how to use SharpNoise in a realtime scenario, to deform a rotating sphere. 
- New 'NoiseTester' example application (still in development) which shows a hierarchical view of a tree of noise modules and their outputs. 
- New 3D NoiseCube and NoiseCubeBuilders. 
- Filtering for NoiseMaps and NoiseCubes, allowing up- and downscaling of noise data. 
- Some code cleanups and small optimizations 
- Fix some bugs, notably in the Select module
