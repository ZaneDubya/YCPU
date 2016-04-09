/***************************************************************************
 *   NativeConstants.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace Ypsilon.Core.Windows
{
    internal static class NativeConstants
    {
        public const int WmNull = 0x00;
        public const int WmCreate = 0x01;
        public const int WmDestroy = 0x02;
        public const int WmMove = 0x03;
        public const int WmSize = 0x05;
        public const int WmActivate = 0x06;
        public const int WmSetfocus = 0x07;
        public const int WmKillfocus = 0x08;
        public const int WmEnable = 0x0A;
        public const int WmSetredraw = 0x0B;
        public const int WmSettext = 0x0C;
        public const int WmGettext = 0x0D;
        public const int WmGettextlength = 0x0E;
        public const int WmPaint = 0x0F;
        public const int WmClose = 0x10;
        public const int WmQueryendsession = 0x11;
        public const int WmQuit = 0x12;
        public const int WmQueryopen = 0x13;
        public const int WmErasebkgnd = 0x14;
        public const int WmSyscolorchange = 0x15;
        public const int WmEndsession = 0x16;
        public const int WmSystemerror = 0x17;
        public const int WmShowwindow = 0x18;
        public const int WmCtlcolor = 0x19;
        public const int WmWininichange = 0x1A;
        public const int WmSettingchange = 0x1A;
        public const int WmDevmodechange = 0x1B;
        public const int WmActivateapp = 0x1C;
        public const int WmFontchange = 0x1D;
        public const int WmTimechange = 0x1E;
        public const int WmCancelmode = 0x1F;
        public const int WmSetcursor = 0x20;
        public const int WmMouseactivate = 0x21;
        public const int WmChildactivate = 0x22;
        public const int WmQueuesync = 0x23;
        public const int WmGetminmaxinfo = 0x24;
        public const int WmPainticon = 0x26;
        public const int WmIconerasebkgnd = 0x27;
        public const int WmNextdlgctl = 0x28;
        public const int WmSpoolerstatus = 0x2A;
        public const int WmDrawitem = 0x2B;
        public const int WmMeasureitem = 0x2C;
        public const int WmDeleteitem = 0x2D;
        public const int WmVkeytoitem = 0x2E;
        public const int WmChartoitem = 0x2F;

        public const int WmSetfont = 0x30;
        public const int WmGetfont = 0x31;
        public const int WmSethotkey = 0x32;
        public const int WmGethotkey = 0x33;
        public const int WmQuerydragicon = 0x37;
        public const int WmCompareitem = 0x39;
        public const int WmCompacting = 0x41;
        public const int WmWindowposchanging = 0x46;
        public const int WmWindowposchanged = 0x47;
        public const int WmPower = 0x48;
        public const int WmCopydata = 0x4A;
        public const int WmCanceljournal = 0x4B;
        public const int WmNotify = 0x4E;
        public const int WmInputlangchangerequest = 0x50;
        public const int WmInputlangchange = 0x51;
        public const int WmTcard = 0x52;
        public const int WmHelp = 0x53;
        public const int WmUserchanged = 0x54;
        public const int WmNotifyformat = 0x55;
        public const int WmContextmenu = 0x7B;
        public const int WmStylechanging = 0x7C;
        public const int WmStylechanged = 0x7D;
        public const int WmDisplaychange = 0x7E;
        public const int WmGeticon = 0x7F;
        public const int WmSeticon = 0x80;

        public const int WmNccreate = 0x81;
        public const int WmNcdestroy = 0x82;
        public const int WmNccalcsize = 0x83;
        public const int WmNchittest = 0x84;
        public const int WmNcpaint = 0x85;
        public const int WmNcactivate = 0x86;
        public const int WmGetdlgcode = 0x87;
        public const int WmNcmousemove = 0xA0;
        public const int WmNclbuttondown = 0xA1;
        public const int WmNclbuttonup = 0xA2;
        public const int WmNclbuttondblclk = 0xA3;
        public const int WmNcrbuttondown = 0xA4;
        public const int WmNcrbuttonup = 0xA5;
        public const int WmNcrbuttondblclk = 0xA6;
        public const int WmNcmbuttondown = 0xA7;
        public const int WmNcmbuttonup = 0xA8;
        public const int WmNcmbuttondblclk = 0xA9;

        public const int WmKeyfirst = 0x100;
        public const int WmKeydown = 0x100;
        public const int WmKeyup = 0x101;
        public const int WmChar = 0x102;
        public const int WmDeadchar = 0x103;
        public const int WmSyskeydown = 0x104;
        public const int WmSyskeyup = 0x105;
        public const int WmSyschar = 0x106;
        public const int WmSysdeadchar = 0x107;
        public const int WmKeylast = 0x108;
        public const int WmUnichar = 0x109;

        public const int WmIMEStartcomposition = 0x10D;
        public const int WmIMEEndcomposition = 0x10E;
        public const int WmIMEComposition = 0x10F;
        public const int WmIMEKeylast = 0x10F;

        public const int WmInitdialog = 0x110;
        public const int WmCommand = 0x111;
        public const int WmSyscommand = 0x112;
        public const int WmTimer = 0x113;
        public const int WmHscroll = 0x114;
        public const int WmVscroll = 0x115;
        public const int WmInitmenu = 0x116;
        public const int WmInitmenupopup = 0x117;
        public const int WmMenuselect = 0x11F;
        public const int WmMenuchar = 0x120;
        public const int WmEnteridle = 0x121;

        public const int WmCtlcolormsgbox = 0x132;
        public const int WmCtlcoloredit = 0x133;
        public const int WmCtlcolorlistbox = 0x134;
        public const int WmCtlcolorbtn = 0x135;
        public const int WmCtlcolordlg = 0x136;
        public const int WmCtlcolorscrollbar = 0x137;
        public const int WmCtlcolorstatic = 0x138;

        public const int WmMousefirst = 0x200;
        public const int WmMousemove = 0x200;
        public const int WmLbuttondown = 0x201;
        public const int WmLbuttonup = 0x202;
        public const int WmLbuttondblclk = 0x203;
        public const int WmRbuttondown = 0x204;
        public const int WmRbuttonup = 0x205;
        public const int WmRbuttondblclk = 0x206;
        public const int WmMbuttondown = 0x207;
        public const int WmMbuttonup = 0x208;
        public const int WmMbuttondblclk = 0x209;
        public const int WmMousewheel = 0x20A;
        public const int WmXbuttondown = 0x20B;
        public const int WmXbuttonup = 0x20C;
        public const int WmXbuttondblclk = 0x20D;
        public const int WmMousehwheel = 0x20E;

        public const int WmParentnotify = 0x210;
        public const int WmEntermenuloop = 0x211;
        public const int WmExitmenuloop = 0x212;
        public const int WmNextmenu = 0x213;
        public const int WmSizing = 0x214;
        public const int WmCapturechanged = 0x215;
        public const int WmMoving = 0x216;
        public const int WmPowerbroadcast = 0x218;
        public const int WmDevicechange = 0x219;

        public const int WmMdicreate = 0x220;
        public const int WmMdidestroy = 0x221;
        public const int WmMdiactivate = 0x222;
        public const int WmMdirestore = 0x223;
        public const int WmMdinext = 0x224;
        public const int WmMdimaximize = 0x225;
        public const int WmMditile = 0x226;
        public const int WmMdicascade = 0x227;
        public const int WmMdiiconarrange = 0x228;
        public const int WmMdigetactive = 0x229;
        public const int WmMdisetmenu = 0x230;
        public const int WmEntersizemove = 0x231;
        public const int WmExitsizemove = 0x232;
        public const int WmDropfiles = 0x233;
        public const int WmMdirefreshmenu = 0x234;

        public const int WmIMESetcontext = 0x281;
        public const int WmIMENotify = 0x282;
        public const int WmIMEControl = 0x283;
        public const int WmIMECompositionfull = 0x284;
        public const int WmIMESelect = 0x285;
        public const int WmIMEChar = 0x286;
        public const int WmIMEKeydown = 0x290;
        public const int WmIMEKeyup = 0x291;

        public const int WmMousehover = 0x2A1;
        public const int WmNcmouseleave = 0x2A2;
        public const int WmMouseleave = 0x2A3;

        public const int WmCut = 0x300;
        public const int WmCopy = 0x301;
        public const int WmPaste = 0x302;
        public const int WmClear = 0x303;
        public const int WmUndo = 0x304;

        public const int WmRenderformat = 0x305;
        public const int WmRenderallformats = 0x306;
        public const int WmDestroyclipboard = 0x307;
        public const int WmDrawclipboard = 0x308;
        public const int WmPaintclipboard = 0x309;
        public const int WmVscrollclipboard = 0x30A;
        public const int WmSizeclipboard = 0x30B;
        public const int WmAskcbformatname = 0x30C;
        public const int WmChangecbchain = 0x30D;
        public const int WmHscrollclipboard = 0x30E;
        public const int WmQuerynewpalette = 0x30F;
        public const int WmPaletteischanging = 0x310;
        public const int WmPalettechanged = 0x311;

        public const int WmHotkey = 0x312;
        public const int WmPrint = 0x317;
        public const int WmPrintclient = 0x318;

        public const int WmHandheldfirst = 0x358;
        public const int WmHandheldlast = 0x35F;
        public const int WmPenwinfirst = 0x380;
        public const int WmPenwinlast = 0x38F;
        public const int WmCoalesceFirst = 0x390;
        public const int WmCoalesceLast = 0x39F;
        public const int WmDdeFirst = 0x3E0;
        public const int WmDdeInitiate = 0x3E0;
        public const int WmDdeTerminate = 0x3E1;
        public const int WmDdeAdvise = 0x3E2;
        public const int WmDdeUnadvise = 0x3E3;
        public const int WmDdeAck = 0x3E4;
        public const int WmDdeData = 0x3E5;
        public const int WmDdeRequest = 0x3E6;
        public const int WmDdePoke = 0x3E7;
        public const int WmDdeExecute = 0x3E8;
        public const int WmDdeLast = 0x3E8;

        public const int WmUser = 0x400;
        public const int WmApp = 0x8000;

        public const int WhJournalrecord = 0;
        public const int WhJournalplayback = 1;
        public const int WhKeyboard = 2;
        public const int WhGetmessage = 3;
        public const int WhCallwndproc = 4;
        public const int WhCbt = 5;
        public const int WhSysmsgfilter = 6;
        public const int WhMouse = 7;
        public const int WhHardware = 8;
        public const int WhDebug = 9;
        public const int WhShell = 10;
        public const int WhForegroundidle = 11;
        public const int WhCallwndprocret = 12;
        public const int WhKeyboardLl = 13;
        public const int WhMouseLl = 14;

        public const int GwlWndproc = -4;
        public const int DlgcWantallkeys = 0x0004;
        public const int DlgcWantchars = 0x0080;
        public const int DlgcWanttab = 0x0002;
        public const int DlgcHassetsel = 0x0008;

        public const int LocaleIdefaultansicodepage = 0x1004;
        public const int LocaleReturnNumber = 0x20000000;
        public const int SortDefault = 0x0;
    }
}
