using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace Merit_Money
{
    public class CircularImageView : ImageView
    {
        public static float radius = 200.0f;

        public CircularImageView(Context context)
            : base(context)
        {

        }

        public CircularImageView(Context context, IAttributeSet attrs):
            base(context, attrs)
        {

        }

        public CircularImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)

        {

        }
        protected override void OnDraw(Canvas canvas)
        {
            Path clipPath = new Path();
            RectF rect = new RectF(5, 5, this.Width-5, this.Height-5);
            clipPath.AddRoundRect(rect, radius, radius, Path.Direction.Cw);
            canvas.ClipPath(clipPath);
            base.OnDraw(canvas);
        }

    }
}