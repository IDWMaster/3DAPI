using System;
using System.Collections.Generic;
using System.Text;

namespace _3DAPI
{
    public abstract class Keyboard
    {
    
        public event keyboardeventargs onKeyDown;
        public event keyboardeventargs onKeyUp;
        protected void ntfyKeyDown(string keyname)
        {
            if (onKeyDown != null)
            {
                onKeyDown.Invoke(keyname);
            }

        }
        protected void ntfyKeyUp(string keyname)
        {
            if (onKeyUp != null)
            {
                onKeyUp.Invoke(keyname);
            }

        }
    }
    public abstract class Touchpad
    {
        
        public event TouchEvent onTouchFound;
        public event TouchEvent onTouchLost;
        public event TouchEvent onTouchMoved;
        protected void ntfyTouchFound(int touchpoint, int x, int y)
        {
            if (onTouchFound != null)
            {
                onTouchFound.Invoke(touchpoint, x, y);
            }
        }
        protected void ntfyTouchLost(int touchpoint, int x, int y)
        {
            if (onTouchLost != null)
            {
                onTouchLost.Invoke(touchpoint, x, y);
            }
        }
        protected void ntfyTouchMove(int touchpoint, int x, int y)
        {
            if (onTouchMoved != null)
            {
                onTouchMoved.Invoke(touchpoint, x, y);
            }
        }
    }
    public delegate void TouchEvent(int touchpoint, int x, int y);
    public abstract class Mouse
    {
    
        public event mouseEvent onMouseDown;
        public event mouseEvent onMouseMove;
        public event mouseEvent onMouseUp;
        protected void ntfyMouseDown(MouseButton btn, int x, int y)
        {
            if (onMouseDown != null)
            {
                onMouseDown.Invoke(btn, x, y);
            }
        }
        protected void ntfyMouseUp(MouseButton btn, int x, int y)
        {
            if (onMouseUp != null)
            {
                onMouseUp.Invoke(btn, x, y);
            }
        }
        protected void ntfyMouseMove(MouseButton btn, int x, int y)
        {
            if (onMouseMove != null)
            {
                onMouseMove.Invoke(btn, x, y);
            }
        }

    }
    
    public enum MouseButton
    {
    Left,Middle,Right,One,Two,Three,Four,Five,Six,None
    }
    public delegate void mouseEvent(MouseButton btn, int x, int y); 
    public delegate void keyboardeventargs(string KeyName);
}
