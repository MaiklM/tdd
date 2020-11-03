﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        internal readonly HashSet<Rectangle> rectangles = new HashSet<Rectangle>();
        internal readonly Point center;
        private readonly PointGetter getPointer;
        internal CircularCloudLayouter(Point center)
        {
            this.center = center;
            getPointer = new PointGetter(center);
        }

        internal Rectangle PutNextRectangle(Size rectangleSize)
        {
            var result = MoveToCentre(PutRectangleOnCircle(rectangleSize));
            rectangles.Add(result);
            return result;
        }

        private Rectangle PutRectangleOnCircle(Size size)
        {
            var rectangle = new Rectangle();
            do
            {
                var point = getPointer.GetNextPoint();
                rectangle = GetRectangleFromSizeAndCenter(size, point);
            } while (HasIntersection(rectangle));               
            return rectangle;
        }

        private Rectangle GetRectangleFromSizeAndCenter(Size size, Point rectangleCenter)
        {
            var location = new Point(rectangleCenter.X - (int)(size.Width / 2), rectangleCenter.Y - (int)(size.Height / 2));
            return new Rectangle(location, size);
        }

        private Rectangle MoveToCentre(Rectangle rectangle)
        {
            var previousRectangle = new Rectangle();
            while(true)
            {
                var vectorToMove = GetVectorToMove(rectangle);
                rectangle = MoveToPixel(rectangle, vectorToMove);
                if(rectangle == previousRectangle)
                    break;
                previousRectangle = rectangle;
            }
            return rectangle;
        }

        private Rectangle MoveToPixel(Rectangle rectangle, Point vectorToMove)
        {
            if(Math.Abs(vectorToMove.X) > Math.Abs(vectorToMove.Y))
            {
                var newRectangleX = new Rectangle(rectangle.Location, rectangle.Size);
                newRectangleX.X += vectorToMove.X > 0 ? 1 : -1;
                if(!HasIntersection(newRectangleX))
                    return newRectangleX;
            }
            var newRectangle = new Rectangle(rectangle.Location, rectangle.Size);
            newRectangle.Y +=  vectorToMove.Y > 0 ? 1 : vectorToMove.Y < 0 ? -1 : 0;
            if(!HasIntersection(newRectangle))
                return newRectangle;
            return rectangle;
        }

        private bool HasIntersection(Rectangle rectangle) => rectangles.Any(r => r.IntersectsWith(rectangle));

        private Point GetVectorToMove(Rectangle rectangle)
        {
            var centreRectangle = GetCentreRectangle(rectangle);
            return new Point(center.X - centreRectangle.X, center.Y - centreRectangle.Y);
        }

        private Point GetCentreRectangle(Rectangle rectangle)
        {
            var yCentreRectangle = (int)((rectangle.Top + rectangle.Bottom) / 2);
            var xCentreRectangle = (int)((rectangle.Left + rectangle.Right) / 2);
            var centreRectangle = new Point(xCentreRectangle, yCentreRectangle);
            return centreRectangle;
        }
    }
}
