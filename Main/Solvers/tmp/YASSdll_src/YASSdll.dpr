library YASSdll;

// ============================================================================
// Compiling instructions
//
// The conditional symbols in 'YASS.pas' must be edited in order to compile
// this dll version of the plugin:
//
// disable: {$DEFINE CONSOLE_APPLICATION}
// enable : {$DEFINE PLUGIN_MODULE}
//
// disable: {$DEFINE FPC}
// enable : {$DEFINE DELPHI}
//
// enable : {$DEFINE WINDOWS}
// ============================================================================

uses SysUtils, Windows,
     YASS,YASSdllSettings_;

type
  TPCharVector  = array[0..(MaxInt div SizeOf(PChar))-1] of PChar; // vector or PChar pointers
  PTPCharVector = ^TPCharVector; // a pointer to a vector of PChar pointers; in the function 'Solve(...)', the C-declaration of the board is "Char**"

function SolveEx(Width__, Height__        : Cardinal;
                 BoardAsText__            : PChar;
                 SolutionAsText__         : PChar;    // null-terminated string
                 SolutionAsTextSize__     : Cardinal; // including null-terminator
                 SokobanStatusPointer__   : PSokobanStatus;
                 SokobanCallBackFunction__: TSokobanCallBackFunction): Integer; stdcall; forward;

procedure Configure(WindowHandle__: HWND) stdcall;
var YASSdllSettingsForm:TYASSdllSettingsForm;
begin
//MessageBox(0,'Configure...',TEXT_APPLICATION_TITLE,MB_OK+MB_ICONINFORMATION);
  try    YASSdllSettingsForm:=TYASSdllSettingsForm.Create(nil);
         try     YASSdllSettingsForm.ParentWindow:=WindowHandle__;
                 YASSdllSettingsForm.ShowModal;
         finally YASSdllSettingsForm.Free;
         end;
  except on E:Exception do MessageBox(WindowHandle__,E.Message,TEXT_APPLICATION_TITLE,MB_OK+MB_ICONERROR);
  end;
end;

procedure GetConstraints(var MaxWidth__,MaxHeight__,MaxBoxCount__: Cardinal) stdcall;
begin
  MaxWidth__    := MAX_BOARD_WIDTH;
  MaxHeight__   := MAX_BOARD_HEIGHT;
  MaxBoxCount__ := MAX_BOX_COUNT;
end;

procedure GetPluginName(PluginName__: PChar; MaxPluginNameLength__: Cardinal) stdcall;
begin
  StrLCopy(PluginName__, TEXT_APPLICATION_TITLE+SPACE+TEXT_APPLICATION_VERSION_NUMBER, MaxPluginNameLength__);
end;

function GetSupportedOptimizations: Integer; stdcall; // deprecated; in fact, it was never publically documented so only a few early versions of the YASS plugin used it
begin
  Result:=SOKOBAN_PLUGIN_FLAG_MOVES           + SOKOBAN_PLUGIN_FLAG_SECONDARY_PUSHES + // moves, pushes (primary+secondary optimizations: bits 15-0)
          SOKOBAN_PLUGIN_FLAG_PUSHES          + SOKOBAN_PLUGIN_FLAG_SECONDARY_MOVES  + // pushes, moves
          SOKOBAN_PLUGIN_FLAG_PUSHES*65536;                                            // pushes only   (primary optimizations only     : bits 21-16)
end;

function IsSupportedOptimization(Optimization__: Cardinal): Integer; stdcall;
begin
  if   (Optimization__ = SOKOBAN_PLUGIN_FLAG_MOVES           + SOKOBAN_PLUGIN_FLAG_SECONDARY_PUSHES) // moves, pushes
       or
       (Optimization__ = SOKOBAN_PLUGIN_FLAG_PUSHES          + SOKOBAN_PLUGIN_FLAG_SECONDARY_MOVES)  // pushes, moves
       or
       (Optimization__ = SOKOBAN_PLUGIN_FLAG_PUSHES)                                                 // pushes only
       or
       (Optimization__ = SOKOBAN_PLUGIN_FLAG_BOX_LINES       + SOKOBAN_PLUGIN_FLAG_SECONDARY_MOVES)  // boxlines, moves
       or
       (Optimization__ = SOKOBAN_PLUGIN_FLAG_BOX_LINES       + SOKOBAN_PLUGIN_FLAG_SECONDARY_PUSHES) // boxlines, moves
       then
       Result:=1  // true
  else Result:=0; // false
