/***************************************************************************
 *   MessageHook.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ypsilon.Core.Windows
{
    public abstract class MessageHook : IDisposable
    {
        public abstract int HookType { get; }

        private IntPtr m_HWnd;
        private WndProcHandler m_Hook;
        private IntPtr m_PrevWndProc;
        private IntPtr m_HImc;

        public IntPtr HWnd => m_HWnd;

        public MessageHook(IntPtr hWnd)
        {
            m_HWnd = hWnd;
            m_Hook = WndProcHook;
            m_PrevWndProc = (IntPtr)NativeMethods.SetWindowLong(
                hWnd,
                NativeConstants.GwlWndproc, (int)Marshal.GetFunctionPointerForDelegate(m_Hook));
            m_HImc = NativeMethods.ImmGetContext(m_HWnd);
            new InputMessageFilter(m_Hook);
        }

        ~MessageHook()
        {
            Dispose(false);
        }

        protected virtual IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeConstants.WmGetdlgcode:
                    return (IntPtr)(NativeConstants.DlgcWantallkeys);
               case NativeConstants.WmIMESetcontext:
                    if ((int)wParam == 1)
                        NativeMethods.ImmAssociateContext(hWnd, m_HImc);
                    break;
                case NativeConstants.WmInputlangchange:
                    int rrr = (int)NativeMethods.CallWindowProc(m_PrevWndProc, hWnd, msg, wParam, lParam);
                    NativeMethods.ImmAssociateContext(hWnd, m_HImc);
                    
                    return (IntPtr)1;
            }

            return NativeMethods.CallWindowProc(m_PrevWndProc, hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }

    // This is the class that brings back the alt messages
    // http://www.gamedev.net/community/forums/topic.asp?topic_id=554322
    internal class InputMessageFilter : IMessageFilter
    {
        private WndProcHandler m_Hook;

        public InputMessageFilter(WndProcHandler hook)
        {
            m_Hook = hook;
            Application.AddMessageFilter(this);
        }
        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        protected extern static bool m_TranslateMessage(ref System.Windows.Forms.Message m);

        bool IMessageFilter.PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeConstants.WmSyskeydown:
                case NativeConstants.WmSyskeyup:
                    {
                        bool b = m_TranslateMessage(ref m);
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }

                case NativeConstants.WmSyschar:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WmKeydown:
                case NativeConstants.WmKeyup:
                    {
                        bool b = m_TranslateMessage(ref m);
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WmChar:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WmDeadchar:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
            }
            return false;
        }
    }
}
