using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace NoiseTester
{
    public partial class NoiseControl : UserControl
    {
        public event Action SomethingChanged;

        PlaneNoiseMapBuilder m_noiseBuilder;
        ImageRenderer m_imageRenderer;

        Module m_module;

        public Module Module
        {
            get { return m_module; }
            set
            {
                m_module = value;
                propertyGrid.SelectedObject = value;
                label1.Text = value != null ? value.GetType().Name : "";
                m_noiseBuilder.SourceModule = value;
            }
        }

        public NoiseControl()
        {
            InitializeComponent();

            var map = new NoiseMap();
            var builder = new PlaneNoiseMapBuilder()
            {
                DestNoiseMap = map,
            };
            builder.SetDestSize(pictureBox.Width, pictureBox.Height);
            m_noiseBuilder = builder;

            var image = new SharpNoise.Utilities.Imaging.Image();
            var renderer = new ImageRenderer()
            {
                SourceNoiseMap = map,
                DestinationImage = image,
            };

            if (greyRadioButton.Checked)
                renderer.BuildGrayscaleGradient();
            else if (terrainRadioButton.Checked)
                renderer.BuildTerrainGradient();
            else
                throw new Exception();

            m_imageRenderer = renderer;
        }

        public void Generate(RectangleF bounds)
        {
            if (this.Module == null)
                return;

            m_noiseBuilder.SetBounds(bounds.Left, bounds.Right, bounds.Top, bounds.Bottom);
            m_noiseBuilder.Build();

            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (float v in m_noiseBuilder.DestNoiseMap.Data)
            {
                if (v < min)
                    min = v;
                if (v > max)
                    max = v;
            }

            m_imageRenderer.Render();

            var bmp = m_imageRenderer.DestinationImage.ToGdiBitmap();

            this.BeginInvoke(new Action(() =>
                {
                    minLabel.Text = String.Format("Min/Max {0:F2}/{1:F2}", min, max);

                    pictureBox.Image = bmp;
                }));
        }

        void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (SomethingChanged != null)
                SomethingChanged();
        }

        void OnCheckedChanged(object sender, EventArgs e)
        {
            if (greyRadioButton.Checked)
                m_imageRenderer.BuildGrayscaleGradient();
            else if (terrainRadioButton.Checked)
                m_imageRenderer.BuildTerrainGradient();
            else
                throw new Exception();

            if (SomethingChanged != null)
                SomethingChanged();
        }
    }
}
