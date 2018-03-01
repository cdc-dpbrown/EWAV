/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       RefreshTimer.cs
 *  Namespace:  Ewav.Common    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows.Threading;

namespace Ewav.Common
{
    //class Singleton
    //{
    //    public static readonly Singleton   _instance = new SingletonB();
    //    public void Test()
    //    {
    //        // Code runs.
    //    }
    //    Singleton(  )  
    //    {
    //    }
    //}
    public class RefreshTimer
    {
        public delegate void RefreshTimerFiredEventHandler(object o);

        // DO NOT MODIFY    
        public static readonly RefreshTimer Instance = new RefreshTimer();

        readonly DispatcherTimer dt;

        public event RefreshTimerFiredEventHandler RefreshTimerFiredEvent;

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        /// <value>The seconds.</value>
        public int Seconds { get; set; }

        /// <summary>
        /// WARNING – This constructor cannot be public.  
        /// Making the constructor of this singleton class public will rip a hole in space-time, 
        /// surely destroying us all. -DS                               
        /// </summary>
        private RefreshTimer()
        {
            this.dt = new DispatcherTimer();

            this.dt.Tick += new EventHandler(dt_Tick);
        }

        public void Start()
        {
            this.dt.Interval = new TimeSpan(0, 0, 0, this.Seconds);          
            this.dt.Start();    
        }

        public void Stop()
        {
            this.dt.Stop();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            this.RefreshTimerFiredEvent(this);
        }
    }
}