using bitOxide.MarioWiiPowerup.Core;
using bitOxide.MarioWiiPowerup.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bitOxide.MarioWiiPowerup.WPF
{
    /// <summary>
    /// Interaktionslogik für PowerUpPanel.xaml
    /// </summary>
    public partial class PowerUpPanel : Canvas
    {
        private readonly double w = 900;
        private readonly double h = 303;

        private readonly Rect[] itemPanel = new Rect[18];
        private readonly Rect[] sideSelect = new Rect[12];

        private readonly MainPanel viewModelPanel = new MainPanel();

        private enum Icon
        {
            Unknown,
            Bowser,
            BowserJr,
            Fire,
            Ice,
            Mini,
            Penguin,
            Propeller,
            Star,
            Mushroom,
            Pow,
            QuestionMark,
            Idea,
            Delete
        }

        private Icon?[] Order = new Icon?[] {
            Icon.Ice, Icon.Fire, Icon.Mushroom, Icon.Pow,
            Icon.Penguin, Icon.Propeller, Icon.Mini,Icon.Idea,
            Icon.Star, Icon.BowserJr, Icon.Bowser,Icon.Delete,
        };

        public PowerUpPanel()
        {
            InitializeComponent();

            MinHeight = h;
            MaxHeight = h;
            Height = h;
            MinWidth = w;
            MaxWidth = w;
            Width = w;

            viewModelPanel.FocusBestPosition();
        }

        protected override void OnRender(DrawingContext dc)
        {
            //dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, ActualWidth, ActualHeight));
            DrawBoard(dc);
        }

        private Icon? GetIconFromItem(Item item)
        {
            if (item == Item.Bowser) return Icon.Bowser;
            if (item == Item.FireFlower) return Icon.Fire;
            if (item == Item.Fly) return Icon.Propeller;
            if (item == Item.IceFlower) return Icon.Ice;
            if (item == Item.Mini) return Icon.Mini;
            if (item == Item.MiniBowser) return Icon.BowserJr;
            if (item == Item.Mushroom) return Icon.Mushroom;
            if (item == Item.Penguin) return Icon.Penguin;
            if (item == Item.Star) return Icon.Star;
            return null;
        }

        private Item GetItemFromIcon(Icon? ico)
        {
            if (ico == null) return null;
            switch (ico)
            {
                case Icon.Bowser: return Item.Bowser;
                case Icon.BowserJr: return Item.MiniBowser;
                case Icon.Fire: return Item.FireFlower;
                case Icon.Ice: return Item.IceFlower;
                case Icon.Mini: return Item.Mini;
                case Icon.Mushroom: return Item.Mushroom;
                case Icon.Penguin: return Item.Penguin;
                case Icon.Propeller: return Item.Fly;
                case Icon.Star: return Item.Star;

                default: return null;
            }
        }

        private void DrawBoard(DrawingContext dc)
        {
            double rand = 20;
            double offset = 10;
            double item_offset = 20;
            double item_size = (w - 1 * offset - 12 * item_offset - 2 * rand) / 10;

            double width_main = 7 * item_offset + 6 * item_size;
            double width_side = 5 * item_offset + 4 * item_size;
            double rounding = item_size / 10.0;

            double height = 4 * item_offset + 3 * item_size;
            var full_height = height + 2 * rand;

            Brush Background = Brushes.WhiteSmoke;
            Pen p = new Pen(Brushes.LightBlue, 5);

            dc.DrawRoundedRectangle(Background, p, new Rect(rand, rand, width_main, height), rounding, rounding);
            //dc.DrawRoundedRectangle(Background, p, new Rect(offset + width_main + rand, rand, width_side, height), rounding, rounding);

            var qui = new Func<double, double>((double c) => { return c * (item_size + item_offset) + item_offset; });

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var num = y * 6 + x;
                    var xx = qui(x) + rand;
                    var yy = qui(y) + rand;
                    var rect = new Rect(xx, yy, item_size, item_size);
                    itemPanel[num] = rect;
                    var icon = GetIconFromItem(viewModelPanel.GetItemFromPosition(num));
                    var halfTransparent = false;

                    if (icon == null && viewModelPanel.SolvedBoard != null)
                    {
                        icon = GetIconFromItem(viewModelPanel.SolvedBoard[num]);
                        halfTransparent = true;
                    }

                    DrawItemFrame(
                        dc,
                        xx,
                        yy,
                        item_size,
                        num == viewModelPanel.FocusedPositionId,
                        icon,
                        halfTransparent,
                        viewModelPanel.IsPositionSafe(num)
                    );
                }
            }

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var num = y * 4 + x;
                    var xx = qui(x) + width_main + offset + rand;
                    var yy = qui(y) + rand;
                    var rect = new Rect(xx, yy, item_size, item_size);
                    sideSelect[num] = rect;

                    DrawItemFrame(
                        dc,
                        xx,
                        yy,
                        item_size,
                        selected: false,
                        Order[y * 4 + x],
                        halfTransparent: false,
                        isSafeSpot: false
                    );
                }
            }

            //dc.DrawRoundedRectangle(Brushes.WhiteSmoke, p, new Rect(offset, 2 * offset + height, width_main, low_height), rounding, rounding);

            //var mini_cnt = 10;
            //var mini_offset_v = 25.0;
            //var mini_size = low_height - 2 * mini_offset_v;
            //var mini_offset_h = (width_main - mini_cnt * mini_size) / (double)(mini_cnt + 1);

            //for (int i = 0; i < mini_cnt; i++)
            //{
            //    DrawMiniFrame(dc, offset + mini_offset_h + i * (mini_offset_h + mini_size), 2 * offset + height + mini_offset_v, mini_size);
            //}

        }

        private void DrawMiniFrame(DrawingContext dc, double x, double y, double size)
        {
            double rounding = size / 10.0;


            Brush Background = Brushes.Lavender;
            Pen p = null;// new Pen(selected ? Brushes.Red : Brushes.LightBlue, 5);

            if (size > 0)
                dc.DrawRoundedRectangle(Background, p, new Rect(x, y, size, size), rounding, rounding);
        }

        private static readonly Dictionary<Icon, BitmapImage> imgCache = new Dictionary<Icon, BitmapImage>();

        static PowerUpPanel()
        {
            foreach (Icon i in Enum.GetValues(typeof(Icon)))
                imgCache.Add(i, GetImageFromIcon(i));
        }

        private static BitmapImage GetImageFromIcon(Icon ico)
        {
            var asm = Assembly.GetExecutingAssembly();
            var iconName = GetStreamNameFromIcon(ico);

            if (iconName == null)
                return null;

            var str = "bitOxide.MarioWiiPowerup.WPF.icons." + iconName;

            using (var imgStream = asm.GetManifestResourceStream(str))
            {
                if (imgStream == null) throw new ResourceReferenceKeyNotFoundException();
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = imgStream;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
        }

        private static string GetStreamNameFromIcon(Icon ico)
        {
            switch (ico)
            {
                case Icon.QuestionMark: return "sm_question.png";
                case Icon.Idea: return "sm_idea.png";
                case Icon.Pow: return "sm_pow.png";
                case Icon.Delete: return "sm_delete.png";

                case Icon.Ice: return "sm_ice.png";
                case Icon.Fire: return "sm_fire.png";
                case Icon.Bowser: return "sm_bowser.png";
                case Icon.BowserJr: return "sm_bowserjr.png";
                case Icon.Mini: return "sm_mini.png";
                case Icon.Mushroom: return "sm_super.png";
                case Icon.Propeller: return "sm_propeller.png";
                case Icon.Star: return "sm_star.png";
                case Icon.Penguin: return "sm_penguin.png";

                case Icon.Unknown:
                default:
                    return null;
            }

        }


        private void DrawItemFrame(
            DrawingContext dc,
            double x,
            double y,
            double size,
            bool selected,
            Icon? icon,
            bool halfTransparent,
            bool isSafeSpot
        )
        {
            double rounding = size / 10.0;
            double thickness = Math.Max(1, size / 20.0);
            double img_offset = size / 5.0;

            Brush Background = Brushes.White;
            Brush penBrush = Brushes.LightBlue;

            if (selected)
            {
                penBrush = Brushes.Red;
            }
            else if (isSafeSpot)
            {
                penBrush = Brushes.Green;
            }

            Pen p = new Pen(penBrush, thickness);

            dc.DrawRoundedRectangle(Background, p, new Rect(x, y, size, size), rounding, rounding);

            if (icon != null && imgCache.ContainsKey(icon.Value))
            {
                var bi = imgCache[icon.Value];
                if (halfTransparent) dc.PushOpacity(0.5);
                dc.DrawImage(bi, new Rect(x + img_offset, y + img_offset, size - 2 * img_offset, size - 2 * img_offset));
                if (halfTransparent) dc.Pop();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);

            var iPanelRect = findHit(itemPanel, p);
            var sideRect = findHit(sideSelect, p);

            if (iPanelRect != null)
            {
                viewModelPanel.FocusPosition(iPanelRect.Item1);
                this.InvalidateVisual();
            }
            else if (sideRect != null)
            {
                var selIcon = Order[sideRect.Item1];
                if (selIcon == Icon.Pow)
                {
                    viewModelPanel.ResetBoardInfos();
                    this.InvalidateVisual();
                }
                else if (selIcon == Icon.Idea)
                {
                    viewModelPanel.FocusBestPosition();
                    InvalidateVisual();
                }
                else if (selIcon == Icon.Delete)
                {
                    viewModelPanel.RemoveFocusedItem();
                    this.InvalidateVisual();
                }
                else
                {
                    var item = GetItemFromIcon(selIcon);
                    if (item != null)
                    {
                        viewModelPanel.ClickItem(item);
                        this.InvalidateVisual();
                    }
                }
            }
        }

        private static Tuple<int, Rect> findHit(IEnumerable<Rect> rects, Point p)
        {
            return rects.Select((r, i) => new Tuple<int, Rect>(i, r)).Where(g => g.Item2.Contains(p)).FirstOrDefault();
        }
    }
}
