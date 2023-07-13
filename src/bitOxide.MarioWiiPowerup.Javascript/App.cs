using System;
using System.Collections.Generic;
using System.Globalization;
using bitOxide.MarioWiiPowerup.Core;
using bitOxide.MarioWiiPowerup.Core.ViewModel;
using Bridge;
using Bridge.Html5;

namespace bitOxide.MarioWiiPowerup.Javascript
{
    public class App
    {
        private readonly HTMLCanvasElement canvasScreen;
        private readonly CanvasRenderingContext2D ctx;

        private readonly ItemImageStore itemImages = ItemImageStore.GetDefault();
        private readonly MainPanel panelViewModel = new MainPanel();
        private readonly HTMLImageElement bgImage;
        private readonly HTMLImageElement toad;

        private bool helpActive = false;

        public App(HTMLCanvasElement screen)
        {
            this.canvasScreen = screen;
            ctx = screen.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            ctx.ImageSmoothingEnabled = true;

            bgImage = new HTMLImageElement() { Src = "img/bg.png" };
            toad = new HTMLImageElement() { Src = "img/toad.png" };

            screen.AddEventListener(EventType.Click, Clicked);
            panelViewModel.FocusBestPosition();
        }

        private void Clicked(Event e)
        {
            if (e.IsMouseEvent())
            {
                var me = (MouseEvent)e;
                int col, row;
                bool colHit, rowHit;

                HitScan(me.ClientX * devicePixelRatio, me.ClientY * devicePixelRatio, out col, out row, out colHit, out rowHit);

                if (rowHit && colHit)
                {
                    if (col == 8 && row >= 0 && row <= 2)
                    {
                        if (row == 0)
                        {
                            if (!helpActive)
                            {
                                panelViewModel.ResetBoardInfos();
                            }
                        }
                        else if (row == 1)
                        {
                            if (!helpActive)
                            {
                                panelViewModel.RemoveFocusedItem();
                            }
                        }
                        else
                        {
                            helpActive = !helpActive;
                            //panelViewModel.FocusBestPosition();
                        }
                    }
                    else if (col >= offsetBoardLeftCols && col <= (5 + offsetBoardLeftCols) && row >= 0 && row <= 2)
                    {
                        if (!helpActive)
                        {
                            var c = col - offsetBoardLeftCols;
                            var posId = row * 6 + c;
                            panelViewModel.FocusPosition(posId);
                        }
                    }
                    else if (row == 3)
                    {
                        if (!helpActive)
                        {
                            panelViewModel.ClickItem(itemButtons[col]);
                        }
                    }

                    Draw();
                }
            }
        }

        private void HitScan(double clientX, double clientY, out int column, out int row, out bool colHit, out bool rowHit)
        {
            var row1start = offsetTop;
            var row1end = row1start + boxScale;
            var row2start = row1end + distA * boxScale;
            var row2end = row2start + boxScale;
            var row3start = row2end + distA * boxScale;
            var row3end = row3start + boxScale;
            var row4start = row3end + distB * boxScale;
            var row4end = row4start + boxScale;

            row = 0;
            rowHit = false;
            column = 0;
            colHit = false;

            if (clientY >= row1start && clientY <= row1end)
            {
                row = 0;
                rowHit = true;
            }
            else if (clientY >= row2start && clientY <= row2end)
            {
                row = 1;
                rowHit = true;
            }
            else if (clientY >= row3start && clientY <= row3end)
            {
                row = 2;
                rowHit = true;
            }
            else if (clientY >= row4start && clientY <= row4end)
            {
                row = 3;
                rowHit = true;
            }

            for (int y = 0; y < 9; y++)
            {
                var colStart = offsetLeft + y * (1 + distA) * boxScale;
                var colEnd = colStart + boxScale;

                if (clientX >= colStart && clientX <= colEnd)
                {
                    column = y;
                    colHit = true;
                    return;
                }
            }
        }

        private readonly Item[] itemButtons = new Item[]
        {
            Item.Fly,
            Item.IceFlower,
            Item.FireFlower,
            Item.Mini,
            Item.Penguin,
            Item.Star,
            Item.Mushroom,
            Item.MiniBowser,
            Item.Bowser
        };

        private readonly string[] actionIcons = new string[]
        {
            "pow",
            "bullet",
            "question"
        };

        private double offsetLeft = 30;
        private double offsetTop = 30;
        private double boxScale = 167;
        private double devicePixelRatio = 0;

        private double scaleBg = 0;
        private double offsetLeftBg = 0;
        private double offsetTopBg = 0;
        private const double bgImageWidth = 1375;
        private const double bgImageHeight = 500;

        private const double iconMargin = 0.1;
        private const double iconSize = 1 - 2 * iconMargin;
        private const double distA = 0.25;
        private const double distB = 0.5;
        private const double realWidth = 9 + 9 * distA;
        private const double realHeight = 4 + 3 * distA + distB;


        private readonly int offsetBoardLeftCols = 1;

        private readonly string borderColorDefault = "#ADD8E6";
        private readonly string borderColorSet = "#191970";
        private readonly string borderColorFocus = "#3a316f";
        private readonly string borderColorSafeSpot = "#7f77ae";
        private readonly string backgroundColorSafeSpot = "#7f77ae";

