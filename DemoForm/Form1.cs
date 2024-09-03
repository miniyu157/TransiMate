using TransiMate;

namespace DemoForm
{
    public partial class Form1 : Form
    {
        public class RectangleInterpolator : IInterpolatorStrategy
        {
            public object Interpolate(object startValue, object endValue, double progress)
            {
                Rectangle start = (Rectangle)startValue;
                Rectangle end = (Rectangle)endValue;
                int newX = start.X + (int)((end.X - start.X) * progress);
                int newY = start.Y + (int)((end.Y - start.Y) * progress);
                int newWidth = start.Width + (int)((end.Width - start.Width) * progress);
                int newHeight = start.Height + (int)((end.Height - start.Height) * progress);
                return new Rectangle(newX, newY, newWidth, newHeight);
            }
        }

        public Form1()
        {
            InitializeComponent();

            button1.Click += Button1_Click;

            TypeInterpolator.RegisterStrategy<Rectangle>(new RectangleInterpolator());
        }

        bool to = false;
        CancellationTokenSource cts = new();

        private void Button1_Click(object? sender, EventArgs e)
        {
            to = !to;

            cts.Cancel();
            cts = new();

            // Position transition
            _ = TransiMateTick.Start(
                panel1.Left,                                            // start value
                to == true ? 442 : 67,                                  // end value
                new AnimationInfo(500, 120, EasingType.EaseInOutCubic), // animation info (use preset)
                value => panel1.Left = value,                           // set value
                cts.Token);                                             // cancel token

            // Color transition
            _ = TransiMateTick.Start(
                panel1.BackColor,                                       // start value
                to == true ? Color.LightGreen : Color.LightBlue,        // end value
                new AnimationInfo(500, 30, "0, 0, 1, 1"),               // animation info (use specified Bezier control points)
                value => panel1.BackColor = value,                      // set value
                cts.Token);                                             // cancel token

            // Size transition with custom easing function
            _ = TransiMateTick.Start(
                panel1.Size,                                            // start value
                to == true ? new Size(150, 150) : new Size(80, 80),     // end value
                500,                                                    // duration (ms)
                120,                                                    // delay (ms)
                p => EaseInOutCubic(p),                                 // custom easing function
                value => panel1.Size = value,                           // set value
                cts.Token);                                             // cancel token
        }

        static double EaseInOutCubic(double progress)
        {
            if (progress < 0.5)
            {
                return 4 * progress * progress * progress;
            }
            else
            {
                double f = ((2 * progress) - 2);
                return 0.5 * f * f * f + 1;
            }
        }
    }
}
