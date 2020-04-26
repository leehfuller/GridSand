using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GridSand
{
    class DrawGrid
    {
        public float HexHeight = 50;
        int a = 0, b = 0, c = 0;

        private void DrawHexGrid(float xmin, float xmax, float ymin, float ymax, float height)
        {

        }

        private void DrawTriGrid()
        {

        }

        private void DrawSqrGrid()
        {

        }


        void GetHex(float x, float y, out int row, out int column)
        {
            // Find out which major row and column we are on:
            row = (int)(y / b);
            column = (int)(x / (a + c));

            // Compute the offset into these row and column:
            float dy = y - (float)row * b;
            float dx = x - (float)column * (a + c);

            // Are we on the left of the hexagon edge, or on the right?
            if (((row ^ column) & 1) == 0)
                dy = b - dy;
            int right = dy * (a - c) < b * (dx - c) ? 1 : 0;

            // Now we have all the information we need, just fine-tune row and column.
            row += (column ^ row ^ right) & 1;
            column += right;
        }

    }
}
