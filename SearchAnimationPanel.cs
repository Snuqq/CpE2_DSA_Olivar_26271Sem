using System;
using System.Diagnostics;
using System.Collections.Generic;
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
        private readonly HashSet<int> visited = new HashSet<int>();
        private readonly Color[] palette = { Color.FromArgb(65, 94, 173), Color.FromArgb(86, 126, 187), Color.FromArgb(101, 111, 181) };
        private PointF from, destination, marker;
        private double duration = 400;
        private float progress = 1;
        private readonly Bitmap[] furinaSprites = new Bitmap[3];

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
            visited.Clear();
            UpdateHeight();
            progress = 1;
            Invalidate();
        }

        private PointF Position(int index)
        {
            int columns = Math.Max(1, (ClientSize.Width - 20) / 90);
            return new PointF(51 + (index % columns) * 90, 200 + (index / columns) * 205);
        }

        public void ShowStep(int index, bool isFound, int milliseconds)
        {
            from = activeIndex < 0 ? Position(Math.Max(0, index)) : marker;
            marker = from;
            if (activeIndex >= 0) visited.Add(activeIndex);
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
                from.Y + (destination.Y - from.Y) * eased - (float)Math.Sin(progress * Math.PI) * 32);
            if (progress >= 1)
            {
                clock.Stop();
                animationTimer.Stop();
            }
            Invalidate();
        }

        private void UpdateHeight()
        {
            int columns = Math.Max(1, (ClientSize.Width - 20) / 90);
            int rows = Math.Max(1, (values.Length + columns - 1) / columns);
            int height = Math.Max(390, rows * 205 + 70);
            if (Height != height) Height = height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var background = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(239, 248, 255), Color.FromArgb(230, 234, 252), 90F))
                g.FillRectangle(background, ClientRectangle);
            using (var titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (var captionFont = new Font("Segoe UI", 8F))
            using (var text = new SolidBrush(Color.FromArgb(66, 62, 86)))
            {
                g.DrawString(values.Length == 0 ? "Your search adventure starts here" :
                    activeIndex < 0 ? "Search complete" : found ? "Target found!" : "Following the search trail...",
                    titleFont, text, 16, 10);
                g.DrawString("Orange: checking    Green: found    Faded: previously checked", captionFont, text, 16, 32);
                g.DrawString("Furina model: Xenoart3d / CC BY 4.0", captionFont, text, 16, Height - 18);
                if (values.Length == 0)
                {
                    g.DrawString("Enter sorted numbers above, then press Start or Next Step.", captionFont, text, 16, 65);
                    return;
                }
                using (var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        PointF point = Position(i);
                        bool selected = i == activeIndex;
                        Color color = selected ? (found ? Color.SeaGreen : Color.DarkOrange) : palette[i % palette.Length];
                        if (visited.Contains(i) && !selected) color = Color.FromArgb(178, 181, 193);
                        using (var shadow = new SolidBrush(Color.FromArgb(24, 70, 60, 90)))
                            g.FillEllipse(shadow, point.X - 28, point.Y + 1, 56, 10);
                        using (var ground = new Pen(Color.FromArgb(216, 205, 218), 2))
                            g.DrawLine(ground, point.X - 39, point.Y + 12, point.X + 39, point.Y + 12);
                        if (selected)
                        {
                            using (var halo = new SolidBrush(Color.FromArgb(28, color)))
                                g.FillEllipse(halo, point.X - 39, point.Y - 87, 78, 99);
                        }
                        DrawFurina(g, point.X, point.Y, selected ? (found ? 2 : 1) : 0,
                            selected ? (int)(progress * 23) : ((int)(progress * 23) + i * 3) % 24);
                        DrawBadge(g, values[i].ToString(), new RectangleF(point.X - 42, point.Y - 150, 84, 25), color, center);
                        g.DrawString("Index " + i, captionFont, text,
                            new RectangleF(point.X - 40, point.Y + 16, 80, 20), center);
                        if (selected)
                        {
                            g.DrawString(found ? "FOUND" : "CHECKING", captionFont, text,
                                new RectangleF(point.X - 43, point.Y + 35, 86, 18), center);
                            if (found) DrawCelebration(g, point, progress);
                        }
                    }
                    if (activeIndex >= 0 && !found)
                    {
                        // The moving magnifying glass follows actual probe positions; values stay sorted.
                        float x = marker.X + 31, y = marker.Y - 55;
                        using (var trail = new Pen(Color.FromArgb(90, 226, 149, 73), 2))
                        {
                            trail.DashStyle = DashStyle.Dot;
                            g.DrawLine(trail, from.X + 31, from.Y - 55, x, y);
                        }
                        using (var lens = new SolidBrush(Color.FromArgb(195, 239, 251, 255)))
                            g.FillEllipse(lens, x - 9, y - 17, 18, 18);
                        using (var rim = new Pen(Color.FromArgb(100, 76, 63), 3))
                        {
                            g.DrawEllipse(rim, x - 9, y - 17, 18, 18);
                            g.DrawLine(rim, x + 6, y - 1, x + 14, y + 8);
                        }
                    }
                }
            }
        }

        private static void DrawBadge(Graphics g, string value, RectangleF box, Color color, StringFormat center)
        {
            using (var path = new GraphicsPath())
            {
                const float diameter = 16;
                path.AddArc(box.Left, box.Top, diameter, diameter, 180, 90);
                path.AddArc(box.Right - diameter, box.Top, diameter, diameter, 270, 90);
                path.AddArc(box.Right - diameter, box.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(box.Left, box.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                using (var fill = new SolidBrush(Color.White)) g.FillPath(fill, path);
                using (var border = new Pen(color, 1.5F)) g.DrawPath(border, path);
                using (var font = new Font("Segoe UI", value.Length > 8 ? 8F : 10F, FontStyle.Bold))
                using (var ink = new SolidBrush(Color.FromArgb(54, 49, 70)))
                    g.DrawString(value, font, ink, box, center);
            }
        }

        // Frames rendered from Xenoart3d's CC BY model; see Assets/Furina/ATTRIBUTION.md.
        private void DrawFurina(Graphics g, float x, float y, int action, int frame)
        {
            if (furinaSprites[action] == null)
            {
                string[] names = { "idle", "search", "found" };
                using (var stream = typeof(SearchAnimationPanel).Assembly.GetManifestResourceStream(
                    "CpE2_DSA_Olivar_26271Sem.Assets.Furina." + names[action] + ".png"))
                {
                    if (stream == null) throw new InvalidOperationException("Furina animation resource is missing.");
                    using (var image = new Bitmap(stream)) furinaSprites[action] = new Bitmap(image);
                }
            }
            var source = new Rectangle((frame % 6) * 256, (frame / 6) * 320, 256, 320);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(furinaSprites[action], new RectangleF(x - 52, y - 117, 104, 130), source, GraphicsUnit.Pixel);
        }

        private static void DrawCelebration(Graphics g, PointF point, float progress)
        {
            for (int i = 0; i < 8; i++)
            {
                double angle = i * Math.PI / 4;
                float radius = 24 + progress * 30;
                float x = point.X + (float)Math.Cos(angle) * radius;
                float y = point.Y - 48 + (float)Math.Sin(angle) * radius;
                using (var sparkle = new Pen(i % 2 == 0 ? Color.Goldenrod : Color.MediumSeaGreen, 2))
                {
                    g.DrawLine(sparkle, x - 3, y, x + 3, y);
                    g.DrawLine(sparkle, x, y - 3, x, y + 3);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (values != null) UpdateHeight();
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
                foreach (var sprite in furinaSprites) if (sprite != null) sprite.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
