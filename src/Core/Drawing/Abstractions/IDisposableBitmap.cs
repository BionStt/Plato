﻿using System;
using System.Drawing;

namespace PlatoCore.Drawing.Abstractions
{
    public interface IDisposableBitmap : IDisposable
    {

        IDisposableBitmap Configure(Action<BitmapOptions> action);

        Bitmap Render(Action<Bitmap> renderer);

    }

}
