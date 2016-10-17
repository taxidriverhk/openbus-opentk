using System;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using SDL2;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Screen
    {
        #region Private Members
        private static bool closed;
        private static bool initialized;

        private static int width;
        private static int height;

        private static IntPtr windowHandle;
        private static IntPtr glContext;
        private static GraphicsContext graphicsContext;
        #endregion

        #region Properties
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
        #endregion

        #region Public Methods
        public static bool Initialize(int inputWidth, int inputHeight, string title)
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
                SDL.SDL_GL_GetProcAddress,
                () => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));

            width = inputWidth;
            height = inputHeight;

            closed = false;
            initialized = true;
            return true;
        }

        public static void Destroy()
        {
            if(initialized)
            {
                graphicsContext.Dispose();
                SDL.SDL_GL_DeleteContext(glContext);
                SDL.SDL_DestroyWindow(windowHandle);
                SDL.SDL_QuitSubSystem(SDL.SDL_INIT_VIDEO);
                initialized = false;
            }
        }

        public static void HandleEvents()
        {
            SDL.SDL_Event eventTriggered;
            while(SDL.SDL_PollEvent(out eventTriggered) != 0)
            {
                switch(eventTriggered.type)
                {
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        if (eventTriggered.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
                            closed = true;
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
        #endregion
    }
}
