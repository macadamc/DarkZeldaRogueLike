using UnityEngine;

namespace CoreGame.Helper
{
    public class QtreeObj : QuadTree.IHasRectangle
    {
        public int ID;
        public string Tag;
        internal Rect _rect;
        public Rect Rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
            }
        }

        public QtreeObj()
        {
            _rect = new Rect();
        }
        public QtreeObj(float x, float y, float width, float height)
        {
            _rect = new Rect(x, y, width, height);
        }
        public QtreeObj(Rect rect)
        {
            _rect = rect;
        }
    }
}
