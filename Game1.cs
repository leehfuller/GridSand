using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GridSand
{
    public class colourTiles
    {
        public float X;
        public float Y;
        public Color C;

        public colourTiles(float x, float y, Color c)
        {
            X = x;
            Y = y;
            C = c;
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font1;
        Texture2D tLine;

        Texture2D tRaster;
        Color[] rasterColors = null;
        Color lineColor = Color.Black;
        Color screenLines = Color.Black;
        Color selectedTile = Color.White;

        private List<Vector2> selectTiles = new List<Vector2>();
        private List<Vector2> wobbleTiles = new List<Vector2>();
        private List<Vector2> adjacentTiles = new List<Vector2>();

        private List<Vector2> hideTiles = new List<Vector2>();
        private List<colourTiles> colouredTiles = new List<colourTiles>();
        Random randomGrid = new Random();

        List<Vector2> icons = new List<Vector2>();

        Texture2D tHex1;
        Color[] drawColors = null;

        Texture2D hexagon1;
        Texture2D hexagon2;
        Texture2D triangle1;
        Texture2D triangle2;
        Texture2D square1;

        Texture2D[] spriteSymbols = null;

        int gridstyle = 5;

        bool checkeredOn = true;
        bool showRaster = false;
        bool showNumbers = false;
        bool attractOn = false;
        bool selectionBot = false;

        MouseState oldState;
        Vector2 selectClick = new Vector2(-99, -99);
        Vector2 selectTile = new Vector2(-99, -99);

        DateTime whenClicked = DateTime.Now;
        DateTime lastBotMove = DateTime.Now;
        Vector2 botDirection = new Vector2(1, 0);

        bool startRotateRight = false;
        bool startRotateLeft = false;
        bool seeThroughSelected = false;
        bool ghostClick = false; 
        float rotateMe = 0.0f;
        float wobbleAngle = 0.0f;
        float wobbleNow = 0.0f;

        int tX = 30;
        int tY = 30;

        int gX = 50;
        int gY = 50;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;

            graphics.PreferredBackBufferWidth = 1366;  //1024  //1366 // 1440
            graphics.PreferredBackBufferHeight = 768;  //768

            graphics.IsFullScreen = true;

            IsMouseVisible = true;           

            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = Content.Load<SpriteFont>("font1");

            tLine = new Texture2D(graphics.GraphicsDevice, 2, 2);
            Color[] rasterLine = new Color[tLine.Width * tLine.Height];
            for (int i = 0; i < rasterLine.Length; i++) rasterLine[i] = Color.White;
            tLine.SetData<Color>(rasterLine);

            tRaster = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            rasterColors = new Color[tRaster.Width * tRaster.Height];

            tHex1 = new Texture2D(GraphicsDevice, 50, 50);
            drawColors = new Color[50 * 50];

            //selectTiles.Add(new Vector2(0, 0));

            for (int j = 0; j < tHex1.Height; j++)
            {
                lineColor = Color.Yellow;

                for (int k = 0; k < tHex1.Width; k++)
                {
                    drawColors[(j * tHex1.Width) + k] = lineColor;
                }
            }

            //lineColor = Color.Black;
            //foreach (Vector2 p in points)
            //{
            //    drawColors[(int)p.X * (int)p.Y] = lineColor;
            //}

            tHex1.SetData<Color>(drawColors);

            hexagon1 = Content.Load<Texture2D>("Hex7");
            hexagon2 = Content.Load<Texture2D>("Hex1b");
            triangle1 = Content.Load<Texture2D>("Tri1");
            triangle2 = Content.Load<Texture2D>("Tri2");
            square1 = Content.Load<Texture2D>("Sqr1");

            spriteSymbols = new Texture2D[20];
            spriteSymbols[0] = Content.Load<Texture2D>("chess_pieces_black");
            spriteSymbols[1] = Content.Load<Texture2D>("chess_pieces_white");
            spriteSymbols[2] = Content.Load<Texture2D>("chipWhite_sideBorder");
            spriteSymbols[3] = Content.Load<Texture2D>("chipWhiteBlue_side");
            spriteSymbols[4] = Content.Load<Texture2D>("chipWhite_sideBorder");
            spriteSymbols[5] = Content.Load<Texture2D>("clayHex");
            spriteSymbols[6] = Content.Load<Texture2D>("desertHex");
            spriteSymbols[7] = Content.Load<Texture2D>("Gem_Star (12)");
            spriteSymbols[8] = Content.Load<Texture2D>("oreHex");
            spriteSymbols[9] = Content.Load<Texture2D>("pieceBlack_multi03");
            spriteSymbols[10] = Content.Load<Texture2D>("pieceBlack_multi07");
            spriteSymbols[11] = Content.Load<Texture2D>("pieceBlack_multi09");
            spriteSymbols[12] = Content.Load<Texture2D>("pieceBlack_multi17");
            spriteSymbols[13] = Content.Load<Texture2D>("pieceBlack_single11");
            spriteSymbols[14] = Content.Load<Texture2D>("sheepHex");
            spriteSymbols[15] = Content.Load<Texture2D>("waterHex");
            spriteSymbols[16] = Content.Load<Texture2D>("wheatHex");
            spriteSymbols[17] = Content.Load<Texture2D>("woodHex");

            // mocked up tile icons just for sandbox of grid
            for (int j=0; j< randomGrid.Next(32); j++)
            {
                icons.Add(new Vector2(randomGrid.Next(20), randomGrid.Next(20)));
            }

            Debug.WriteLine("Icons: " + icons.Count);
        }

        // A utility function to calculate area of triangle formed by (x1, y1) (x2, y2) and (x3, y3)
        static double area(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
        }

        static double area(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (area(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y));
        }

        // A function to check whether point P(x, y) lies inside the triangle formed by A(x1, y1), B(x2, y2) and C(x3, y3)
        static bool isInside(float x1, float y1, float x2, float y2, float x3, float y3, float x, float y)
        {
            /* Calculate area of triangle ABC */
            double A = area(x1, y1, x2, y2, x3, y3);

            /* Calculate area of triangle PBC */
            double A1 = area(x, y, x2, y2, x3, y3);

            /* Calculate area of triangle PAC */
            double A2 = area(x1, y1, x, y, x3, y3);

            /* Calculate area of triangle PAB */
            double A3 = area(x1, y1, x2, y2, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return (A == A1 + A2 + A3);
        }

        static bool isInside(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 click)
        {
            return (isInside(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, click.X, click.Y));
        }

        // Return true if p1 and p2 are on the same side of BA
        public static bool SameSide(Vector2 p1, Vector2 p2, Vector2 A, Vector2 B)
        {
            // Convert points to Vector3 for use the Cross product, which is Vector3-only
            Vector3 cp1 = Vector3.Cross(new Vector3(B - A, 0), new Vector3(p1 - A, 0));
            Vector3 cp2 = Vector3.Cross(new Vector3(B - A, 0), new Vector3(p2 - A, 0));
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        // Return true if the point p is in the triangle ABC
        public static bool PointInTriangle(Vector2 p, Vector2 A, Vector2 B, Vector2 C)
        {
            return SameSide(p, A, B, C) && SameSide(p, B, A, C) && SameSide(p, C, A, B);
        }

        private float TriangleWidth(float height)
        {
            return (float)(2 * height / Math.Sqrt(3));
        }

        // Return the row and column of the triangle at this point.
        private (float, float) PointToTriangle(float x, float y, float width, float height)
        {
            Debug.WriteLine("PointToTriangle. (x, y) (width, height): (" + x + "," + y + ") (" + width + "," + height + ")");

            float row = (y-tY) / height;
            float col = (x-tX) / width;

            Debug.WriteLine("PointToTriangle. (col, row): (" + col + "," + row + ")");

            /*
            float dy = (row + 1) * (height) - y - tY;
            float dx = x - col * width;
            if (row % 2 == 1) dy = height - dy;

            Debug.WriteLine("PointToTriangle. (dx, dy, ratio): (" + dx + "," + dy + "," + dx/dy + ")");

            if (dy > 1)
            {
                if (dx <= width/2)
                {
                    Debug.WriteLine("PointToTriangle. Left Triangle");

                    // Left half of triangle.
                    float ratio = dx / dy;
                    if (ratio < 1f / Math.Sqrt(3)) col -= 1.0f;
                }
                else
                {
                    Debug.WriteLine("PointToTriangle. Right Triangle");

                    // Right half of triangle.
                    float ratio = (width - dx) / dy;
                    if (ratio < 1f / Math.Sqrt(3)) col += 1.0f;
                }
            }
            */

            col = (float) Math.Round(col);
            row = (float) Math.Round(row);

            Debug.WriteLine("PointToTriangle. (col, row): (" + col + "," + row + ")");

            return (col, row);
        }

        private (float, float) PointToSquare(float x, float y, float width, float height)
        {
            float xSquare = (x + tX) / (width - 10);
            float ySquare = (y + tY) / (height - 10);

            return (xSquare, ySquare-1);
        }

        private float HexWidth(float height)
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        Color nextRainbow(Color c)
        {
            Color r = c;
            r.R += 4;
            r.G += 8;
            r.B += 2;

            return (r);
        }
        void drawShadowText(string text, int x, int y, Color c)
        {
            spriteBatch.DrawString(font1, text, new Vector2(x - 2, y - 2), Color.Black);
            spriteBatch.DrawString(font1, text, new Vector2(x, y), c);
        }

        void drawLine(Vector2 begin, Vector2 end, Color color, int width = 1, bool bloomEffect = false)
        {
            // TODO bloom effect

            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;

            spriteBatch.Draw(tLine, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        void drawClick(int x, int y)
        {
            drawLine(new Vector2(x-(gX/8), y-(gY/8)), new Vector2(x+(gX/8), y+(gY/8)), Color.LightGray, 1);
            drawLine(new Vector2(x-(gX/8), y+(gY/8)), new Vector2(x+(gX/8), y-(gY/8)), Color.DarkGray, 1);
        }

        void drawGhostShape(int x, int y, int gridStyle)
        {
            switch (gridstyle)
            {
                case (0):  // Hexagon
                    spriteBatch.Draw(hexagon1, new Rectangle(x, y, gX, gY), new Rectangle(0, 0, hexagon1.Width, hexagon1.Height), Color.BlueViolet, 0.0f, new Vector2(hexagon1.Width / 2, hexagon1.Height / 2), SpriteEffects.None, 1);
                    break;
                case (1):  // Square
                    spriteBatch.Draw(square1, new Rectangle(x, y, gX - 5, gY - 5), new Rectangle(0, 0, square1.Width, square1.Height), Color.Green, 0.0f, new Vector2(square1.Width / 2, square1.Height / 2), SpriteEffects.None, 1);
                    break;
                case (2):  // Triangle
                    spriteBatch.Draw(triangle1, new Rectangle(x, y, gX, gY), new Rectangle(0, 0, triangle1.Width, triangle1.Height), Color.PaleVioletRed, 0.0f, new Vector2(triangle1.Width / 2, triangle1.Height / 2), SpriteEffects.None, 1);
                    spriteBatch.Draw(triangle2, new Rectangle(x, y, gX, gY), new Rectangle(0, 0, triangle1.Width, triangle1.Height), Color.IndianRed, 0.0f, new Vector2(triangle1.Width / 2, triangle1.Height / 2), SpriteEffects.None, 1);
                    break;
                case (3):  // Bloom style
                    spriteBatch.Draw(hexagon2, new Rectangle(x, y, gX - 8, gY - 6), new Rectangle(0, 0, hexagon1.Width, hexagon1.Height), Color.LawnGreen, MathHelper.TwoPi / 4, new Vector2(hexagon1.Width / 2, hexagon1.Height / 2), SpriteEffects.None, 1);
                    break;
                case (4):  // Scrolling Screen
                    break;
                case (5):  // Blank Screen
                    break;
                default:
                    break;
            }
        }
        Vector2 findTile(float clickX, float clickY, int gridstyle)
        {
            Vector2 returnTile = new Vector2(-1, -1);

            switch (gridstyle)
            {
                case (0):  // Hexagon
                    (returnTile.X, returnTile.Y) = PointToHex(clickX, clickY, gX, gY-4);
                    break;
                case (1):  // Square
                    (returnTile.X, returnTile.Y) = PointToSquare(clickX, clickY, gX, gY);
                    break;
                case (2):  // Triangle
                    (returnTile.X, returnTile.Y) = PointToTriangle(clickX, clickY, gX/2, gY-4);
                    break;
                case (3):  // Bloom style
                    (returnTile.X, returnTile.Y) = PointToBloomHex(clickX, clickY, gX-4, gY-2);
                    break;
                case (4):  // Scrolling Screen
                    break;
                case (5):  // Blank Screen
                    break;
                default:
                    break;
            }

            return (returnTile);
        }

        bool findAdjacentTiles(Vector2 currentTile, bool clearTiles = true)
        {
            if (clearTiles) adjacentTiles.Clear();

            bool foundTiles = true;
            float x = currentTile.X;
            float y = currentTile.Y;

            switch (gridstyle)
            {
                case (0): // Hex (x6)
                    if (x % 2 == 0)
                    {
                        //Debug.WriteLine("Hex1 - X Even");
                        adjacentTiles.Add(new Vector2(x, y - 1));
                        adjacentTiles.Add(new Vector2(x - 1, y - 1));
                        adjacentTiles.Add(new Vector2(x + 1, y - 1));

                        adjacentTiles.Add(new Vector2(x - 1, y));
                        adjacentTiles.Add(new Vector2(x, y + 1));
                        adjacentTiles.Add(new Vector2(x + 1, y));
                    }
                    else
                    {
                        //Debug.WriteLine("Hex1 - X Odd");
                        adjacentTiles.Add(new Vector2(x, y - 1));
                        adjacentTiles.Add(new Vector2(x - 1, y));
                        adjacentTiles.Add(new Vector2(x + 1, y));

                        adjacentTiles.Add(new Vector2(x - 1, y + 1));
                        adjacentTiles.Add(new Vector2(x, y + 1));
                        adjacentTiles.Add(new Vector2(x + 1, y + 1));
                    }

                    foundTiles = true;
                    break;
                case (1): // Square (x8) 
                    adjacentTiles.Add(new Vector2(x-1, y-1));
                    adjacentTiles.Add(new Vector2(x, y-1));
                    adjacentTiles.Add(new Vector2(x+1, y-1));

                    adjacentTiles.Add(new Vector2(x-1, y));
                    adjacentTiles.Add(new Vector2(x+1, y));

                    adjacentTiles.Add(new Vector2(x-1, y+1));
                    adjacentTiles.Add(new Vector2(x, y+1));
                    adjacentTiles.Add(new Vector2(x+1, y+1));

                    foundTiles = true;
                    break;
                case (2): // Triangles (x3)
                    if (x % 2 == 0)
                    {
                        //Debug.WriteLine("Triangle - X Even");
                        adjacentTiles.Add(new Vector2(x-1, y));
                        adjacentTiles.Add(new Vector2(x+1, y));

                        if (y % 2 == 0)
                        {
                            //Debug.WriteLine("Triangle - Y Even");
                            adjacentTiles.Add(new Vector2(x, y + 1));
                        }
                        else
                        {
                            //Debug.WriteLine("Triangle - Y Odd");
                            adjacentTiles.Add(new Vector2(x, y - 1));
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("Triange - X Odd");
                        adjacentTiles.Add(new Vector2(x-1, y));
                        adjacentTiles.Add(new Vector2(x+1, y));

                        if (y % 2 == 0)
                        {
                            //Debug.WriteLine("Triangle - Y Even");
                            adjacentTiles.Add(new Vector2(x, y - 1));
                        }
                        else
                        {
                            //Debug.WriteLine("Triangle - Y Odd");
                            adjacentTiles.Add(new Vector2(x, y + 1));
                        }
                    }

                    foundTiles = true;
                    break;
                case (3): // Bloom Hex (x6)
                    if (y % 2 == 0)
                    {
                        //Debug.WriteLine("Hex2 - X Even");

                        adjacentTiles.Add(new Vector2(x - 1, y - 1));
                        adjacentTiles.Add(new Vector2(x, y - 1));

                        adjacentTiles.Add(new Vector2(x - 1, y));
                        adjacentTiles.Add(new Vector2(x + 1, y));

                        adjacentTiles.Add(new Vector2(x - 1, y + 1));
                        adjacentTiles.Add(new Vector2(x, y + 1));
                    }
                    else
                    {
                        //Debug.WriteLine("Hex2 - X Odd");

                        adjacentTiles.Add(new Vector2(x + 1, y - 1));
                        adjacentTiles.Add(new Vector2(x, y - 1));

                        adjacentTiles.Add(new Vector2(x - 1, y));
                        adjacentTiles.Add(new Vector2(x + 1, y));

                        adjacentTiles.Add(new Vector2(x + 1, y + 1));
                        adjacentTiles.Add(new Vector2(x, y + 1));
                    }

                    foundTiles = true;
                    break;
                case (4): // TODO
                    foundTiles = false;
                    break;
                case (5): // Blank
                    foundTiles = false;
                    break;
                default:
                    foundTiles = false;
                    break;
            }

            //Debug.WriteLine("findAdjacentTiles: " + adjacentTiles.Count);
            return (foundTiles);
        }

        void hideGridTiles(int wTile, int hTile, int gridStyle, bool clearFirst = false)
        {
            if (clearFirst) hideTiles.Clear();

            // seed grid with some random gaps
            for (int iGaps=0; iGaps<100; iGaps++)
            {
                Vector2 vGap = new Vector2(randomGrid.Next(wTile), randomGrid.Next(hTile));
                
                if (hideTiles.Contains(vGap) == false)
                    hideTiles.Add(vGap);
            }

            // prune any inaccessible tiles - dependent on style
            // loop through each tile and check to see if surrounded by hidden tiles - if so, remove too
            for (int countY = 0; countY < hTile; countY++)
            {
                for (int countX = 0; countX < wTile; countX++)
                {
                    int countHidden = 0;
                    bool findPartners = findAdjacentTiles(new Vector2(countX, countY), true);

                    foreach (Vector2 v in adjacentTiles)
                    {
                        if (hideTiles.Contains(v) == true)
                        {
                            countHidden++;
                        }

                        int pruneCheck = 0;
                        if (gridStyle != 2) pruneCheck = 2;

                        if (countHidden >= adjacentTiles.Count- pruneCheck)
                        {
                            if (hideTiles.Contains(v) == false)
                            {
                                hideTiles.Add(v);
                                //colouredTiles.Add(new colourTiles(countX, countY, Color.Red));
                                Debug.WriteLine("hideGridTiles - PRUNE Adjacent: (" + countX + "," + countY + ") - " + countHidden + " of " + adjacentTiles.Count);
                            }
                        }
                    }
                }
            }

            Debug.WriteLine("hideGridTiles:" + hideTiles.Count);
        }

        void randomlyChangeTiles(int chance, int maxX, int maxY)
        {
            colouredTiles.Clear();

            for (int iY=0; iY<maxY; iY++)
            {
                for (int iX = 0; iX < maxX; iX++)
                {
                    if (randomGrid.Next(0, 100) > chance)
                    {
                        Color randomColor = new Color(randomGrid.Next(255), randomGrid.Next(255), randomGrid.Next(255));
                        colouredTiles.Add(new colourTiles(iX, iY, randomColor));
                    }
                }
            }
        }

        void rainbowChangeTiles(int maxX, int maxY)
        {
            colouredTiles.Clear();
            Color c = Color.White;

            for (int iY = 0; iY < maxY; iY++)
            {
                c = nextRainbow(c);
                for (int iX = 0; iX < maxX; iX++)
                {
                    colouredTiles.Add(new colourTiles(iX, iY, c));
                }
            }
        }

        void displayAttractMode(int maxX, int maxY)
        {
            //colouredTiles.Clear();
 
            foreach (colourTiles c in colouredTiles)
            {
                c.C = nextRainbow(c.C);
            }
        }

        // Base on the grid type... find an adjacent tile and then choose to move the selection
        // keep moving around the grid and change direction when get to an edge or a dead end
        void wanderBot()
        {
            if (lastBotMove.AddMilliseconds(500) < DateTime.Now)
            {
                selectTile = selectTile + botDirection;

                if (selectTile.X > 40 || selectTile.X <= 0) botDirection.X = -botDirection.X;
                if (selectTile.Y > 40 || selectTile.Y <= 0) botDirection.Y = -botDirection.Y;

                if (hideTiles.Contains(selectTile))
                {
                    selectTile = selectTile - botDirection;
                    botDirection = -botDirection;
                }

                // crazy ivan
                if (randomGrid.Next(100) < 2)
                {
                    botDirection.Y = -1;
                }

                if (selectTiles.Contains(selectTile) == false) selectTiles.Add(selectTile);

                lastBotMove = DateTime.Now;
            }
        }

        // *** In real game ***
        // Normal tile movement would only allow moves to adjacent (as defined by grid style) tiles
        // City tiles increase overal growth by a % to all tiles and allow movement of 4? tiles radius from city
        // Food tiles increase overall growth by a % to all tiles - show Growth on screen and apply to tiles
        // Resource tiles increase attack/defense efficiency for all tiles - show efficiency on screen

        void displayTileIcons()
        {
            // mocked up tile icons just for sandbox of grid

            int iconIndex = 9;

            switch (gridstyle)
            {
                case (0): // Hex
                    foreach (Vector2 v in icons)
                    {
                        spriteBatch.Draw(spriteSymbols[iconIndex], new Rectangle((int)v.X*(int)(gX*0.667), (int)v.Y*(int)(gY-4-tY), 40, 40), new Rectangle(0, 0, spriteSymbols[iconIndex].Width, spriteSymbols[iconIndex].Height), Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 1);
                        iconIndex++;
                        if (iconIndex > 13) iconIndex = 9;
                    }
                    break;
                case (1): // Square
                    foreach (Vector2 v in icons)
                    {
                        spriteBatch.Draw(spriteSymbols[iconIndex], new Rectangle((int)v.X*(gX-10)-tX, (int)v.Y*(gY-10)-tY-2, 40, 40), new Rectangle(0, 0, spriteSymbols[iconIndex].Width, spriteSymbols[iconIndex].Height), Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 1);
                        iconIndex++;
                        if (iconIndex > 13) iconIndex = 9;
                    }
                    break;
                case (2): // Triangles
                    foreach (Vector2 v in icons)
                    {
                        spriteBatch.Draw(spriteSymbols[iconIndex], new Rectangle((int)v.X*(gX/2)-6-tX, (int)v.Y*(gY-8)-tY, 40, 40), new Rectangle(0, 0, spriteSymbols[iconIndex].Width, spriteSymbols[iconIndex].Height), Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 1);
                        iconIndex++;
                        if (iconIndex > 13) iconIndex = 9;
                    }
                    break;
                case (3): // Bloom Hex
                    foreach (Vector2 v in icons)
                    {
                        spriteBatch.Draw(spriteSymbols[iconIndex], new Rectangle((int)(v.X*gX-4)-tX, (int)v.Y*(int)((gY-4)*0.667f)-tY, 40, 40), new Rectangle(0, 0, spriteSymbols[iconIndex].Width, spriteSymbols[iconIndex].Height), Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 1);
                        iconIndex++;
                        if (iconIndex > 13) iconIndex = 9;
                    }
                    break;
                case (4): // TODO
                    foreach (Vector2 v in icons)
                    {
                        spriteBatch.Draw(spriteSymbols[iconIndex], new Rectangle((int)v.X*gX-5, (int)v.Y*gY, 40, 40), new Rectangle(0, 0, spriteSymbols[iconIndex].Width, spriteSymbols[iconIndex].Height), Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 1);
                        iconIndex++;
                        if (iconIndex > 13) iconIndex = 9;
                    }
                    break;
                case (5): // Blank
                    break;
                default:
                    break;
            }
        }

        void drawBloomGrid(int wTile, int hTile, bool checkered)
        {
            Color tileColor = Color.White;
            int x = 0, y = 0;

            for (int countY = 0; countY < (graphics.GraphicsDevice.Viewport.Height / (hTile*0.667f))-1; countY++)
            {
                for (int countX = 0; countX < graphics.GraphicsDevice.Viewport.Width / (wTile-4); countX++)
                {
                    if (countX % 2 == 0) tileColor = Color.LightGray; else tileColor = Color.FloralWhite;
                    if (checkered == false) tileColor = Color.White;

                    float r = rotateMe;

                    foreach (colourTiles t in colouredTiles)
                        if ((int)t.X == countX && (int)t.Y == countY)
                            tileColor = t.C;

                    if (selectTiles.Contains(new Vector2(countX, countY))) tileColor = Color.Yellow;
                    if (adjacentTiles.Contains(new Vector2(countX, countY))) tileColor = Color.MediumVioletRed;

                    if (countX == (int)selectTile.X && countY == (int)selectTile.Y)
                    {
                        r = wobbleAngle;
                        tileColor = selectedTile;
                    }

                    if ((seeThroughSelected == false || tileColor != Color.Yellow) && hideTiles.Contains(new Vector2(countX, countY)) == false)
                    {
                        float rotateOffset = r;
                        rotateOffset += MathHelper.TwoPi/4;   // flip hex model

                        (x, y) = figureHex(countX, countY, wTile, hTile, true);
                        spriteBatch.Draw(hexagon2, new Rectangle(x, y, wTile-8, hTile-6), new Rectangle(0, 0, hexagon1.Width, hexagon1.Height), tileColor, rotateOffset, new Vector2(hexagon1.Width / 2, hexagon1.Height / 2), SpriteEffects.None, 1);

                        if (showNumbers == true) drawShadowText(countX + "," + countY, x - 12, y - 12, Color.White);
                    }

                }
            }
        }

        void drawTriGrid(int wTile, int hTile, bool checkered)
        {
            Color tileColor = Color.White;
            bool flipflop = true;
            int countX = -1;
            int countY = -1;

            for (int y = tY; y < graphics.GraphicsDevice.Viewport.Height - tY; y += (hTile - 4))
            {
                countY++;
                countX = 0;

                for (int x = tX; x < graphics.GraphicsDevice.Viewport.Width - tX; x += ((wTile) / 2))
                {
                    countX++;

                    float r = rotateMe;

                    if (flipflop)
                    {
                        tileColor = Color.LightGray;

                        foreach (colourTiles t in colouredTiles)
                            if ((int)t.X == countX && (int)t.Y == countY)
                                tileColor = t.C;

                        if (checkered == false) tileColor = Color.White;
                        if (selectTiles.Contains(new Vector2(countX-1, countY))) tileColor = Color.Yellow;
                        if (adjacentTiles.Contains(new Vector2(countX-1, countY))) tileColor = Color.MediumVioletRed;

                        if (countX - 1 == (int)selectTile.X && countY == (int)selectTile.Y)
                        {
                            r = wobbleAngle;
                            tileColor = selectedTile;
                        }

                        if ((seeThroughSelected == false || tileColor != Color.Yellow) && hideTiles.Contains(new Vector2(countX, countY)) == false)
                            spriteBatch.Draw(triangle1, new Rectangle(x, y, wTile, hTile), new Rectangle(0, 0, triangle1.Width, triangle1.Height), tileColor, r, new Vector2(triangle1.Width/2, triangle1.Height/2), SpriteEffects.None, 1);

                        if (showNumbers == true)
                            drawShadowText((countX-1) + "," + countY, x-10, y-2, Color.FloralWhite);
                    }
                    else
                    {
                        tileColor = Color.FloralWhite;

                        foreach (colourTiles t in colouredTiles)
                            if ((int)t.X == countX && (int)t.Y == countY)
                                tileColor = t.C;

                        if (checkered == false) tileColor = Color.White;
                        if (selectTiles.Contains(new Vector2(countX-1, countY))) tileColor = Color.Yellow;
                        if (adjacentTiles.Contains(new Vector2(countX-1, countY))) tileColor = Color.MediumVioletRed;

                        if (countX - 1 == (int)selectTile.X && countY == (int)selectTile.Y)
                        {
                            r = wobbleAngle;
                            tileColor = selectedTile;
                        }

                        if ((seeThroughSelected == false || tileColor != Color.Yellow) && hideTiles.Contains(new Vector2(countX, countY)) == false)
                            spriteBatch.Draw(triangle2, new Rectangle(x, y, wTile, hTile), new Rectangle(0, 0, triangle2.Width, triangle2.Height), tileColor, r, new Vector2(triangle2.Width/2, triangle2.Height/2), SpriteEffects.None, 1);

                        if (showNumbers == true)
                            drawShadowText((countX-1) + "," + countY, x-10, y-18, Color.LightGray);
                    }

                    flipflop = !flipflop;                    
                }
            }
        }

        void drawSqrGrid(int wTile, int hTile, bool checkered)
        {
            Color tileColor = Color.White;
            bool flipflop = true;
            int countX = -1;
            int countY = -1;

            for (int y = tY; y < graphics.GraphicsDevice.Viewport.Height - tY; y += (hTile-10))
            {
                countY++;
                countX = 0;

                for (int x = tX; x < graphics.GraphicsDevice.Viewport.Width - tX; x += (wTile-10))
                {
                    countX++;

                    if (flipflop) tileColor = Color.LightGray; else tileColor = Color.FloralWhite;
                    flipflop = !flipflop;

                    if (checkered == false) tileColor = Color.White;

                    foreach (colourTiles t in colouredTiles)
                        if ((int)t.X == countX && (int)t.Y == countY)
                            tileColor = t.C;

                    float r = rotateMe;

                    if (selectTiles.Contains(new Vector2(countX, countY))) tileColor = Color.Yellow;
                    if (adjacentTiles.Contains(new Vector2(countX, countY))) tileColor = Color.MediumVioletRed;

                    if (countX == (int)selectTile.X && countY == (int)selectTile.Y)
                    {
                        tileColor = selectedTile;
                        r = wobbleAngle;
                    }

                    if ((seeThroughSelected == false || tileColor != Color.Yellow) && hideTiles.Contains(new Vector2(countX, countY)) == false)
                        spriteBatch.Draw(square1, new Rectangle(x, y, wTile-5, hTile-5), new Rectangle(0, 0, square1.Width, square1.Height), tileColor, r, new Vector2(square1.Width/2, square1.Height/2), SpriteEffects.None, 1);

                    if (showNumbers == true)
                        drawShadowText((countX-1) + "," + countY, x-10, y-10, Color.White);
                }
            }
        }

        private (float, float) PointToHex(float x, float y, float width, float height)
        {
            // http://csharphelper.com/blog/2015/10/draw-a-hexagonal-grid-in-c/
            // http://barankahyaoglu.com/dev/coordinate-calculations-in-hexagonal-world/

            // Find the test rectangle containing the point.
            float col = (int)(x / (width * 0.667f));
            float row = (col % 2 == 0) ? (int)(y / height) : (int)((y - height / 2) / height);

            // Find the test area.
            float testx = col * width * 0.667f;
            float testy = row * height;
            if (col % 2 == 1) testy += height / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (col % 2 == 0) row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 == 1) row++;
                col--;
            }

            return (col,row);
        }

        private (float, float) PointToBloomHex(float x, float y, float width, float height)
        {
            // http://csharphelper.com/blog/2015/10/draw-a-hexagonal-grid-in-c/
            // http://barankahyaoglu.com/dev/coordinate-calculations-in-hexagonal-world/

            // Find the test rectangle containing the point.
            float row = (int)(y / (height * 0.667f));
            float col = (row % 2 == 0) ? (int)(x / width) : (int)((x - width / 2) / width);

            // Find the test area.
            float testy = row * height * 0.667f;
            float testx = row * height;
            if (row % 2 == 1) testx += width / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dy = y - testy;
            if (dy < height / 4)
            {
                float dx = x - (testx + width / 2);
                if (dy < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dx < 0) is_above = true;
                    if (dx > 0) is_below = true;
                }
                else if (dx < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dx / dy > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dx / dy > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (row % 2 == 0) col--;
                row--;
            }
            else if (is_below)
            {
                if (row % 2 == 1) col++;
                row--;
            }

            return (col, row);
        }

        private (int, int) figureHex(int pX, int pY, int wTile, int hTile, bool hStyle = false)
        {
            // Start with the leftmost corner of the upper left hexagon.
            float x = tX;
            float y = tY;
            
            if (hStyle == false)
            {   // Calc to support the Vertically aligning style of hex

                // Move down the required number of rows.
                y += pY * (hTile - 4);

                // If the column is odd, move down half a hex more.
                if (pX % 2 == 1) y += (hTile - 4) / 2;

                // Move over for the column number.
                x += (float)(wTile * 0.667) * pX;
            }
            else
            {   // Calc to support the Horizontal aligning style of hex

                // Move down the required number of rows.
                y += pY * (hTile - 18);

                // If the row is odd, move along half a hex more.
                if (pY % 2 == 1) x += (hTile - 4) / 2;

                // Move over for the column number.
                x += (float)(wTile-4) * pX;
            }

            return ((int)x, (int)y);
        }

        void drawHexGrid(int wTile, int hTile, bool checkered)
        {
            Color tileColor = Color.White;
            int x = 0, y = 0; 

            for (int countY = 0; countY < graphics.GraphicsDevice.Viewport.Height/(hTile-4); countY++)
            {
                for (int countX = 0; countX < graphics.GraphicsDevice.Viewport.Width/(wTile*0.667)-1; countX++)
                {
                    if (countX % 2 == 0) tileColor = Color.LightGray; else tileColor = Color.FloralWhite;
                    if (checkered == false) tileColor = Color.White;

                    foreach (colourTiles t in colouredTiles)
                        if ((int)t.X == countX && (int)t.Y == countY)
                            tileColor = t.C;

                    if (selectTiles.Contains(new Vector2(countX, countY))) tileColor = Color.Yellow;
                    if (adjacentTiles.Contains(new Vector2(countX, countY))) tileColor = Color.MediumVioletRed;

                    float r = rotateMe;

                    if (countX == (int)selectTile.X && countY == (int)selectTile.Y)
                    {
                        tileColor = selectedTile;
                        r = wobbleAngle;
                    }

                    if ((seeThroughSelected == false || tileColor != Color.Yellow) && hideTiles.Contains(new Vector2(countX, countY)) == false)
                    {
                        (x, y) = figureHex(countX, countY, wTile, hTile);
                        spriteBatch.Draw(hexagon1, new Rectangle(x, y, wTile, hTile), new Rectangle(0, 0, hexagon1.Width, hexagon1.Height), tileColor, r, new Vector2(hexagon1.Width / 2, hexagon1.Height / 2), SpriteEffects.None, 1);

                        if (showNumbers == true) drawShadowText(countX + "," + countY, x-12, y-12, Color.White);
                    }

                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState newState = Mouse.GetState();

            if ((newState.RightButton == ButtonState.Pressed && oldState.RightButton == ButtonState.Released) || Keyboard.GetState().IsKeyDown(Keys.Tab) == true)
            {
                if (whenClicked.AddMilliseconds(100) < DateTime.Now)
                {
                    gridstyle++;
                    if (gridstyle > 5) gridstyle = 0;
                    adjacentTiles.Clear();
                    //selectTiles.Clear();

                    whenClicked = DateTime.Now;
                }
            }

            if ((newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released))
            {
                selectClick.X = newState.X;
                selectClick.Y = newState.Y;

                // find nearest tile to the mouse click
                selectTile = findTile(selectClick.X, selectClick.Y, gridstyle);

                if (selectTile.X >= 0 && selectTile.Y >= 0)
                {
                    selectTile.X = (int)selectTile.X;
                    selectTile.Y = (int)selectTile.Y;

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == false)
                    {
                        if (selectTiles.Contains(selectTile)) selectTiles.Remove(selectTile);
                        else selectTiles.Add(selectTile);
                    }
                    else
                    {
                        if (hideTiles.Contains(selectTile)) hideTiles.Remove(selectTile);
                        else hideTiles.Add(selectTile);
                    }

                    Debug.WriteLine("findTile: (" + selectTile.X + "," + selectTile.Y + ")");

                    //Debug.WriteLine("XY: (" + newState.X + "," + newState.Y + ")");
                    //Debug.WriteLine("PositionXY: (" + Mouse.GetState().Position.X + "," + Mouse.GetState().Position.Y + ")");

                    wobbleNow = 0.05f;
                }
            }

            if (whenClicked.AddMilliseconds(500) < DateTime.Now)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.C) == true)
                {
                    checkeredOn = !checkeredOn;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
                {
                    selectTile.X++;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
                {
                    selectTile.X--;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Down) == true)
                {
                    selectTile.Y++;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up) == true)
                {
                    selectTile.Y--;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
                {
                    if (selectTile.X >= 0 && selectTile.Y >= 0)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == false)
                        {
                            if (selectTiles.Contains(selectTile)) selectTiles.Remove(selectTile);
                            else selectTiles.Add(selectTile);
                        }
                        else
                        {
                            if (hideTiles.Contains(selectTile)) hideTiles.Remove(selectTile);
                            else hideTiles.Add(selectTile);
                        }

                        Debug.WriteLine("findTile: (" + selectTile.X + "," + selectTile.Y + ")");

                        wobbleNow = 0.05f;
                    }
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.F4) == true)
                {
                    graphics.IsFullScreen = true;
                    graphics.ApplyChanges();
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R) == true)
                {
                    showRaster = !showRaster;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
                {
                    startRotateRight = true;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Back) == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == true)
                    {
                        seeThroughSelected = false;
                        selectTiles.Clear();
                    }
                    else
                    {
                        seeThroughSelected = !seeThroughSelected;
                    }

                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.N) == true)
                {
                    showNumbers = !showNumbers;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.G) == true)
                {
                    ghostClick = !ghostClick;
                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.H) == true)
                {
                    // randomly hide parts of grid, just assume 55x55

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == true)
                        hideGridTiles(55, 55, gridstyle, true);
                    else
                        hideGridTiles(55, 55, gridstyle, false);

                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.T) == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == true)
                    {
                        colouredTiles.Clear();
                    }
                    else
                    {
                        // 50% change of change and just set assume 55x55 grid
                        randomlyChangeTiles(50, 55, 55);
                    }

                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
                {
                    if (attractOn == false)
                    {
                        colouredTiles.Clear();
                        rainbowChangeTiles(55, 55);
                        attractOn = true;
                    }
                    else
                    {
                        colouredTiles.Clear();
                        attractOn = false;
                    }

                    //displayAttractMode(55, 55);

                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S) == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) == true)
                    {
                        adjacentTiles.Clear();
                    }
                    else
                    {
                        bool ok = findAdjacentTiles(selectTile);
                    }

                    whenClicked = DateTime.Now;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.B) == true)
                {
                    selectionBot = !selectionBot;

                    botDirection.X = 1;
                    botDirection.Y = 0;

                    whenClicked = DateTime.Now;
                }
            }

            // raster effect
            screenLines = nextRainbow(screenLines);
            lineColor = screenLines;
            selectedTile = nextRainbow(selectedTile);

            for (int j = tRaster.Height - 1; j >= 0; j--)
            {
                lineColor = nextRainbow(lineColor);

                for (int k = 0; k < tRaster.Width; k++)
                {
                    rasterColors[(j * tRaster.Width) + k] = lineColor;
                }
            }

            oldState = newState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (showRaster == true)
            {
                tRaster.SetData<Color>(rasterColors);
                spriteBatch.Draw(tRaster, new Vector2(0, 0), Color.White);
            }

            if (attractOn == true)
            {
                displayAttractMode(55, 55);
            }

            if (selectionBot == true)
            {
                wanderBot();
            }

            if (startRotateRight == true)
            {
                rotateMe += 0.05f;
                if (rotateMe >= MathHelper.TwoPi)
                {
                    rotateMe = 0f;
                    startRotateRight = false;
                    startRotateLeft = true;
                }
            }

            if (startRotateLeft == true)
            {
                rotateMe -= 0.05f;
                if (rotateMe <= -MathHelper.TwoPi)
                {
                    rotateMe = 0f;
                    startRotateRight = false;
                    startRotateLeft = false;
                }
            }

            if (wobbleNow != 0.0f)
            {
                wobbleAngle += wobbleNow;

                if (wobbleAngle > MathHelper.PiOver4/4)
                { 
                    wobbleNow = -0.05f;
                }

                if (wobbleAngle < -MathHelper.PiOver4/4)
                {
                    wobbleAngle = 0.0f;
                    wobbleNow = -0.0f;
                }
            }

            switch (gridstyle)
            {
                case (0):  // Hexagon
                    drawHexGrid(gX, gY, checkeredOn);
                    drawShadowText("Hexagon", graphics.PreferredBackBufferWidth-100, 0, Color.Red);
                    break;
                case (1):  // Square
                    drawSqrGrid(gX, gY, checkeredOn);
                    drawShadowText("Square", graphics.PreferredBackBufferWidth - 100, 0, Color.Red);
                    break;
                case (2):  // Triangle
                    drawTriGrid(gX, gY, checkeredOn);
                    drawShadowText("Triangle", graphics.PreferredBackBufferWidth - 100, 0, Color.Red);
                    break;
                case (3):  // Bloom style
                    drawBloomGrid(gX, gY, checkeredOn);
                    drawShadowText("Bloom Effect", graphics.PreferredBackBufferWidth - 200, 0, Color.Red);
                    break;
                case (4):  // Bloom style
                    //drawBloomGrid(gX, gY, checkeredOn);
                    drawShadowText("TODO - SCROLLING", graphics.PreferredBackBufferWidth - 200, 0, Color.Red);
                    break;
                case (5):  // Blank
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X, (int)selectClick.Y, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Red, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+10, (int)selectClick.Y+10, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Orange, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+20, (int)selectClick.Y+20, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Yellow, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+30, (int)selectClick.Y+30, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Green, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+40, (int)selectClick.Y+40, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Blue, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+50, (int)selectClick.Y+50, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Indigo, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    spriteBatch.Draw(square1, new Rectangle((int)selectClick.X+60, (int)selectClick.Y+60, 25, 25), new Rectangle(0, 0, square1.Width, square1.Height), Color.Violet, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);

                    drawShadowText("Right Click or <TAB> to change grids", 100, 10, Color.ForestGreen);
                    drawShadowText("Left Click or Arrows/<ENTER> to select tile, with <SHIFT> to hide", 100, 50, Color.White);
                    drawShadowText("<SPACE> to rotate all tiles", 100, 80, Color.White);
                    drawShadowText("<BACKSPACE> selected tiles clear, with <SHIFT> to reset", 100, 110, Color.White);
                    drawShadowText("<C> to turn on/off checkerboard", 100, 140, Color.White);
                    drawShadowText("<R> to turn on/off Raster effect", 100, 170, Color.White);
                    drawShadowText("<N> to turn on/off tile numbering", 100, 200, Color.White);
                    drawShadowText("<F4> to hopefully turn on full screen", 100, 230, Color.White);
                    drawShadowText("<G> to toggle click position", 100, 260, Color.White);
                    drawShadowText("<H> to randomly hide parts of the grid. <SHIFT+H> to reset and hide parts", 100, 290, Color.White);
                    drawShadowText("<T> to randomly draw grid colors (<SHIFT>-T to clear)", 100, 320, Color.White);
                    drawShadowText("<A> to toggle attract mode", 100, 350, Color.White);
                    drawShadowText("<S> to show tiles connected to currently selected tile (<SHIFT>-S to clear)", 100, 380, Color.White);
                    drawShadowText("<B> to turn on/off selection Bot", 100, 410, Color.White);

                    drawShadowText("<ESC> to Quit", 100, 440, Color.Gray);

                    drawShadowText("GRID SANDBOX", graphics.PreferredBackBufferWidth - 150, 0, Color.IndianRed);

                    for (int si=2; si<18; si++)
                    {
                        spriteBatch.Draw(spriteSymbols[si], new Rectangle(20+(si*60), 500, 50, 50), new Rectangle(0, 0, spriteSymbols[si].Width, spriteSymbols[si].Height), Color.White, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                        drawShadowText(si.ToString(), 40+(si*60), 550, Color.Gray);
                    }
                    spriteBatch.Draw(spriteSymbols[0], new Rectangle(20, 600, 300, 75), new Rectangle(0, 0, spriteSymbols[0].Width, spriteSymbols[0].Height), Color.White, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    drawShadowText("0", 20, 680, Color.Gray);
                    spriteBatch.Draw(spriteSymbols[1], new Rectangle(400, 600, 300, 75), new Rectangle(0, 0, spriteSymbols[0].Width, spriteSymbols[0].Height), Color.White, rotateMe, new Vector2(0, 0), SpriteEffects.None, 1);
                    drawShadowText("1", 400, 680, Color.Gray);
                    break;
                default:
                    break;
            }

            drawShadowText(selectClick.X + "," + selectClick.Y, (int)selectClick.X, (int)selectClick.Y, Color.Gray);
            drawShadowText((int)selectTile.X + "," + (int)selectTile.Y, (int)selectClick.X, (int)selectClick.Y+20, Color.ForestGreen);

            if (ghostClick == false) drawClick((int)selectClick.X, (int)selectClick.Y);
            else drawGhostShape((int)selectClick.X, (int)selectClick.Y, gridstyle);

            // display mocked icons
            displayTileIcons();

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