end;

function Optimize(Width__, Height__        : Cardinal;
                  BoardAsText__            : PChar;
                  GameAsText__             : PChar;    // one or more null-terminated strings, with an extra null-terminator at the end to signal the end of the list
                  GameAsTextSize__         : Cardinal; // size of the 'GameAsText__' buffer in bytes
                  SokobanStatusPointer__   : PSokobanStatus;
                  SokobanCallBackFunction__: TSokobanCallBackFunction): Integer; stdcall;
var Col,Row,Index:Integer; IsASolution:Boolean; GameAsText,InitializeErrorText:String;
    InitializePluginResult:TPluginResult;
    YASSdllSettingsForm:TYASSdllSettingsForm;
begin
  if   (Width__ >=3) and (Width__ <=MAX_BOARD_WIDTH ) and
       (Height__>=3) and (Height__<=MAX_BOARD_HEIGHT) then
       if StrLen(BoardAsText__)=Width__*Height__ then
          try    YASSdllSettingsForm:=TYASSdllSettingsForm.Create(nil); // loads and validates the settings
                 try     with YASSdllSettingsForm do
                           if LoadSettingsFromIniFileResult=prOK then
                              try
                                   Positions.MemoryByteSize:=SpinEditOptimizerTransPositionTableSize.Value*1024*1024;
                                   if   CheckBoxOptimizerSearchTime.Checked then
                                        Solver.SearchLimits.TimeLimitMS:=SpinEditOptimizerMaxTime.Value*1000
                                   else Solver.SearchLimits.TimeLimitMS:=High(Solver.SearchLimits.TimeLimitMS); {high-value signals unlimited search time}
                                   Solver.SearchLimits.PushCountLimit:=High(Solver.SearchLimits.PushCountLimit);
                                   Solver.SearchLimits.DepthLimit:=MAX_HISTORY_BOX_MOVES;
                                   Optimizer.SearchLimits.PushCountLimit:=Solver.SearchLimits.PushCountLimit;
                                   Optimizer.SearchLimits.DepthLimit:=Solver.SearchLimits.DepthLimit;
                                   Optimizer.SearchLimits.TimeLimitMS:=Solver.SearchLimits.TimeLimitMS;
                                   Solver.BackwardSearchDepthLimit:=Solver.SearchLimits.DepthLimit;
                                   Game.DeadlockSets.AdjacentOpenSquaresLimit:=SpinEditOptimizerPrecalculatedDeadlocksComplexity.Value;
                                   Game.DeadlockSets.BoxLimitForDynamicSets:=DEFAULT_DEADLOCK_SETS_BOX_LIMIT_FOR_DYNAMIC_SETS;
                                   Game.DeadlockSets.BoxLimitForPrecalculatedSets:=DEFAULT_DEADLOCK_SETS_BOX_LIMIT_FOR_PRECALCULATED_SETS;
                                   Solver.SearchMethod:=SearchMethod;
                                   Solver.Enabled:=False;
                                   Solver.FindPushOptimalSolution:=True;
                                   Optimizer.Enabled:=True;
                                   Solver.ShowBestPosition:=False;
                                   Solver.StopWhenSolved:=CheckBoxStopWhenSolved.Checked;
                                   UserInterface.Prompt:=True;
                                   Solver.ReuseNodesEnabled:=False;
                                   LogFile.Enabled:=CheckBoxOptimizerLog.Checked;
                                   Solver.PackingOrder.Enabled:=CheckBoxPackingOrderSearch.Checked;
                                   Solver.PackingOrder.BoxCountThreshold:=SpinEditPackingOrderSearchThreshold.Value;
                                   Solver.SokobanCallBackFunction:=SokobanCallBackFunction__;
                                   Solver.SokobanStatusPointer:=SokobanStatusPointer__;
                                   Optimizer.MethodEnabled   [omBoxPermutations                        ]:=CheckBoxFallbackStrategyOptimization.Checked; {the fallback strategy is the "box permutations" method}
                                   Optimizer.MethodEnabled   [omRearrangement                          ]:=CheckBoxRearrangementOptimization   .Checked;
                                   Optimizer.MethodEnabled   [omGlobalSearch                           ]:=CheckBoxGlobalOptimization          .Checked;
                                   Optimizer.MethodEnabled   [omBoxPermutationsWithTimeLimit           ]:=CheckBoxBoxPermutationsOptimization .Checked;
                                   Optimizer.MethodEnabled   [omVicinitySearch                         ]:=CheckBoxVicinityOptimization        .Checked;

                                   Optimizer.MethodOrder     [Low(Optimizer.MethodOrder)               ]:=Low(TOptimizationMethod); {the fallback method is the first one}
                                   Optimizer.MethodOrder     [SpinEditRearrangementOptimization.Value  ]:=omRearrangement;
                                   Optimizer.MethodOrder     [SpinEditGlobalOptimization.Value         ]:=omGlobalSearch;
                                   Optimizer.MethodOrder     [SpinEditBoxPermutationsOptimization.Value]:=omBoxPermutationsWithTimeLimit;
                                   Optimizer.MethodOrder     [SpinEditVicinityOptimization.Value       ]:=omVicinitySearch;

                                   if   (SokobanStatusPointer__<>nil) and
                                        ((SokobanStatusPointer__^.Flags and (SOKOBAN_PLUGIN_FLAG_MOVES+SOKOBAN_PLUGIN_FLAG_PUSHES+SOKOBAN_PLUGIN_FLAG_BOX_LINES))<>0) then begin
                                        if   (SokobanStatusPointer__^.Flags and SOKOBAN_PLUGIN_FLAG_BOX_LINES)=0 then
                                             // use caller's moves or pushes selection
                                             if        (SokobanStatusPointer__^.Flags and SOKOBAN_PLUGIN_FLAG_MOVES)<>0 then
                                                       Optimizer.Optimization:=opMovesPushes
                                             else if   (SokobanStatusPointer__^.Flags and SOKOBAN_PLUGIN_FLAG_SECONDARY_MOVES)<>0 then
                                                       Optimizer.Optimization:=opPushesMoves
                                                  else Optimizer.Optimization:=opPushesOnly
                                        else // use caller's boxlines selection
                                             if        (SokobanStatusPointer__^.Flags and SOKOBAN_PLUGIN_FLAG_SECONDARY_MOVES)<>0 then
                                                       Optimizer.Optimization:=opBoxLinesMoves
                                             else if   (SokobanStatusPointer__^.Flags and SOKOBAN_PLUGIN_FLAG_SECONDARY_PUSHES)<>0 then
                                                       Optimizer.Optimization:=opBoxLinesPushes
                                                  else Optimizer.Optimization:=Optimization; // use current settings
                                        end
                                   else Optimizer.Optimization:=Optimization;   // use current settings

                                   with Optimizer do begin
                                     FillChar(VicinitySettings,SizeOf(VicinitySettings),0);
                                     Index:=High(VicinitySettings);
                                     if CheckBoxVicinityBox1.Checked and (SpinEditVicinityBox1.Value>=1) then begin VicinitySettings[Index]:=SpinEditVicinityBox1.Value; Dec(Index); end;
                                     if CheckBoxVicinityBox2.Checked and (SpinEditVicinityBox2.Value>=1) then begin VicinitySettings[Index]:=SpinEditVicinityBox2.Value; Dec(Index); end;
                                     if CheckBoxVicinityBox3.Checked and (SpinEditVicinityBox3.Value>=1) then begin VicinitySettings[Index]:=SpinEditVicinityBox3.Value; Dec(Index); end;
                                     if CheckBoxVicinityBox4.Checked and (SpinEditVicinityBox4.Value>=1) then begin VicinitySettings[Index]:=SpinEditVicinityBox4.Value; end;
                                     QuickVicinitySearchEnabled:=CheckBoxQuickVicinitySearch.Checked;
                                     end;

                                   if YASS.Initialize(
                                        Positions.MemoryByteSize,
                                        Solver.SearchLimits.PushCountLimit,
                                        Solver.SearchLimits.DepthLimit,
                                        Solver.BackwardSearchDepthLimit,
                                        Optimizer.SearchLimits.PushCountLimit,
                                        Optimizer.SearchLimits.DepthLimit,
                                        Game.DeadlockSets.AdjacentOpenSquaresLimit,
                                        Game.DeadlockSets.BoxLimitForDynamicSets,
                                        Game.DeadlockSets.BoxLimitForPrecalculatedSets,
                                        Solver.SearchMethod,
                                        Solver.Enabled,
                                        Optimizer.Enabled,
                                        Optimizer.QuickVicinitySearchEnabled,
                                        Solver.ShowBestPosition,//False,
                                        Solver.StopWhenSolved,
                                        UserInterface.Prompt,
                                        Solver.ReuseNodesEnabled,
                                        LogFile.Enabled,
                                        Solver.PackingOrder.Enabled,
                                        Solver.PackingOrder.BoxCountThreshold,
                                        Solver.SearchLimits.TimeLimitMS,
                                        Optimizer.SearchLimits.TimeLimitMS,
                                        Optimizer.BoxPermutationsSearchTimeLimitMS,
                                        Optimizer.MethodEnabled,
                                        Optimizer.MethodOrder,
                                        Optimizer.Optimization,
                                        Optimizer.VicinitySettings,
                                        Solver.SokobanCallBackFunction,
                                        Solver.SokobanStatusPointer) then begin

                                      InitializeBoard(Width__,Height__,True);
                                      for Row:=1 to Game.BoardHeight do
                                          for Col:=1 to Game.BoardWidth do begin
                                              Game.Board[ColRowToSquare(Col,Row)]:=Legend.CharToItem[BoardAsText__^];
                                              Inc(BoardAsText__); // get ready for next character
                                              end;
                                      if LogFile.Enabled then CreateLogFile(ChangeFileExt(IniFileName,LOG_FILE_EXT));

                                      if InitializeGame(InitializePluginResult,InitializeErrorText) then begin
                                         //MessageBox(Format('Width: %d, Height: %d',[Game.BoardWidth,Game.BoardHeight])+NL+BoardToText(NL),Game.Title,MB_ICONINFORMATION);

                                         if OptimizeGame(GameAsTextSize__,GameAsText__) then begin
                                            if   (GameAsText__<>nil) and (GameAsTextSize__<>0) then begin
                                                 PathToText(Positions.BestPosition,GameAsTextSize__,GameAsText__,IsASolution,Optimizer.Result);
                                                 Result:=Ord(Optimizer.Result);
                                                 end
                                            else Result:=Ord(prInvalidLevel); // the caller didn't provide a buffer for the game: report back that the level (or rather the game) is invalid
                                            end
                                         else begin
                                            if      Optimizer.Result=prGameTooLong then
                                                    Result:=Ord(prGameTooLong)
                                            else if Solver.Terminated then
                                                    Result:=Ord(prTerminatedByUser)
                                            else if Solver.LowerBound>=INFINITY then
                                                    Result:=Ord(prUnsolvable)
                                            else if Solver.TimeMS+Optimizer.TimeMS+Game.InitializationTimeMS>=Solver.SearchLimits.TimeLimitMS then
                                                    Result:=Ord(prTimeout)
                                            else    Result:=Ord(prUnsolved);
                                            end;
                                         end
                                      else Result:=Ord(InitializePluginResult);
                                      end
                                   else Result:=Ord(prFailed);
                              finally GameAsText:='';
                                      YASS.Finalize; // the plugin can and must be finalized, even if 'YASS.Initialize' failed
                              end
                           else begin
                              Result:=Ord(LoadSettingsFromIniFileResult);
                              end;
                 finally YASSdllSettingsForm.Free;
                 end;
          except on E:Exception do begin
                 MessageBox(0,E.Message,TEXT_APPLICATION_TITLE,MB_OK+MB_ICONERROR);
                 Result:=Ord(prFailed);
                 end;
          end
       else
          Result:=Ord(prInvalidLevel)
  else Result:=Ord(prConstraintsViolation);
