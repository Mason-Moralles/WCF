using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OrderDashboard.OrderServiceRef;

namespace OrderDashboard.Controls
{
    public partial class PieChart : UserControl
    {
        public PieChart()
        {
            InitializeComponent();
        }

        public void Draw(List<OrderStatus> data)
        {
            CanvasRoot.Children.Clear();

            if (data == null || data.Count == 0)
                return;

            double total = 0;
            foreach (var item in data)
                total += item.Count;

            double startAngle = 0;
            double centerX = 150;
            double centerY = 150;
            double radius = 120;

            Random rnd = new Random();

            foreach (var item in data)
            {
                double sweepAngle = item.Count / total * 360;

                PathFigure pf = new PathFigure();
                pf.StartPoint = new Point(centerX, centerY);

                double x = centerX + radius * Math.Cos(Math.PI * startAngle / 180);
                double y = centerY + radius * Math.Sin(Math.PI * startAngle / 180);

                pf.Segments.Add(new LineSegment(new Point(x, y), true));

                x = centerX + radius * Math.Cos(Math.PI * (startAngle + sweepAngle) / 180);
                y = centerY + radius * Math.Sin(Math.PI * (startAngle + sweepAngle) / 180);

                pf.Segments.Add(new ArcSegment(
                    new Point(x, y),
                    new Size(radius, radius),
                    sweepAngle,
                    sweepAngle > 180,
                    SweepDirection.Clockwise,
                    true));

                pf.Segments.Add(new LineSegment(new Point(centerX, centerY), true));

                PathGeometry pg = new PathGeometry();
                pg.Figures.Add(pf);

                Path path = new Path();
                path.Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)rnd.Next(50, 200),
                    (byte)rnd.Next(50, 200),
                    (byte)rnd.Next(50, 200)));

                path.Data = pg;

                CanvasRoot.Children.Add(path);

                startAngle += sweepAngle;
            }
        }
    }
}
