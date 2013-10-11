using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FaceTracking
{
    public class InteractionPoints
    {

        public Rectangle head;

        public Rectangle headUp;
        public Rectangle headDown;
        public Rectangle headLeft;
        public Rectangle headRight;

        public Rectangle neutral;
        public Rectangle shoulderRight;
        public Rectangle shoulderLeft;

        public Rectangle left;
        public Rectangle right;

        public InteractionPoints()
        {

        }

        public Rectangle[] GetRects()
        {
            return new Rectangle[] 
            {
                head, headUp, headDown, headLeft, headRight,
                shoulderLeft, shoulderRight,
                left, neutral, right
            };
        }

        public void setHead(Rectangle head)
        {
            this.head = head;

            headLeft = new Rectangle(head.X - head.Height, head.Y, head.Height, head.Height);
            headRight = new Rectangle(head.X + head.Width, head.Y, head.Height, head.Height);
            headUp = new Rectangle(head.X - head.Width / 2, head.Y - head.Height / 2, head.Width * 2, head.Height);
            headDown = new Rectangle(head.X - head.Width / 2, head.Y + head.Height / 2, head.Width * 2, head.Width);

            shoulderLeft = new Rectangle(head.X - head.Width, headLeft.Y + headLeft.Height, head.Width, head.Width);
            shoulderRight = new Rectangle(headRight.X, headLeft.Y + headRight.Height, head.Width, head.Width);

            left = new Rectangle(headDown.X - head.Height, headDown.Y + headDown.Height / 2, head.Height, 2 * head.Height);
            right = new Rectangle(headDown.X + headDown.Width, headDown.Y + headDown.Height / 2, head.Height, 2 * head.Height);

            neutral = new Rectangle(headDown.X, headDown.Y, headDown.Width, 2 * head.Height);
        }

        public int classify(Point p)
        {
            if (shoulderLeft.Contains(p))
                return 4;
            if (shoulderRight.Contains(p))
                return 5;

            if (left.Contains(p))
                return 2;
            if (right.Contains(p))
                return 3;

            if (neutral.Contains(p))
                return 1;

            if (headLeft.Contains(p))
                return 6;
            if (headRight.Contains(p))
                return 7;
            if (headUp.Contains(p))
                return 8;
            if (headDown.Contains(p))
                return 9;

            return 0;
        }
    }
}