        //private readonly string borderColorAction = "#555";
        private readonly string borderColorAction = "#FFF";

        public void ResizeCalc()
        {
            double devPx = 0;
            Script.Write("devPx = window.devicePixelRatio;");
            devicePixelRatio = devPx;

            var scaleX = Window.InnerWidth / realWidth;
            var scaleY = Window.InnerHeight / realHeight;

            var baseScale = Math.Min(scaleX, scaleY);
            boxScale = baseScale * devicePixelRatio;



            canvasScreen.Width = (int)Math.Round(Window.InnerWidth * devicePixelRatio);
            canvasScreen.Height = (int)Math.Round(Window.InnerHeight * devicePixelRatio);

            var bgScaleX = canvasScreen.Width / bgImageWidth;
            var bgScaleY = canvasScreen.Height / bgImageHeight;
            scaleBg = Math.Max(bgScaleX, bgScaleY);

            offsetLeftBg = (canvasScreen.Width - scaleBg * bgImageWidth) / 2;
            offsetTopBg = (canvasScreen.Height - scaleBg * bgImageHeight) / 2;

            offsetTop = (canvasScreen.Height - boxScale * realHeight) / 2 + distA / 2 * boxScale;
            offsetLeft = (canvasScreen.Width - boxScale * realWidth) / 2 + distA / 2 * boxScale;

            canvasScreen.Style.Width = Window.InnerWidth.ToString() + "px";
            canvasScreen.Style.Height = Window.InnerHeight.ToString() + "px";
        }

        public void Draw()
        {
            if (!itemImages.LoadCompleted || !bgImage.Complete || !toad.Complete)
            {
                Window.SetTimeout(Draw, 50);
                return;
            }

            ctx.Save();
            ctx.GlobalAlpha = 1f;
            ctx.FillStyle = "white";
            ctx.FillRect(0, 0, canvasScreen.Width, canvasScreen.Height);

            ctx.Save();
            ctx.GlobalAlpha = 0.6f;
            ctx.DrawImage(bgImage, offsetLeftBg, offsetTopBg, bgImage.Width * scaleBg, bgImage.Height * scaleBg);
            ctx.GlobalAlpha = 1f;
            ctx.Restore();

            ctx.ResetTransform();
            ctx.Translate(offsetLeft, offsetTop);
            ctx.Scale(boxScale, boxScale);

            DrawBoard();

            ctx.Restore();
        }

        private void DrawNumberOfSolutions()
        {
            ctx.Save();

            ctx.Translate(0.1, 0.4);
            ctx.Font = $"0.4px Arial";
            ctx.FillStyle = "#000";
            ctx.FillText($"Panels: {2}", 0, 0);
            ctx.Restore();
        }

        private void DrawBoard()
        {
            ctx.Save();
            var distFactor = 1 + distA;
            var solved = panelViewModel.SolvedBoard;
            var noMatches = panelViewModel.MatchingBoardCount < 1;

            //DrawNumberOfSolutions();

            //Draw board
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    var posId = y * 6 + x;

                    var focused = panelViewModel.FocusedPositionId == posId;

                    var icon = itemImages.GetIcon(panelViewModel.GetItemFromPosition(posId));
                    var iconWasSet = icon != null;

                    var colorSafeSpot = panelViewModel.IsPositionSafe(posId) && !iconWasSet && solved == null;

                    var color = borderColorDefault;
                    var knownBad = panelViewModel.IsPositionBad(posId);
                    var isWinPosition = panelViewModel.IsWinningPosition(posId);

                    if (icon == null)
                    {
                        if (solved != null)
                        {
                            //Get icon from solution
                            icon = itemImages.GetIcon(solved[posId]);
                        }
                        else
                        {
                            icon = itemImages.GetIcon(panelViewModel.GetDerivedItem(posId));

                            if (icon != null)
                            {
                                colorSafeSpot = false;
                            }
                        }
                    }

                    if (icon == null && isWinPosition && colorSafeSpot)
                    {
                        icon = itemImages.GetIcon("win");
                    }

                    if (focused)
                    {
                        color = borderColorFocus;
                    }
                    else if (colorSafeSpot)
                    {
                        color = borderColorSafeSpot;
                    }

                    ctx.Save();
                    ctx.Translate((offsetBoardLeftCols + x) * distFactor, y * distFactor);

                    DrawButton(
                        icon,
                        color,
                        iconWasSet ? 1.0 : 0.7,
                        bw: noMatches,
                        isSelected: focused,
                        isSafeSpot: colorSafeSpot,
                        forceBgBlack: knownBad
                    );

                    ctx.Restore();
                }
            }

            //Item selection
            for (int x = 0; x < 9; x++)
            {
                ctx.Save();
                ctx.Translate(x * distFactor, 3 + 2 * distA + distB);

                var item = itemButtons[x];
                var isRelevantItem = panelViewModel.IsItemInFocusedPosition(item);
                var alpha = isRelevantItem ? 1.0 : 0.2;

                DrawButton(itemImages.GetIcon(item), null, alpha: alpha, preventBgBlack: !isRelevantItem);

                ctx.Restore();
            }

