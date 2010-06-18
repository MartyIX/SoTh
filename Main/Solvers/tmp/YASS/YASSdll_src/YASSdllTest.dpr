{
YASS - Yet Another Sokoban Solver Plugin
Version 0.02
Copyright (c) 2005-2009 by Brian Damgaard, Denmark

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
}

{$DEFINE DELPHI}                               {compiler: use only one of these: DELPHI of FPC}
{///$DEFINE FPC}                               {compiler: use only one of these: DELPHI of FPC}

{$DEFINE WINDOWS}                              {use this on the Windows platform only}

{$IFDEF DELPHI}
  {$APPTYPE CONSOLE}                           {use this with DELPHI only}
  {$UNDEF FPC}                                 {for safety}
  {$M 1024*1024,1024*1024}                     {stack size, minumum 1 MB}
{$ENDIF}

{$IFDEF FPC}
  {$MODE OBJFPC} {$PACKENUM 1}                 {use this with FPC only}
  {$UNDEF DELPHI}                              {for safety}
  {$M 1048576,0}                               {stack size, minumum 1 MB}
{$ENDIF}

{$R-}                                          {'R-': range checking disabled}


program YASSdllTest;
uses SysUtils,
  Windows,
  Messages,
  Forms,
  Controls,
  YASSdllSettings_ in 'YASSdllSettings_.pas' {YASSdllSettingsForm};

const
  NL                           = #13#10;
  TEXT_APPLICATION_TITLE       = 'YASS - Yet another Sokoban solver plugin';
  TEXT_SETTINGS_WINDOW_CAPTION
                               = 'YASS - Settings';
  YASS_DLL                     = 'YASSDLL.dll';

type
  TBoardAsChars = array of array of Char;

var
  MaxWidth,MaxHeight,MaxBoxCount:Cardinal;
  PluginName:array[0..511] of Char;

procedure Configure(Window__: HWND) stdcall; external YASS_DLL;

procedure GetConstraints(var MaxWidth__,MaxHeight__,MaxBoxCount__: Cardinal) stdcall; external YASS_DLL;

procedure GetPluginName(PluginName__: PChar; MaxPluginNameLength__: Cardinal) stdcall; external YASS_DLL;

procedure ShowAbout(Window: HWND) stdcall; external YASS_DLL;

function  Solve(Width__, Height__: Cardinal; BoardAsChars__: TBoardAsChars; SolutionAsUDLR__: PChar; MaxSolutionAsUDLRSize__: Cardinal): Integer; stdcall; external YASS_DLL;


procedure Settings;
var WindowHandle:Hwnd;

  function WindowProcedure(hWnd, uMsg,	wParam,	lParam: Integer): Integer; stdcall;
  begin
    case uMsg of
      WM_CREATE         : begin Result:=0; exit;
                          end;
      WM_DESTROY        : begin PostQuitMessage(0); Result:=0; exit;
                          end;
    end; // case
    Result := DefWindowProc(hWnd, uMsg, wParam, lParam);
  end;

  function RegisterWindow:Boolean;
  var WindowClass:TWndClassA;
  begin
    FillChar(WindowClass,SizeOf(WindowClass),0);
    WindowClass.Style              := CS_CLASSDC or CS_PARENTDC or CS_DBLCLKS or CS_OWNDC or CS_HREDRAW or CS_VREDRAW;
    WindowClass.LpfnWndProc        := @WindowProcedure;
    WindowClass.HInstance          := HInstance;
    WindowClass.HIcon              := LoadIcon(HInstance,IDI_APPLICATION);
    WindowClass.HCursor            := LoadCursor(0, IDC_ARROW);
    WindowClass.HbrBackground      := Color_BtnFace + 1;
    WindowClass.LpszClassname      := PChar(TEXT_APPLICATION_TITLE);
    Result:=RegisterClass(WindowClass)<>0;
  end;

  function CreateWindow(var WindowHandle__:Hwnd):Boolean;
  begin

    WindowHandle__:=CreateWindowEx(WS_EX_WINDOWEDGE,
                 PChar(TEXT_APPLICATION_TITLE),
                 PChar(TEXT_SETTINGS_WINDOW_CAPTION),
                 WS_OVERLAPPEDWINDOW or WS_VISIBLE or WS_SIZEBOX or WS_CAPTION or WS_SYSMENU,
                 30, 50, 480, 300, 0, 0, HInstance, nil);;
    Result:=WindowHandle__<>0;
  end;

  function RunMessageLoop(WindowHandle__:Hwnd):Integer;
  var Msg:TMsg;
  begin
    if WindowHandle__<>0 then begin
       UpdateWindow(WindowHandle__);
       while True do
         if PeekMessage(Msg,0,0,0,PM_REMOVE) then
            if   Msg.Message<>WM_QUIT then begin
                 TranslateMessage(Msg);
                 DispatchMessage(Msg);
                 end
            else break;
       Result:=Msg.wParam;
       end
    else Result:=0;
  end;

begin
  if RegisterWindow and CreateWindow(WindowHandle) then
     RunMessageLoop(WindowHandle);
end;

// main ======================================================================

begin // main program

  YASSdllSettingsForm:=TYASSdllSettingsForm.Create(nil);
  try     if YASSdllSettingsForm.ShowModal=mrOk then begin
             end;
  finally YASSdllSettingsForm.Free;
  end;
  exit;

  Settings;
  exit;

// about... -------------------------------------------------------------------
  ShowAbout(0);


// plugin name ----------------------------------------------------------------
  GetPluginName(PChar(Addr(PluginName)),SizeOf(PluginName)-SizeOf(Char));
  MessageBox(0,Format('Plugin name:'+NL+'%s',[PChar(Addr(PluginName))]),TEXT_APPLICATION_TITLE,MB_OK+MB_ICONINFORMATION);

// contraints -----------------------------------------------------------------
  GetConstraints(MaxWidth,MaxHeight,MaxBoxCount);
  MessageBox(0,Format('%s'+NL+'Maximum dimensions: %d x %d, maximum number of boxes: %d',
                      [String(PChar(Addr(PluginName))),MaxWidth,MaxHeight,MaxBoxCount]),
             TEXT_APPLICATION_TITLE,
             MB_OK+MB_ICONINFORMATION);

// configure ------------------------------------------------------------------
  Configure(0);
end.

