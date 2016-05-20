using System;
using System.Collections.Generic;
using System.Text;

namespace BitmapCutter.Core.API
{
    /// <summary>
    /// Cutter Model
    /// </summary>
    public class Cutter
    {
        #region -Constructor-
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter()
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
        { }
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter(double _Zoom, int _X, int _Y, int _CutterWidth, int _CutterHeight, int _Width, int _Height)
            : this(_Zoom, _X, _Y, _CutterWidth, _CutterHeight, _Width, _Height, (int)(_Zoom * _Width), (int)(_Zoom * _Height))
        { }
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter(double _Zoom,int _X,int _Y,int _CutterWidth,int _CutterHeight,int _Width,int _Height,int _SaveWidth,int _SaveHeight) {
            this._Zoom = _Zoom;
            this._X = _X;
            this._Y = _Y;
            this._CutterHeight = _CutterHeight;
            this._CutterWidth = _CutterWidth;
            this._Width = _Width;
            this._Height = _Height;
            this._SaveWidth = _SaveWidth;
            this._SaveHeight = _SaveHeight;
        }
        #endregion

        #region -Properties-
        private int _SaveWidth;
        /// <summary>
        /// Resize bitmap(Width)
        /// </summary>
        public int SaveWidth
        {
            get { return _SaveWidth; }
            set { _SaveWidth = value; }
        }
        private int _SaveHeight;
        /// <summary>
        /// Resize bitmap(Height)
        /// </summary>
        public int SaveHeight
        {
            get { return _SaveHeight; }
            set { _SaveHeight = value; }
        }
        private double _Zoom;
        /// <summary>
        /// Zoom rate
        /// </summary>
        public double Zoom
        {
            get { return _Zoom; }
            set { _Zoom = value; }
        }
        private int _X;
        /// <summary>
        /// X coordinates(from left-top corner)
        /// </summary>
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }
        private int _Y;
        /// <summary>
        /// Y coordinates(from left-top corner)
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        private int _CutterWidth;
        /// <summary>
        /// Width of cutter
        /// </summary>
        public int CutterWidth
        {
            get { return _CutterWidth; }
            set { _CutterWidth = value; }
        }
        private int _CutterHeight;
        /// <summary>
        /// Height of cutter
        /// </summary>
        public int CutterHeight
        {
            get { return _CutterHeight; }
            set { _CutterHeight = value; }
        }
        private int _Width;
        /// <summary>
        /// Width of original size
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        private int _Height;
        /// <summary>
        /// Height of original size
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        #endregion
    }
}
