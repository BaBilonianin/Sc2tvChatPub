// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedBroadcasterPlugin.cs" company="Starboard">
//   Copyright © 2011 All Rights Reserved
// </copyright>
// <author> William Eddins </author>
// <summary>
//   Represents a XsplitPlugin object that will keep track of a Visual
//   object, and render updates based on a timer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XSplit.Wpf
{
    //using RatChat;
    //using System;
    //using System.Runtime.InteropServices;
    //using System.Threading;
    //using System.Threading.Tasks;
    //using System.Timers;
    //using System.Windows.Media;
    //using VHMediaCOMLib;

    //public class TimedBroadcasterPlugin : XsplitPlugin, IDisposable {
    //    #region Constants and Fields
    //    private bool disposed;
    //    private TaskScheduler taskScheduler;
    //    #endregion

    //    #region Constructors and Destructors

    //    protected TimedBroadcasterPlugin( VHCOMRenderEngineExtSrc2 xsplit )
    //        : base(xsplit) {
    //    }

    //    ~TimedBroadcasterPlugin() {
    //        this.Dispose(false);
    //    }

    //    #endregion

    //    public MainWindow Visual { get; private set; }

    //    public static TimedBroadcasterPlugin CreateInstance( string connectionUID, MainWindow visual ) {
    //        TimedBroadcasterPlugin plugin = null;

    //        try {
    //            var extsrc = new VHCOMRenderEngineExtSrc2 { ConnectionUID = connectionUID };
    //            plugin = new TimedBroadcasterPlugin(extsrc) {
    //                Visual = visual,
    //                taskScheduler = TaskScheduler.FromCurrentSynchronizationContext()
    //            };
    //        } catch (COMException) {
    //            // Do nothing, the plugin failed to load so null will be returned.
    //        }

    //        return plugin;
    //    }

        
    //    public void Dispose() {
    //        this.Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose( bool disposing ) {
    //        if (!this.disposed) {
    //        }
    //        this.disposed = true;
    //    }

    //    public void TransferVisual() {
    //        Task.Factory.StartNew(
    //            () => this.RenderVisual(
    //                this.Visual,
    //                (int)this.Visual.ActualWidth,
    //                (int)this.Visual.ActualHeight),
    //            CancellationToken.None,
    //            TaskCreationOptions.None,
    //            this.taskScheduler);
    //    }

    //}
}