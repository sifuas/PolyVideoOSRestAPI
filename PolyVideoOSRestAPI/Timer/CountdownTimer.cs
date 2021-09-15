using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI.Timer
{
    /// <summary>
    /// Countdown Timer
    /// </summary>
    public class CountdownTimer
    {
        /// <summary>
        /// Callbacks that are triggered by the Countdown Timer
        /// </summary>
        public event EventHandler<EventArgs> TimerStarted;
        public event EventHandler<EventArgs> TimerExpired;
        public event EventHandler<EventArgs> TimerStopped;
        public event EventHandler<EventArgs> TimerCancelled;

        /// <summary>
        /// Callback that is triggered every second and returns the time remaining.
        /// </summary>
        public event EventHandler<TimerEventArgs> TimerValueChanged;

        /// <summary>
        /// Set the time based on the total number of seconds representing the total time value.
        /// </summary>
        public double TotalTimeInSeconds
        {
            get { return TimeValue.TotalSeconds; }
            set
            {
                TimeValue = TimeSpan.FromSeconds(value);
            }
        }

        public ushort TotalTimeInSecondsUShort
        {
            get { return (ushort)TotalTimeInSeconds; }
            set 
            {                
                TotalTimeInSeconds = (double)value; 
            }
        }

        /// <summary>
        /// Set the amount of time for the countdown to use.
        /// </summary>
        public int Seconds 
        {
            get { return TimeValue.Seconds; }
            set
            {
                TimeValue = new TimeSpan(TimeValue.Hours, TimeValue.Minutes, value);                
            }
        }

        public int Minutes 
        {
            get { return TimeValue.Minutes; }
            set
            {
                TimeValue = new TimeSpan(TimeValue.Hours, value, TimeValue.Seconds);
            }
        }

        public int Hours 
        {
            get { return TimeValue.Hours; }
            set
            {
                TimeValue = new TimeSpan(value, TimeValue.Minutes, TimeValue.Seconds);
            }
        }

        /// <summary>
        /// Set the amount of time for the countdown to use as a TimeSpan.
        /// </summary>
        public TimeSpan TimeValue { get; set; }

        /// <summary>
        /// Indicates whether the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// The start and end times for the timer.
        /// Dates are only valid if IsRunning is true;
        /// </summary>
        public DateTime StartTime { get; private set; }
        public DateTime FinishTime { get; private set; }

        /// <summary>
        /// The amount of time remaining on the timer if it is running.
        /// </summary>
        public TimeSpan TimeRemaining { get; private set; }

        /// <summary>
        /// The amount of time remaining on the timer as a percent of the total time.
        /// Value from 0 - 100
        /// </summary>
        public int PercentRemaining { get; private set; }

        /// <summary>
        /// The amount of tiem remaining on the timer as a serial value.
        /// Displays Hours, Minutes and Seconds.
        /// For Example:
        ///     1 Hr 2 Mins. 3 Secs.
        ///     1 Min., 2 Secs.
        ///     1 Sec.
        /// </summary>
        public string SerialValue { get; private set; }

        /// <summary>
        /// The background timer to use
        /// </summary>
        private CTimer timerThread;

        /// <summary>
        /// Default constructor for Simpl+
        /// </summary>
        public CountdownTimer()
        {
            StartTime   = DateTime.Now;
            FinishTime  = StartTime;
            TimeValue   = new TimeSpan(0, 0, 0);
            timerThread = new CTimer(TimerCallback, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Start the timer based on the time values set.
        /// Fires the TimerStarted Event
        /// </summary>
        public void Start( )
        {            
            if (IsRunning)
                return;

            timerThread.Stop();

            // calculate when the timer will expire
            StartTime   = DateTime.Now;
            FinishTime  = StartTime + TimeValue;

            // setup the timer to repeat every second
            timerThread.Reset(1000);
            IsRunning   = true;

            // CrestronConsole.PrintLine("Starting CountdownTimer : TImeValue = {0}, StartTime = {1}, FinishTime = {2}", TimeValue, StartTime, FinishTime);

            // trigger callbacks if they are registered
            var handler = TimerStarted;
            if (handler != null)
                handler(this, new EventArgs());

            UpdateState();
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            IsRunning = false;
            Start();
        }

        /// <summary>
        /// Stop the timer.
        /// Fires the TimerStopped event.
        /// </summary>
        public void Stop()
        {
            Stop(true);
        }

        private void Stop(bool fireUpade)
        {
            timerThread.Stop();
            IsRunning = false;
            TimeRemaining = new TimeSpan(0, 0, 0);

            // trigger callbacks if they are registered
            var handler = TimerStopped;
            if (fireUpade && (handler != null))
                TimerStopped(this, new EventArgs());
        }

        /// <summary>
        /// Ends the timer and triggers the callback function.
        /// Fires the TimerFinished Event.
        /// </summary>
        public void End()
        {
            Stop(false);

            // trigger callbacks if they are registered
            var handler = TimerExpired;
            if (handler != null)
                handler(this, new EventArgs());

            // clear feedback
            var timerhandler = TimerValueChanged;
            if (timerhandler != null)
                timerhandler(this, new TimerEventArgs(TimeRemaining, 0, ""));
        }

        /// <summary>
        /// Cancel the timer.
        /// Fires the TimerCanceled Event.
        /// </summary>
        public void Cancel()
        {
            Stop(false);

            var handler = TimerCancelled;
            if( handler != null )
                handler( this, new EventArgs() );
        }

        /// <summary>
        /// Function called when the timer expires.
        /// </summary>
        /// <param name="obj"></param>
        private void TimerCallback(Object obj)
        {
            // check if the timer has expired
            if (DateTime.Now >= FinishTime)
            {
                End();
            }
            else
            {
                timerThread.Reset(1000);
            }

            if( IsRunning )
                CrestronInvoke.BeginInvoke( callbackFunc => UpdateState());
        }

        /// <summary>
        /// If defined, fire the callback to update the time remaining.
        /// </summary>
        private void UpdateState( )
        {
            TimeRemaining = FinishTime - DateTime.Now;
             
            // calculate the time remaining and create the feedback
            string plural = (TimeRemaining.Seconds != 1) ? "s" : "";

            String formattedTimeString = String.Format("{0} Sec{1}.", TimeRemaining.Seconds, plural);

            if ((TimeRemaining.Hours > 0) || (TimeRemaining.Minutes > 0))
            {
                plural = (TimeRemaining.Minutes != 1) ? "s" : "";
                formattedTimeString = String.Format("{0} Min{1}. {2}", TimeRemaining.Minutes, plural, formattedTimeString);
            }

            if (TimeRemaining.Hours > 0)
            {
                plural = (TimeRemaining.Hours != 1) ? "s" : "";
                formattedTimeString = String.Format("{0} Hr{1}. {2}", TimeRemaining.Hours, plural, formattedTimeString);
            }

            SerialValue = formattedTimeString;

            // calculate the percentage of time left out of the total time that the timer started with.
            double remainingTimeSeconds = Math.Round(TimeRemaining.TotalSeconds);
            double totalTimeSeconds     = Math.Round((FinishTime - StartTime).TotalSeconds);
            
            PercentRemaining = (ushort)Math.Round((remainingTimeSeconds / totalTimeSeconds) * 100);

            // trigger callbacks if they are registered
            var handler = TimerValueChanged;
            if (handler != null)                
                handler(this, new TimerEventArgs(TimeRemaining, (ushort)PercentRemaining, SerialValue));
            
        }
    }
}