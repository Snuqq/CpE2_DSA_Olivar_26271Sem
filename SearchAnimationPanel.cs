using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CpE2_DSA_Olivar_26271Sem
{
    public class SearchAnimationPanel : Panel
    {
        private readonly Timer animationTimer = new Timer();
        private readonly Stopwatch clock = new Stopwatch();
        private int[] values = new int[0];
        private int activeIndex = -1;
        private bool found;
        private PointF from, destination, marker;
        private double duration = 400;
        private float progress = 1;

        public SearchAnimationPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            animationTimer.Interval = 16;
            animationTimer.Tick += Animate;
        }

        public void SetValues(int[] numbers)
        {
            animationTimer.Stop();
            clock.Reset();
            values = numbers;
            activeIndex = -1;
            found = false;
            progress = 1;
            Invalidate();
        }

        private PointF Position(int index)
        {
            int columns = Math.Max(1, ClientSize.Width / 90);
            return new PointF(51 + (index % columns) * 90, 38 + (index / columns) * 65);
        }

        public void ShowStep(int index, bool isFound, int milliseconds)
        {
            from = activeIndex < 0 ? Position(Math.Max(0, index)) : marker;
            marker = from;
            activeIndex = index;
            found = isFound;
            if (index < 0)
            {
                animationTimer.Stop();
                clock.Reset();
                Invalidate();
                return;
            }
            destination = Position(index);
            duration = Math.Max(80, milliseconds);
            progress = 0;
            clock.Restart();
            animationTimer.Start();
            Invalidate();
        }

        public void PauseAnimation()
        {
            clock.Stop();
            animationTimer.Stop();
        }

        public void ResumeAnimation()
        {
            if (activeIndex < 0 || progress >= 1) return;
            clock.Start();
            animationTimer.Start();
        }

        private void Animate(object sender, EventArgs e)
        {
            if (!animationTimer.Enabled) return;
            progress = Math.Min(1, (float)(clock.Elapsed.TotalMilliseconds / duration));
            float eased = progress * progress * (3 - 2 * progress);
            marker = new PointF(from.X + (destination.X - from.X) * eased,
                from.Y + (destination.Y - from.Y) * eased - (float)Math.Sin(progress * Math.PI) * 24);
            if (progress >= 1)
            {
                clock.Stop();
                animationTimer.Stop();
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int i = 0; i < values.Length; i++)
                {
                    PointF point = Position(i);
                    Brush color = i == activeIndex ? (found ? Brushes.SeaGreen : Brushes.DarkOrange) : Brushes.SteelBlue;
                    float pulse = i == activeIndex && found ? (float)Math.Sin(progress * Math.PI) * 5 : 0;
                    e.Graphics.FillRectangle(color, point.X - 40 - pulse, point.Y - 14 - pulse, 80 + pulse * 2, 30 + pulse * 2);
                    e.Graphics.DrawString(values[i].ToString(), Font, Brushes.White,
                        new RectangleF(point.X - 40, point.Y - 14, 80, 30), center);
                    e.Graphics.DrawString("Index " + i, Font, Brushes.Black,
                        new RectangleF(point.X - 40, point.Y + 18, 80, 20), center);
                }
                if (activeIndex >= 0)
                {
                    float x = marker.X, y = marker.Y - 26;
                    e.Graphics.FillEllipse(Brushes.Gainsboro, x - 12, marker.Y - 12, 24, 5);
                    e.Graphics.FillEllipse(found ? Brushes.SeaGreen : Brushes.DarkOrange, x - 10, y - 18, 20, 20);
                    e.Graphics.FillEllipse(Brushes.White, x - 6, y - 12, 4, 4);
                    e.Graphics.FillEllipse(Brushes.White, x + 2, y - 12, 4, 4);
                    e.Graphics.DrawArc(Pens.White, x - 4, y - 8, 8, 6, 0, 180);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (activeIndex >= 0)
            {
                destination = Position(activeIndex);
                if (progress >= 1) marker = destination;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer.Dispose();
                clock.Stop();
            }
            base.Dispose(disposing);
        }
    }
}
