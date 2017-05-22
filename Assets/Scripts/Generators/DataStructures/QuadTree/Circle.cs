using System.Collections.Generic;
using UnityEngine;


namespace CoreGame.Helper
{
    //public class Circle : QuadTree.IHasRectangle
    //{
    //    public int ID;
    //    public float radius;
    //    public Vector2 centerPos;
    //    public List<QtreeObj> rooms;
    //    public UnityEngine.Rect Rect
    //    {
    //        get
    //        {
    //            return new Rect(centerPos.x - radius, centerPos.y - radius, radius * 2, radius * 2);
    //        }
    //    }

    //    public Circle(Vector2 CenterPos, float Radius)
    //    {
    //        centerPos = CenterPos;
    //        radius = Radius;
    //        rooms = new List<QtreeObj>();
    //    }

    //    public static bool Intersect(Circle c1, Circle c2)
    //    {

    //        //ABS(R0 - R1) <= SQRT((x0 - x1) ^ 2 + (y0 - y1) ^ 2) <= (R0 + R1)
    //        float dist = Mathf.Sqrt(Mathf.Pow((c1.centerPos.x - c2.centerPos.x), 2) + Mathf.Pow((c1.centerPos.y - c2.centerPos.y), 2));
    //        if (dist > (c1.radius + c2.radius))
    //        {
    //            // No overlap
    //            return false;
    //        }
    //        else if (dist <= Mathf.Abs(c1.radius - c2.radius))
    //        {
    //            //inside circle
    //            return true;
    //        }
    //        else              // if (distance <= r1 + r2)
    //        {
    //            //overlaps circle
    //            return true;
    //        }
    //    }

    //    public static bool pointInCircle(Circle circle, float x, float y)
    //    {
    //        return Mathf.Pow(x - circle.centerPos.x, 2) + Mathf.Pow(y - circle.centerPos.y, 2) < Mathf.Pow(circle.radius, 2);
    //    }
    //}


    public class Circle : QtreeObj
    {
        public float radius
        {
            get
            {
                return _rect.width / 2f;
            }
            set
            {
                _rect.width = value * 2f;
                _rect.height = value * 2f;
            }
        }
        public Vector2 centerPos
        {
            get
            {
                return new Vector2(_rect.x + radius, _rect.y + radius);
            }
            set
            {
                _rect.x = value.x - radius;
                _rect.y = value.y - radius;
            }
        }
        public List<QtreeObj> rooms;

        public Circle(Vector2 CenterPos, float Radius)
        {
            centerPos = CenterPos;
            radius = Radius;
            rooms = new List<QtreeObj>();
        }

        public static bool Intersect(Circle c1, Circle c2)
        {
            Vector2 pos1 = c1.centerPos;
            Vector2 pos2 = c2.centerPos;
            float r1 = c1.radius;
            float r2 = c2.radius;

            //ABS(R0 - R1) <= SQRT((x0 - x1) ^ 2 + (y0 - y1) ^ 2) <= (R0 + R1)
            float dist = Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2));
            if (dist > (r1 + r2))
            {
                // No overlap
                return false;
            }
            else if (dist <= Mathf.Abs(r1 - r2))
            {
                //inside circle
                return true;
            }
            else              // if (distance <= r1 + r2)
            {
                //overlaps circle
                return true;
            }
        }

        public static bool pointInCircle(Circle circle, float x, float y)
        {
            return Mathf.Pow(x - circle.centerPos.x, 2) + Mathf.Pow(y - circle.centerPos.y, 2) < Mathf.Pow(circle.radius, 2);
        }
    }
}
