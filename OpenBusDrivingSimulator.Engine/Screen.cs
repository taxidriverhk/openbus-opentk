using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using SDL2;
using OpenBusDrivingSimulator.Engine.Controls;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Screen
    {
        private class IconData
        {
            public Bitmap IconBmp;
            public BitmapData IconBmpData;
            public IntPtr IconSurface;
        }

        private static bool closed;
        private static bool initialized;

        private static int width;
        private static int height;

        private static IntPtr windowHandle;
        private static IntPtr glContext;
        private static GraphicsContext graphicsContext;
        private static IconData iconData;

        private static Ray mouseRay;

        public static bool Closed
        {
            get { return closed; }
        }

        public static bool Initialized
        {
            get { return initialized; }
        }

        public static int Width
        {
            get { return width; }
        }

        public static int Height
        {
            get { return height; }
        }

        public static Ray MouseRay
        {
            get { return mouseRay; }
        }

        public static bool Initialize(int inputWidth, int inputHeight, string title, string iconPath)
        {
            if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_VIDEO) != 0)
                return false;

            // Initialize the window
            SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            windowHandle = SDL.SDL_CreateWindow(title,
                SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                inputWidth, inputHeight, flags);

            // Initialize the OpenGL context
            glContext = SDL.SDL_GL_CreateContext(windowHandle);
            graphicsContext = new GraphicsContext(new ContextHandle(glContext),
                (string proc) => SDL.SDL_GL_GetProcAddress(proc),
                () => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));

            // Add icon to the window
            if (!string.IsNullOrEmpty(iconPath))
            {
                iconData = new IconData();
                iconData.IconBmp = new Bitmap(iconPath);
                iconData.IconBmpData = iconData.IconBmp.LockBits(new Rectangle(0, 0, iconData.IconBmp.Width, iconData.IconBmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                iconData.IconSurface = SDL.SDL_CreateRGBSurfaceFrom(iconData.IconBmpData.Scan0, iconData.IconBmp.Width, iconData.IconBmp.Height,
                    32, iconData.IconBmpData.Stride, 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000);
                SDL.SDL_SetWindowIcon(windowHandle, iconData.IconSurface);
            }

            width = inputWidth;
            height = inputHeight;

            closed = false;
            initialized = true;
            return true;
        }

        public static void Destroy()
        {
            if (initialized)
            {
                graphicsContext.Dispose();
                SDL.SDL_GL_DeleteContext(glContext);
                SDL.SDL_DestroyWindow(windowHandle);
                SDL.SDL_QuitSubSystem(SDL.SDL_INIT_VIDEO);
                if (iconData != null)
                {
                    SDL.SDL_FreeSurface(iconData.IconSurface);
                    iconData.IconBmp.UnlockBits(iconData.IconBmpData);
                }
                initialized = false;
            }
        }

        public static void HandleEvents()
        {
            SDL.SDL_Event eventTriggered;
            while (SDL.SDL_PollEvent(out eventTriggered) != 0)
            {
                switch (eventTriggered.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        closed = true;
                        break;
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (eventTriggered.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
                            closed = true;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        Controller.AddControlToSequence(
                            GetKeyCodeFromScanCode(eventTriggered.key.keysym.scancode));
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        Controller.RemoveControlFromSequence(
                            GetKeyCodeFromScanCode(eventTriggered.key.keysym.scancode));
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        /*
                         * Commented out for now as they are expensive operations
                        Vector2 mouseLocation = new Vector2(eventTriggered.button.x, eventTriggered.button.y);
                        mouseRay = GetMouseRay(mouseLocation);
                        Entity hitEntity = Renderer.GetHitEntity(mouseLocation);
                        */
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Show()
        {
            SDL.SDL_ShowWindow(windowHandle);
        }

        public static void SwapBuffers()
        {
            SDL.SDL_GL_SwapWindow(windowHandle);
        }

        private static Ray GetMouseRay(Vector2 mouseLocation)
        {
            Vector3 nearPoint = GraphicsHelper.UnProject(
                new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f));
            Vector3 farPoint = GraphicsHelper.UnProject(
                new Vector3(mouseLocation.X, mouseLocation.Y, 1.0f));
            return new Ray(nearPoint, farPoint);
        }

        private static KeyCode GetKeyCodeFromScanCode(SDL.SDL_Scancode scanCode)
        {
            return (KeyCode)scanCode;
        }
    }
}