end;

procedure ShowAbout(WindowHandle__: HWND) stdcall;
begin
//MessageBox(WindowHandle__,TEXT_APPLICATION_TITLE+NL+'Version '+TEXT_APPLICATION_VERSION_NUMBER+NL+TEXT_APPLICATION_COPYRIGHT, TEXT_APPLICATION_TITLE, MB_OK+MB_ICONINFORMATION);
  YASSdllSettings_.ShowAbout(WindowHandle__,False,True,'');
end;

function Solve(Width__, Height__: Cardinal; BoardAsText2D__: PTPCharVector; SolutionAsText__: PChar; SolutionAsTextSize__: Cardinal): Integer; stdcall;
var Col,Row:Integer; Source,Dest:PChar;
    BoardAsText1D:array[0..MAX_BOARD_WIDTH*MAX_BOARD_HEIGHT] of Char;
begin
  if   (Width__ >=3) and (Width__ <=MAX_BOARD_WIDTH ) and
       (Height__>=3) and (Height__<=MAX_BOARD_HEIGHT) then begin

       // convert the 2-dimensional board to a 1-dimensional null-terminated text string
       Dest:=Addr(BoardAsText1D);
       for Row:=0 to Pred(Height__) do begin
           Source:=BoardAsText2D__^[Row];
           for Col:=0 to Pred(Width__) do begin
               if Source^<>NULL_CHAR then begin
                  Dest^:=Source^;
                  Inc(Source); // get ready for the next source character
                  end
               else
                  Dest^:=SPACE; // fill the rest of the row with spaces
               Inc(Dest); // get ready for the next destination character
               end;
           end;
       Dest^:=NULL_CHAR; // add string terminator

       Result:=SolveEx(Width__, Height__, Addr(BoardAsText1D), SolutionAsText__, SolutionAsTextSize__, nil,nil);
       end
  else Result:=Ord(prConstraintsViolation);
