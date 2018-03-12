using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge;
using Bridge.Html5;

namespace bitOxide.MarioWiiPowerup.Javascript
{

    internal static class CanvasContextExtension
    {
        public static void RoundedRectPath(this CanvasRenderingContext2D ctx, double x, double y, double w, double h, double r)
        {
            ctx.BeginPath();
            ctx.MoveTo(x + r, y);
            ctx.LineTo(x + w - r, y);
            ctx.QuadraticCurveTo(x + w, y, x + w, y + r);
            ctx.LineTo(x + w, y + h - r);
            ctx.QuadraticCurveTo(x + w, y + h, x + w - r, y + h);
            ctx.LineTo(x + r, y + h);
            ctx.QuadraticCurveTo(x, y + h, x, y + h - r);
            ctx.LineTo(x, y + r);
            ctx.QuadraticCurveTo(x, y, x + r, y);
        }
    }

    public class Program
    {
        private static HTMLCanvasElement canvas;

        [Ready]
        public static void Main()
        {
            ResetBrowserFrame();
            canvas = new HTMLCanvasElement();

            Document.Body.AppendChild(canvas);

            var app = new App(canvas);
            app.ResizeCalc();
            app.Draw();

            

            Window.AddEventListener(EventType.Resize, () =>
            {
                app.ResizeCalc();
                app.Draw();
            });
        }

        private static void ResetBrowserFrame()
        {
            Document.Body.Style.Margin = "0";
            Document.Body.Style.Padding = "0";
            Document.Body.Style.Height = "100%";
            Document.Body.Style.Overflow = Overflow.Hidden;
        }
    }
}
