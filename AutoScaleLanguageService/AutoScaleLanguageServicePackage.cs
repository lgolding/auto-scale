// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;

namespace Lakewood.AutoScale
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [Guid(PackageGuidString)]
    [ProvideService(
        typeof(AutoScaleLanguageService),
        ServiceName = AutoScaleLanguageService.LanguageName + " Language Service")]
    [ProvideLanguageExtension(
        typeof(AutoScaleLanguageService),
        ".autoscale")]
    [ProvideLanguageService(
        typeof(AutoScaleLanguageService),
        AutoScaleLanguageService.LanguageName,
        106,
        CodeSense = true,
        DefaultToInsertSpaces = true,
        RequestStockColors = true,
        EnableCommenting = false,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        ShowMatchingBrace = true,
        ShowCompletion = true,
        EnableAsyncCompletion = true)]
    public sealed class AutoScaleLanguageServicePackage : Package, IOleComponent
    {
        public const string PackageGuidString = "5c0b1391-daa4-441d-8e67-60dfb13ab271";

        private uint _componentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScaleLanguageServicePackage"/> class.
        /// </summary>
        public AutoScaleLanguageServicePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside the Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Proffer the service.
            var serviceContainer = this as IServiceContainer;
            var languageService = new AutoScaleLanguageService();
            languageService.SetSite(this);
            serviceContainer.AddService(
                typeof(AutoScaleLanguageService),
                languageService,
                true);

            // Register a timer to call our language service during idle periods.
            var mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
            if (_componentId == 0 && mgr != null)
            {
                var crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                              (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                              (uint)_OLECADVF.olecadvfRedrawOff |
                                              (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = mgr.FRegisterComponent(this, crinfo, out _componentId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_componentId != 0)
            {
                var mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
                if (mgr != null)
                {
                    int hr = mgr.FRevokeComponent(_componentId);
                }

                _componentId = 0;
            }

            base.Dispose(disposing);
        }

        #endregion Package Members

        #region IOleComponent Members

        // Of the IOleComponent interface members, we need only implement FDoIdle, which
        // enables background parsing. All other members can return default values.
        // See https://msdn.microsoft.com/en-us/library/bb166498.aspx.
        public int FDoIdle(uint grfidlef)
        {
            bool bPeriodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;

            var service = GetService(typeof(AutoScaleLanguageService)) as LanguageService;
            if (service != null)
            {
                service.OnIdle(bPeriodic);
            }

            return 0;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public void Terminate()
        {
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        #endregion IOleComponent Members
    }
}
