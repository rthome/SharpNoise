using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Utilities.Imaging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComplexPlanetExample
{
    public partial class GeneratorSettingsForm : Form
    {
        public PlanetGenerationSettings GeneratorSettings { get; set; }

        public GeneratorSettingsForm()
        {
            InitializeComponent();

            GeneratorSettings = new PlanetGenerationSettings();
            propertyGrid.SelectedObject = GeneratorSettings;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timeLabel.Text = string.Empty;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var generatorModule = new PlanetGenerator(GeneratorSettings).CreatePlanetModule();

            var planetBuilder = new SphereNoiseMapBuilder();
            var planetElevationMap = new NoiseMap();

            planetBuilder.SetBounds(GeneratorSettings.SouthCoord, GeneratorSettings.NorthCoord,
                GeneratorSettings.WestCoord, GeneratorSettings.EastCoord);
            planetBuilder.SetDestSize(GeneratorSettings.GridWidth, GeneratorSettings.GridHeight);

            planetBuilder.SourceModule = generatorModule;
            planetBuilder.DestNoiseMap = planetElevationMap;

            planetBuilder.Build();

            stopwatch.Stop();

            timeLabel.Text = String.Format("Planet generated in {0}", stopwatch.Elapsed.ToString());

            var degExtent = GeneratorSettings.EastCoord - GeneratorSettings.WestCoord;
            var gridExtent = (double)GeneratorSettings.GridWidth;
            var metersPerDegree = (GeneratorSettings.PlanetCircumference / 360.0);
            var resInMeters = (degExtent / gridExtent) * metersPerDegree;
            var seaLevelInMeters = (((GeneratorSettings.SeaLevel + 1) / 2.0) *
                (GeneratorSettings.MaxElev - GeneratorSettings.MinElev)) + GeneratorSettings.MinElev;

            var planetImage = new Image();
            var planetRenderer = new ImageRenderer();
            planetRenderer.SourceNoiseMap = planetElevationMap;
            planetRenderer.DestinationImage = planetImage;
            planetRenderer.AddGradientPoint (-16384.0 + seaLevelInMeters, new Color (  0,   0,   0, 255));
            planetRenderer.AddGradientPoint (    -256 + seaLevelInMeters, new Color (  6,  58, 127, 255));
            planetRenderer.AddGradientPoint (    -1.0 + seaLevelInMeters, new Color ( 14, 112, 192, 255));
            planetRenderer.AddGradientPoint (     0.0 + seaLevelInMeters, new Color ( 70, 120,  60, 255));
            planetRenderer.AddGradientPoint (  1024.0 + seaLevelInMeters, new Color (110, 140,  75, 255));
            planetRenderer.AddGradientPoint (  2048.0 + seaLevelInMeters, new Color (160, 140, 111, 255));
            planetRenderer.AddGradientPoint (  3072.0 + seaLevelInMeters, new Color (184, 163, 141, 255));
            planetRenderer.AddGradientPoint (  4096.0 + seaLevelInMeters, new Color (255, 255, 255, 255));
            planetRenderer.AddGradientPoint (  6144.0 + seaLevelInMeters, new Color (128, 255, 255, 255));
            planetRenderer.AddGradientPoint ( 16384.0 + seaLevelInMeters, new Color (  0,   0, 255, 255));
            planetRenderer.EnableLight = true;
            planetRenderer.LightContrast = 1.0 / resInMeters;
            planetRenderer.LightIntensity = 2;
            planetRenderer.LightElevation = 45;
            planetRenderer.LightAzimuth = 135;
            planetRenderer.Render();

            string filename;
            using(var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image (*.png)|*.png";
                DialogResult result;
                do
                {
                    result = sfd.ShowDialog();
                } while (result != DialogResult.OK);
                filename = sfd.FileName;
            }

            planetImage.SaveGdiBitmap(filename, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
