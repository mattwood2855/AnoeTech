using System;

namespace AnoeTech
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Anoetech game = new Anoetech())
            {
                game.Run();
            }
        }
    }
#endif
}

