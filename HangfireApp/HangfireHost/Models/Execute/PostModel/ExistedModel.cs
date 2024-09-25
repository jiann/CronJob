﻿namespace HangfireHost.Models.Execute.PostModel
{
    public class ExistedModel
    {
        /// <summary>
        /// execute file path
        /// </summary>
        public string MainFilePath { get; set; } = string.Empty;
        
        /// <summary>
        /// main file name, e.g. "main.exe"
        /// </summary>
        /// <example>ConsoleApp1.exe</example>
        public string MainFileName { get; set; } = string.Empty;
        
        /// <summary>
        /// The arguments to pass to the main file 
        /// </summary>
        public string? Args { get; set; }
        
        /// <summary>
        /// execute interval minutes
        /// </summary>
        public int IntervalMinutes { get; set; }
    }
}