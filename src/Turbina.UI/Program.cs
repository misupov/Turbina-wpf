using System;
using System.Windows;
using System.Windows.Media;
using Autofac;
using Turbina.Editors;
using Turbina.Nodes;

namespace Turbina.UI
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var application = new Application();
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Add("Dunno", new LinearGradientBrush());
            resourceDictionary.Add("Dunno1", new LinearGradientBrush(Color.FromRgb(0x35, 0x56, 0xc6), Color.FromRgb(0x20, 0x39, 0x77),
                    new Point(0, 0), new Point(0, 1)));
            resourceDictionary.Add("Dunno2", new LinearGradientBrush(Colors.Peru, Colors.Coral, new Point(0, 0), new Point(0, 1)));
            application.Resources = resourceDictionary;

            var cb = new ContainerBuilder();
            cb.RegisterModule<TurbinaModule>().RegisterModule<TurbinaNodesModule>();
            var container = cb.Build();

            application.Run(new MainWindow(container));
        }
    }
}