using Grafika_PZ2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Point = Grafika_PZ2.Model.Point;

namespace Grafika_PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<SubstationEntity> substationEntities = new List<SubstationEntity>();
        List<NodeEntity> nodeEntities = new List<NodeEntity>();
        List<SwitchEntity> switchEntities = new List<SwitchEntity>();
        List<LineEntity> lineEntities = new List<LineEntity>();

        public MainWindow()
        { 
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeList;

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            foreach (XmlNode node in nodeList)
            {
                SubstationEntity substationobj = new SubstationEntity();

                substationobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                substationobj.Name = node.SelectSingleNode("Name").InnerText;
                substationobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                substationobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                substationEntities.Add(substationobj);
            }


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeobj = new NodeEntity();

                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                nodeEntities.Add(nodeobj);
            }

            

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchobj = new SwitchEntity();

                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                switchEntities.Add(switchobj);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity lineobj = new LineEntity();

                lineobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                lineobj.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    lineobj.IsUnderground = true;
                }
                else
                {
                    lineobj.IsUnderground = false;
                }
                lineobj.R = float.Parse(node.SelectSingleNode("R").InnerText);
                lineobj.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineobj.LineType = node.SelectSingleNode("LineType").InnerText;
                lineobj.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                lineobj.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                lineobj.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);

                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes)
                {
                    Point point = new Point();

                    point.X = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    point.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);

                    lineobj.Vertices.Add(point);
                }

                lineEntities.Add(lineobj);
            }

            double smallestX;
            double smallestY;

            smallestCoordinates(out smallestX, out smallestY);

            double largestX;
            double largestY;

            largestCoordinates(out largestX, out largestY);

            double distance = Math.Sqrt(Math.Pow(largestX - smallestX, 2) + Math.Pow(largestY - smallestY, 2));

            double scaler = distance / 1000;

            foreach (SubstationEntity s in substationEntities)
            {
                s.X -= smallestX;
                s.Y -= smallestY;

                s.X /= scaler;
                s.Y /= scaler;
            }

            foreach (NodeEntity n in nodeEntities)
            {
                n.X -= smallestX;
                n.Y -= smallestY;

                n.X /= scaler;
                n.Y /= scaler;
            }

            foreach (SwitchEntity s in switchEntities)
            {
                s.X -= smallestX;
                s.Y -= smallestY;

                s.X /= scaler;
                s.Y /= scaler;
            }

            foreach (LineEntity l in lineEntities)
            {
                foreach (Point p in l.Vertices)
                {
                    p.X -= smallestX;
                    p.Y -= smallestY;

                    p.X /= scaler;
                    p.Y /= scaler;
                }
            }
        }

        public void largestCoordinates(out double largestX, out double largestY)
        {
            largestX = substationEntities[0].X;
            largestY = substationEntities[0].Y;

            foreach(SubstationEntity s in substationEntities)
            {
                if(s.X > largestX)
                {
                    largestX = s.X;
                }

                if(s.Y > largestY)
                {
                    largestY = s.Y;
                }
            }

            foreach(NodeEntity n in nodeEntities)
            {
                if(n.X > largestX)
                {
                    largestX = n.X;
                }

                if(n.Y > largestY)
                {
                    largestY = n.Y;
                }
            }

            foreach (SwitchEntity s in switchEntities)
            {
                if (s.X > largestX)
                {
                    largestX = s.X;
                }

                if (s.Y > largestY)
                {
                    largestY = s.Y;
                }
            }

            foreach(LineEntity l in lineEntities)
            {
                foreach(Point p in l.Vertices)
                {
                    if(p.X > largestX)
                    {
                        largestX = p.X;
                    }

                    if(p.Y > largestY)
                    {
                        largestY = p.Y;
                    }
                }
            }
        }

        public void smallestCoordinates(out double smallestX, out double smallestY)
        {
            smallestX = substationEntities[0].X;
            smallestY = substationEntities[0].Y;

            foreach (SubstationEntity s in substationEntities)
            {
                if(s.X < smallestX)
                {
                    smallestX = s.X;
                }

                if(s.Y < smallestY)
                {
                    smallestY = s.Y;
                }
            }

            foreach (NodeEntity n in nodeEntities)
            {
                if(n.X < smallestX)
                {
                    smallestX = n.X;
                }

                if(n.Y < smallestY)
                {
                    smallestY = n.Y;
                }
            }

            foreach (SwitchEntity s in switchEntities)
            {
                if (s.X < smallestX)
                {
                    smallestX = s.X;
                }

                if (s.Y < smallestY)
                {
                    smallestY = s.Y;
                }
            }

            foreach (LineEntity l in lineEntities)
            {
                foreach (Point p in l.Vertices)
                {
                    if (p.X < smallestX)
                    {
                        smallestX = p.X;
                    }

                    if (p.Y < smallestY)
                    {
                        smallestY = p.Y;
                    }
                }
            }
        }
    }
}