            //Special Buttons
            for (int y = 0; y < 2; y++)
            {
                ctx.Save();
                ctx.Translate(8 * distFactor, y * distFactor);
                DrawButton(itemImages.GetIcon(actionIcons[y]), null, whitebghide: true);
                ctx.Restore();
            }

            if (helpActive)
                DrawHelpOverlay();


            {
                var y = 2;
                ctx.Save();
                ctx.Translate(8 * distFactor, y * distFactor);
                DrawButton(itemImages.GetIcon(actionIcons[y]), null, whitebghide: true);
                ctx.Restore();
            }

            ctx.Restore();
        }

        private void DrawButton(
            ScaledImage img,
            string borderColor,
            double alpha = 1,
            bool bw = false,
            bool whitebghide = false,
            bool isSafeSpot = false,
            bool isSelected = false,
            bool forceBgBlack = false,
            bool preventBgBlack = false
        )
        {
            var drawBlack = ((img != null && img.BlackBackground) || forceBgBlack) && !preventBgBlack;

            if (drawBlack)
            {
                DrawButtonFill("black");
            }
            else
            {
                if (!whitebghide)
                {
                    ctx.Save();
                    ctx.GlobalAlpha = (float)0.5;

                    if (!isSafeSpot)
                    {
                        DrawButtonFill("white");
                    }
                    else
                    {
                        DrawButtonFill(backgroundColorSafeSpot);
                    }

                    ctx.GlobalAlpha = (float)1;
                    ctx.Restore();
                }
            }

            ctx.Save();

            ctx.Translate(iconMargin, iconMargin);
            ctx.Scale(iconSize, iconSize);

            ctx.GlobalAlpha = (float)alpha;
            DrawImage(img, bw);
            ctx.Restore();

            var bc = borderColor;

            if (bw && img != null)
            {
                ctx.Save();
                ctx.GlobalAlpha = 1;
                ctx.GlobalCompositeOperation = CanvasTypes.CanvasCompositeOperationType.Saturation;
                ctx.FillStyle = "white";
                ctx.RoundedRectPath(0, 0, 1, 1, 0.2);
                ctx.Fill();
                //ctx.FillRect(0, 0, 1, 1);
                ctx.Restore();
            }

            DrawButtonBorder(bc);
        }

        private void DrawHelpOverlay()
        {
            ctx.Save();
            ctx.Save();
            ctx.ResetTransform();
            ctx.FillStyle = "white";
            ctx.GlobalAlpha = 0.5f;
            ctx.FillRect(0, 0, canvasScreen.Width, canvasScreen.Height);
            ctx.Restore();

            var width = 1.0;
            var height = width / 194 * 300;
            ctx.DrawImage(toad, 0, 0, width, height);

            var oT = offsetTop / boxScale;
            var oL = offsetLeft / boxScale;

            ctx.RoundedRectPath(1 + distA, 0, distA * 6 + 7, 3 + distA * 2, 0.2);
            //ctx.RoundedRectPath(0, 0, 1, 1, 0.2);
            ctx.GlobalAlpha = 1f;
            ctx.FillStyle = "white";
            ctx.Fill();
            ctx.LineWidth = 0.05;
            ctx.StrokeStyle = "black";
            ctx.Stroke();

            ctx.Scale(1.0 / boxScale, 1.0 / boxScale);
            ctx.FillStyle = "black";
            var fontsize = 0.25 * boxScale;
            ctx.Font = $"{fontsize.ToString(CultureInfo.InvariantCulture)}px Arial";

            var left = (int)Math.Round((1 + distA * 1.8) * boxScale);
            var top = (int)Math.Round(0.5 * boxScale);

            var txt = "Hello.\n\nThis tool can help you solve power-up panels. Open the selected field\nin-game and select the item you get in this app - repeat until the correct\nboard is found (and shown).\n\nUse the bullet to undo a step and POW to reset the board.\n\nIf the icons turn grey no solution was found.";
            var lines = txt.Split('\n');
            var lineheight = (int)Math.Round(0.3 * boxScale);

            for (int i = 0; i < lines.Length; i++)
                ctx.FillText(lines[i], left, top + lineheight * i);


            ctx.Restore();
        }

        private void DrawImage(ScaledImage img, bool blackWhite)
        {
            if (img == null) return;
            ctx.Save();
            img.TargetSize = (int)Math.Round(boxScale * iconSize * 2);
            ctx.DrawImage(img.ScaledImg, 0, 0, 1.0, 1.0);

            ctx.Restore();
        }

        private void DrawButtonFill(string color)
        {
            ctx.RoundedRectPath(0, 0, 1, 1, 0.2);
            ctx.FillStyle = color;
            ctx.Fill();
        }

        private void DrawButtonBorder(string color)
        {
            if (color == null) return;
            ctx.RoundedRectPath(0, 0, 1, 1, 0.2);
            ctx.StrokeStyle = color;
            ctx.LineWidth = 0.05;
            ctx.Stroke();
        }
    }
}
