using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Module = SharpNoise.Modules.Module;

namespace NoiseTester
{
    public partial class MainForm : Form
    {
        List<NoiseControl> m_controls = new List<NoiseControl>();

        RectangleF m_bounds = new RectangleF(6, 1, 4, 4);

        class ModuleData
        {
            public int MaxSources;
            public int Depth;
            public NoiseControl Control;
        }

        public MainForm()
        {
            InitializeComponent();

            this.Size = new System.Drawing.Size(1200, 800);

            SetupNumericUpDowns();

            SetupTable();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GenerateAll();
        }

        void SetupNumericUpDowns()
        {
            xNumericUpDown.Value = (decimal)m_bounds.X;
            xNumericUpDown.ValueChanged += (s, e) =>
            {
                m_bounds.X = (float)xNumericUpDown.Value;
                GenerateAll();
            };

            wNumericUpDown.Value = (decimal)m_bounds.Width;
            wNumericUpDown.ValueChanged += (s, e) =>
            {
                m_bounds.Width = (float)wNumericUpDown.Value;
                GenerateAll();
            };

            yNumericUpDown.Value = (decimal)m_bounds.Y;
            yNumericUpDown.ValueChanged += (s, e) =>
            {
                m_bounds.Y = (float)yNumericUpDown.Value;
                GenerateAll();
            };

            hNumericUpDown.Value = (decimal)m_bounds.Height;
            hNumericUpDown.ValueChanged += (s, e) =>
            {
                m_bounds.Height = (float)hNumericUpDown.Value;
                GenerateAll();
            };
        }

        void SetupTable()
        {
            var tree = Noise.CreateNoiseTree();

            var map = new Dictionary<Module, ModuleData>();
            int rows = WalkTree(tree, map);

            // Add an extra column & row with autosize to make layout work better
            tableLayoutPanel1.ColumnCount = map[tree].MaxSources + 1;
            tableLayoutPanel1.RowCount = rows + 1;

            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            tableLayoutPanel1.CellPaint += OnCellPaint;

            LayoutTree(tree, map, rows - 1);

            foreach (var ctrl in m_controls)
                ctrl.SomethingChanged += OnSomethingChanged;
        }

        void OnCellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            var c = (NoiseControl)tableLayoutPanel1.GetControlFromPosition(e.Column, e.Row);

            if (c == null)
                return;

            int column = tableLayoutPanel1.GetColumn(c);

            if (e.Column != column)
                return;

            int xSpan = tableLayoutPanel1.GetColumnSpan(c);

            Graphics g = e.Graphics;

            int x1 = e.CellBounds.X;
            int y1 = e.CellBounds.Y;
            int x2 = e.CellBounds.X + e.CellBounds.Width * xSpan;
            int y2 = e.CellBounds.Y + e.CellBounds.Height;

            g.DrawLine(Pens.Red, x1, y1, x1, y2);
            g.DrawLine(Pens.Red, x2, y1, x2, y2);
            if (c.Module.SourceModuleCount == 0)
                g.DrawLine(Pens.Red, x1, y1, x2, y1);
        }

        int WalkTree(Module module, Dictionary<Module, ModuleData> map, int depth = 0)
        {
            int maxChildSources = 0;
            int childDepth = 0;

            foreach (var source in module.SourceModules)
            {
                int d = WalkTree(source, map, depth + 1);

                if (d > childDepth)
                    childDepth = d;

                maxChildSources += map[source].MaxSources;
            }

            var nc = new NoiseControl()
            {
                Module = module,
            };

            map[module] = new ModuleData()
            {
                MaxSources = Math.Max(maxChildSources, 1),
                Depth = depth,
                Control = nc,
            };

            m_controls.Add(nc);

            return childDepth + 1;
        }

        void LayoutTree(Module module, Dictionary<Module, ModuleData> map, int row, int startColumn = 0)
        {
            var nc = map[module].Control;

            tableLayoutPanel1.Controls.Add(nc);
            int column = startColumn;
            tableLayoutPanel1.SetCellPosition(nc, new TableLayoutPanelCellPosition(column, row));

            int xSpan = map[module].MaxSources;
            if (xSpan == 0)
                xSpan = 1;

            tableLayoutPanel1.SetColumnSpan(nc, xSpan);

            int totalWidth = tableLayoutPanel1.GetColumnWidths().Skip(startColumn).Take(xSpan).Sum();

            nc.Anchor = AnchorStyles.None;

            foreach (var source in module.SourceModules)
            {
                LayoutTree(source, map, row - 1, startColumn);

                startColumn += map[source].MaxSources;
            }
        }

        void GenerateAll()
        {
            Parallel.ForEach(m_controls, ctrl =>
            {
                ctrl.Generate(m_bounds);
            });
        }

        void OnSomethingChanged()
        {
            GenerateAll();
        }
    }
}
