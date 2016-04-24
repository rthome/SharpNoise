using OpenTK;
using System;
using System.Windows.Forms;

namespace OpenGLExample
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (var window = new RenderWindow())
            {
                window.Run();
            }
        }
    }
}