end;

function SolveEx(Width__, Height__        : Cardinal;
                 BoardAsText__            : PChar;
                 SolutionAsText__         : PChar;    // null-terminated string
                 SolutionAsTextSize__     : Cardinal; // including null-terminator
                 SokobanStatusPointer__   : PSokobanStatus;
                 SokobanCallBackFunction__: TSokobanCallBackFunction): Integer; stdcall;
var Col,Row:Integer; InitializeErrorText,SolutionAsText:String;
    InitializePluginResult:TPluginResult;
    YASSdllSettingsForm:TYASSdllSettingsForm;
begin
  if   (Width__ >=3) and (Width__ <=MAX_BOARD_WIDTH ) and
       (Height__>=3) and (Height__<=MAX_BOARD_HEIGHT) then
       if StrLen(BoardAsText__)=Width__*Height__ then
          try    YASSdllSettingsForm:=TYASSdllSettingsForm.Create(nil); // loads and validates the settings
                 try     with YASSdllSettingsForm do
                           if LoadSettingsFromIniFileResult=prOK then
                              try
                                   Positions.MemoryByteSize:=SpinEditSolverTransPositionTableSize.Value*1024*1024;
                                   if   CheckBoxSolverSearchTime.Checked then
                                        Solver.SearchLimits.TimeLimitMS:=SpinEditSolverMaxTime.Value*1000
                                   else Solver.SearchLimits.TimeLimitMS:=High(Solver.SearchLimits.TimeLimitMS); {high-value signals unlimited search time}
                                   Solver.SearchLimits.PushCountLimit:=High(Solver.SearchLimits.PushCountLimit);
                                   Solver.SearchLimits.DepthLimit:=MAX_HISTORY_BOX_MOVES;
                                   Optimizer.SearchLimits.PushCountLimit:=Solver.SearchLimits.PushCountLimit;
                                   Optimizer.SearchLimits.DepthLimit:=Solver.SearchLimits.DepthLimit;
                                   Optimizer.SearchLimits.TimeLimitMS:=Solver.SearchLimits.TimeLimitMS;
                                   Solver.BackwardSearchDepthLimit:=Solver.SearchLimits.DepthLimit;
                                   Game.DeadlockSets.AdjacentOpenSquaresLimit:=SpinEditSolverPrecalculatedDeadlocksComplexity.Value;
                                   Game.DeadlockSets.BoxLimitForDynamicSets:=DEFAULT_DEADLOCK_SETS_BOX_LIMIT_FOR_DYNAMIC_SETS;
                                   Game.DeadlockSets.BoxLimitForPrecalculatedSets:=DEFAULT_DEADLOCK_SETS_BOX_LIMIT_FOR_PRECALCULATED_SETS;
                                   Solver.SearchMethod:=SearchMethod;
                                   Solver.Enabled:=True;
                                   Solver.FindPushOptimalSolution:=True;
                                   Optimizer.Enabled:=CheckBoxSmallOptimizations.Checked;
                                   Optimizer.QuickVicinitySearchEnabled:=False;
                                   Solver.ShowBestPosition:=False;
                                   Solver.StopWhenSolved:=CheckBoxStopWhenSolved.Checked;
                                   UserInterface.Prompt:=True;
                                   Solver.ReuseNodesEnabled:=False;
                                   LogFile.Enabled:=CheckBoxSolverLog.Checked;
                                   Solver.PackingOrder.Enabled:=CheckBoxPackingOrderSearch.Checked;
                                   Solver.PackingOrder.BoxCountThreshold:=SpinEditPackingOrderSearchThreshold.Value;
                                   Solver.SokobanCallBackFunction:=SokobanCallBackFunction__;
                                   Solver.SokobanStatusPointer:=SokobanStatusPointer__;
                                   Optimizer.Optimization:=Optimization;

                                   if YASS.Initialize(
                                        Positions.MemoryByteSize,
                                        Solver.SearchLimits.PushCountLimit,
                                        Solver.SearchLimits.DepthLimit,
                                        Solver.BackwardSearchDepthLimit,
                                        Optimizer.SearchLimits.PushCountLimit,
                                        Optimizer.SearchLimits.DepthLimit,
                                        Game.DeadlockSets.AdjacentOpenSquaresLimit,
                                        Game.DeadlockSets.BoxLimitForDynamicSets,
                                        Game.DeadlockSets.BoxLimitForPrecalculatedSets,
                                        Solver.SearchMethod,
                                        Solver.Enabled,
                                        Optimizer.Enabled,
                                        Optimizer.QuickVicinitySearchEnabled,
                                        Solver.ShowBestPosition,//False,
                                        Solver.StopWhenSolved,
                                        UserInterface.Prompt,
                                        Solver.ReuseNodesEnabled,
                                        LogFile.Enabled,
                                        Solver.PackingOrder.Enabled,
                                        Solver.PackingOrder.BoxCountThreshold,
                                        Solver.SearchLimits.TimeLimitMS,
                                        Optimizer.SearchLimits.TimeLimitMS,
                                        Optimizer.BoxPermutationsSearchTimeLimitMS,
                                        Optimizer.MethodEnabled,
                                        Optimizer.MethodOrder,
                                        Optimizer.Optimization,
                                        Optimizer.VicinitySettings,
                                        Solver.SokobanCallBackFunction,
                                        Solver.SokobanStatusPointer) then begin

                                      InitializeBoard(Width__,Height__,True);
                                      for Row:=1 to Game.BoardHeight do
                                          for Col:=1 to Game.BoardWidth do begin
                                              Game.Board[ColRowToSquare(Col,Row)]:=Legend.CharToItem[BoardAsText__^];
                                              Inc(BoardAsText__); // get ready for next character
                                              end;
                                      if LogFile.Enabled then CreateLogFile(ChangeFileExt(IniFileName,LOG_FILE_EXT));

                                      if InitializeGame(InitializePluginResult,InitializeErrorText) then begin
                                         //MessageBox(Format('Width: %d, Height: %d',[Game.BoardWidth,Game.BoardHeight])+NL+BoardToText(NL),Game.Title,MB_ICONINFORMATION);

                                         if Search then begin
                                            if (SolutionAsText__<>nil) and (SolutionAsTextSize__<>0) then begin
                                               SolutionAsText:=GameHistoryMovesAsText;
                                               if   SolutionAsText<>'' then
                                                    StrLCopy(SolutionAsText__,
                                                             PChar(SolutionAsText),
                                                             Min(Pred(Integer(SolutionAsTextSize__)),Length(SolutionAsText))) // 'Pred()': reserve one character for a NULL terminator
                                               else SolutionAsText__^:=NULL_CHAR;
                                               if   Length(SolutionAsText)<Integer(SolutionAsTextSize__) then
                                                    Result:=Ord(prOK)
                                               else Result:=Ord(prGameTooLong);
                                               //MessageBox(0,'Solution: '+IntToStr(Length(SolutionAsText))+SLASH+IntToStr(Game.History.Count),TEXT_APPLICATION_TITLE,MB_OK+MB_ICONINFORMATION);
                                               end
                                            else
                                              Result:=Ord(prOK); // the caller didn't provide a buffer for the solution: report back that the level was solved
                                            end
                                         else begin
                                            //MessageBox(0,'No solution: '+IntToStr(Solver.PushCount)+SLASH+IntToStr(Positions.OpenPositions.MaxValue)+SLASH+IntToStr(Solver.LimitExceededPushCount)+SLASH+IntToStr(Solver.LowerBound),TEXT_APPLICATION_TITLE+' (pushcount, maxvalue, exceededcount, lowerbound)',MB_OK+MB_ICONINFORMATION);
                                            if      Solver.Terminated then
                                                    Result:=Ord(prTerminatedByUser)
                                            else if Solver.LowerBound>=INFINITY then
                                                    Result:=Ord(prUnsolvable)
                                            else if (Solver.PushCount>0) and
                                                    (Positions.OpenPositions.MaxValue>=Low(Positions.OpenPositions.Buckets)-1) and
                                                    // all stored positions were explored without finding a solution
                                                    // ('.MaxValue' = Low(...) - 2 if the search was terminated before all positions were fully expanded)
                                                    (Solver.LimitExceededPushCount=0) and
                                                    // no positions were dropped because of a depth limit or a push count limit
                                                    (Positions.SearchStatistics.RoomPositionsCount=0)
                                                    // no room pruning has been performed, i.e., all boxes have been taken into account during node expansion
                                                    then
                                                    Result:=Ord(prUnsolvable)
                                            else if Solver.TimeMS+Game.InitializationTimeMS>=Solver.SearchLimits.TimeLimitMS then
                                                    Result:=Ord(prTimeout)
                                            else    Result:=Ord(prUnsolved);
                                            end;
                                         end
                                      else Result:=Ord(InitializePluginResult);
                                      end
                                   else Result:=Ord(prFailed);
                              finally SolutionAsText:='';
                                      YASS.Finalize; // the plugin can and must be finalized, even if 'YASS.Initialize' failed
                              end
                           else begin
                              Result:=Ord(LoadSettingsFromIniFileResult);
                              end;
                 finally YASSdllSettingsForm.Free;
                 end;
          except on E:Exception do begin
                 MessageBox(0,E.Message,TEXT_APPLICATION_TITLE,MB_OK+MB_ICONERROR);
                 Result:=Ord(prFailed);
                 end;
          end
       else
          Result:=Ord(prInvalidLevel)
  else Result:=Ord(prConstraintsViolation);
end;

function Terminate: Integer; stdcall;
begin
  // the function 'Terminate()' returns 'Ok' or 'Fail' depending on whether
  // the plugin accepts the terminate command (YASS always does that)

  // note that calling 'Terminate()' DOES NOT stop the plugin, it ASKS the plugin to stop;
  // a solver plugin typically unwinds its recursive calls and returns the search result;
  // a solver may have found a solution or proved the level unsolvable before
  // it returns, so it does not necessarily return 'Fail' after a call to 'Terminate()';

  YASS.Terminate;
  Result := Ord(prOK);
end;

exports
  {$WARNINGS OFF}  {Warning: Symbol 'INDEX' is specific to a platform}
    Configure Index 1,
    GetConstraints Index 2,
    GetPluginName Index 3,
    GetSupportedOptimizations Index 4, // deprecated; in fact, it was never publically documented so only a few early versions of the YASS plugin used it; use 'IsSupportedOptimization' instead
    IsSupportedOptimization Index 5,
    Optimize Index 6,
    ShowAbout Index 7,
    Solve Index 8,
    SolveEx Index 9,
    Terminate Index 10;
  {$WARNINGS ON}
begin
end.
