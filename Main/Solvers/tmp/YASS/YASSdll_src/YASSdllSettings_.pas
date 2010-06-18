unit YASSdllSettings_;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, Spin, ComCtrls,
  {$WARNINGS OFF} {Warning: Unit 'FileCtrl' is specific to a platform}
    FileCtrl,
  {$WARNINGS ON}
  YASS;

type
  TYASSdllSettingsForm = class(TForm)
    PanelTop: TPanel;
    StatusBar1: TStatusBar;
    ButtonDefaultSettings: TButton;
    ButtonAbout: TButton;
    ButtonOK: TButton;
    ButtonCancel: TButton;
    PageControl1: TPageControl;
    TabSheetSolver: TTabSheet;
    GroupBox1: TGroupBox;
    RadioButtonBackwardSearch: TRadioButton;
    RadioButtonForwardSearch: TRadioButton;
    RadioButtonPerimeterSearch: TRadioButton;
    GroupBox2: TGroupBox;
    Label2: TLabel;
    Label3: TLabel;
    SpinEditPackingOrderSearchThreshold: TSpinEdit;
    CheckBoxPackingOrderSearch: TCheckBox;
    GroupBox3: TGroupBox;
    Label1: TLabel;
    Label9: TLabel;
    SpinEditSolverTranspositionTableSize: TSpinEdit;
    SpinEditSolverPrecalculatedDeadlocksComplexity: TSpinEdit;
    CheckBoxSolverSearchTime: TCheckBox;
    SpinEditSolverMaxTime: TSpinEdit;
    GroupBox4: TGroupBox;
    CheckBoxSolverLog: TCheckBox;
    CheckBoxSmallOptimizations: TCheckBox;
    TabSheetOptimizer: TTabSheet;
    GroupBox5: TGroupBox;
    RadioButtonOptimizePushes: TRadioButton;
    GroupBox7: TGroupBox;
    Label4: TLabel;
    Label5: TLabel;
    SpinEditOptimizerTranspositionTableSize: TSpinEdit;
    SpinEditOptimizerPrecalculatedDeadlocksComplexity: TSpinEdit;
    CheckBoxOptimizerSearchTime: TCheckBox;
    SpinEditOptimizerMaxTime: TSpinEdit;
    GroupBox6: TGroupBox;
    CheckBoxOptimizerLog: TCheckBox;
    RadioButtonOptimizeMoves: TRadioButton;
    Memo1: TMemo;
    RadioButtonOptimizePushesOnly: TRadioButton;
    GroupBox8: TGroupBox;
    SpinEditVicinityBox2: TSpinEdit;
    SpinEditVicinityBox1: TSpinEdit;
    SpinEditVicinityBox3: TSpinEdit;
    GroupBox9: TGroupBox;
    SpinEditGlobalOptimization: TSpinEdit;
    SpinEditBoxPermutationsOptimization: TSpinEdit;
    SpinEditVicinityOptimization: TSpinEdit;
    CheckBoxGlobalOptimization: TCheckBox;
    CheckBoxBoxPermutationsOptimization: TCheckBox;
    CheckBoxVicinityOptimization: TCheckBox;
    Bevel1: TBevel;
    CheckBoxFallbackStrategyOptimization: TCheckBox;
    CheckBoxVicinityBox1: TCheckBox;
    CheckBoxVicinityBox2: TCheckBox;
    CheckBoxVicinityBox3: TCheckBox;
    CheckBoxVicinityBox4: TCheckBox;
    SpinEditVicinityBox4: TSpinEdit;
    Bevel2: TBevel;
    CheckBoxQuickVicinitySearch: TCheckBox;
    CheckBoxREarrangementOptimization: TCheckBox;
    SpinEditRearrangementOptimization: TSpinEdit;
    CheckBoxStopWhenSolved: TCheckBox;
    RadioButtonOptimizeBoxLinesMoves: TRadioButton;
    RadioButtonOptimizeBoxLinesPushes: TRadioButton;
    procedure RadioButtonSearchStrategyClick(Sender: TObject);
    procedure CheckBoxPackingOrderSearchEnabledClick(Sender: TObject);
    procedure SpinEditPackingOrderSearchThresholdChange(
      Sender: TObject);
    procedure SpinEditSolverTranspositionTableSizeChange(Sender: TObject);
    procedure SpinEditSolverMaxTimeChange(Sender: TObject);
    procedure SpinEditSolverMaxPushesChange(Sender: TObject);
    procedure SpinEditSolverMaxDepthChange(Sender: TObject);
    procedure SpinEditSolverPrecalculatedDeadlocksComplexityChange(
      Sender: TObject);
    procedure ButtonOKClick(Sender: TObject);
    procedure ButtonCancelClick(Sender: TObject);
    procedure ButtonAboutClick(Sender: TObject);
    procedure ButtonDefaultSettingsClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure ControlMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure FormActivate(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure CheckBoxClick(Sender: TObject);
    procedure FormKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure FormMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure RadioButtonOptimizeMovesOrPushesClick(Sender: TObject);
    procedure PageControl1Change(Sender: TObject);
    procedure SpinEditOptimizationMethodOrderChange(Sender: TObject);
    procedure CheckBoxEnableDisableOptimizationMethodClick(Sender: TObject);
    procedure ButtonDefaultSettingsMouseUp(Sender: TObject;
      Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
    procedure SpinEditVicinityBoxChange(Sender: TObject);
    procedure CheckBoxVicinityBoxClick(Sender: TObject);
    procedure CheckBoxVicinityBoxExit(Sender: TObject);
    procedure ControlEnter(Sender: TObject);
    procedure CheckBoxSolverSearchTimeClick(Sender: TObject);
    procedure CheckBoxSolverSearchTimeExit(Sender: TObject);
    procedure CheckBoxOptimizerSearchTimeClick(Sender: TObject);
    procedure CheckBoxOptimizerSearchTimeExit(Sender: TObject);
    procedure SpinEditExit(Sender: TObject);
  protected
    { protected declarations}
    function  GetOptimization:TOptimization;
    function  GetSearchMethod:TSearchMethod;
    procedure SetOptimization(Optimization__:TOptimization);
    procedure SetSearchMethod(SearchMethod__:TSearchMethod);
  private
    { Private declarations }
    FirstTabSheet:TTabSheet;
    HintControl:TObject;
    OldQuickVicinitySearchChecked:Boolean;
    Modified:Boolean;
    OptimizationMethodOrder:TOptimizationMethodOrder;
    OptimizationMethodSpinEdits:array[TOptimizationMethod] of TSpinEdit;

    procedure DisplayHint(Sender: TObject);
    procedure EnableDisableOptimizationFallbackStrategy;
    function  KeyWithSuffix(const Key__,Suffix__:String):String;
    function  LoadSettingsFromIniFile: Boolean;
    function  SaveSettingsToIniFile(ReallySaveSettings__:Boolean):Boolean;
    procedure SetDefaultOptimizerMethodSettings;
    procedure SetDefaultValuesForOptimizer;
    procedure SetDefaultValuesForSolver;
    procedure ShowStatus;
    procedure SpinEditChange(Sender: TObject);

  public
    { Public declarations }
    LoadSettingsFromIniFileResult:TPluginResult;
    function  IniFileName:String;
    property  Optimization:TOptimization read GetOptimization write SetOptimization;
    property  SearchMethod:TSearchMethod read GetSearchMethod write SetSearchMethod;
  end;

var
  YASSdllSettingsForm: TYASSdllSettingsForm;

function  MessageBox(WindowHandle__:HWND; const Text__,Caption__:String; Flags__:Integer):Integer;
procedure ShowAbout(WindowHandle__:HWND; ShowSettingsCopyright__,ShowSolverHint__:Boolean; const Caption__:String);

implementation

{$R *.DFM}

uses IniFiles;

const // don't localize
  APPLICATION_DATA_SUB_FOLDER              = 'Sokoban\Sokoban YASC\Plugins'; // settings are stored in an ini-file in this subfolder of Windows' "My Documents" virtual folder
  CSIDL_PERSONAL	                   = 5; // folder identifier for Windows' "My Documents" virtual folder
  DEFAULT_OPTIMIZER_TIME_LIMIT_ENABLED     = False;
  DEFAULT_WINDOWS_98_COLOR_BTN_FACE        = clSilver;
  DEFAULT_WINDOWS_VISTA_COLOR_BTN_FACE     = TColor($F0F0F0); //  BGR, not RGB;
  DEFAULT_WINDOWS_XP_COLOR_BTN_FACE        = TColor($D8E9EC); //  BGR, not RGB;
  FILE_NAME_PATH_DELIMITER                 = BACKSLASH;
  INI_FILE_EXT                             = '.ini';
  KEY_DATE                                 = 'Date';
  KEY_ENABLED                              = 'Enabled';
  KEY_LOG                                  = 'Log';
  KEY_OPTIMIZATION                         = 'Optimization';
  KEY_OPTIMIZATION_METHOD_REARRANGEMENT    = 'Rearrangement optimization';
  KEY_OPTIMIZATION_METHOD_BOX_PERMUTATIONS = 'Box permutations optimization';
  KEY_OPTIMIZATION_METHOD_FALLBACK_STRATEGY= 'Fallback strategy optimization';
  KEY_OPTIMIZATION_METHOD_GLOBAL           = 'Global optimization';
  KEY_OPTIMIZATION_METHOD_SETTINGS_EXPIRE_AFTER_24_HOURS
                                           = 'Optimizer method settings expire after 24 hours';
  KEY_OPTIMIZATION_METHOD_VICINITY         = 'Vicinity optimization';
  KEY_OPTIMIZE_MOVES                       = 'Optimize moves';                  // deprecated key; 'Optimization' replaced the boolean moves yes/no in YASS version 2.49
  KEY_PACKING_ORDER                        = 'Packing order';
  KEY_PACKING_ORDER_THRESHOLD_BOXES        = 'Packing order threshold, boxes';
  KEY_PRECALCULATED_DEADLOCKS_COMPLEXITY   = 'Precalculated deadlocks complexity';
  KEY_OPTIMIZER_BOX_PERMUTATIONS_SEARCH_TIME_LIMIT_SECONDS   = 'Search-by-slice time limit, seconds';
  KEY_QUICK_VICINITY_SEARCH                = 'Quick vicinity search';
  KEY_SEARCH_DEPTH_LIMIT                   = 'Search depth limit';
  KEY_SEARCH_DEPTH_LIMIT_PUSHES            = 'Search depth limit, pushes';
  KEY_SEARCH_METHOD                        = 'Search method';
  KEY_SEARCH_PUSHES_LIMIT                  = 'Search pushes limit';
  KEY_SEARCH_PUSHES_LIMIT_MILLION          = 'Search pushes limit, million';
  KEY_SEARCH_TIME_LIMIT                    = 'Search time limit';
  KEY_SEARCH_TIME_LIMIT_SECONDS            = 'Search time limit, seconds';
  KEY_SMALL_OPTIMIZATIONS                  = 'Small optimizations';
  KEY_STOP_WHEN_SOLVED                     = 'Stop when solved';
  KEY_TAB_SHEET                            = 'TabSheet';
  KEY_TIME                                 = 'Time';
  KEY_TRANSPOSITION_TABLE_SIZE_MIB         = 'Transposition table size, MiB';
  KEY_VICINITY_BOX                         = 'Vicinity search box';
  KEY_VICINITY_BOX_ENABLED                 = 'Vicinity search box enabled';
  KEY_VICINITY_BOXES                       = 'Vicinity search boxes';
  LOG_FILE_EXT                             = '.log';
  MAX_SPIN_EDIT_TEXT_LENGTH                = 11; // length(min(integer)) 

  TEXT_HINT_DEFAULT_SETTINGS               = '| Set default values for all options';
  TEXT_HINT_DEFAULT_SETTINGS_OPTIMIZER     = '| Left-click: Set default values for all options. Right-click: Methods and vicinity options only.';
  TEXT_OK_OR_MODIFIED                      : array[Boolean] of String
                                           = ('OK','Modified');
  TEXT_OPTIMIZATION_TYPES                  : array[TOptimization] of String
                                           = ('Moves, pushes','Pushes, moves', 'Pushes only','Box lines, moves','Box lines, pushes');
  TEXT_SAVE_CHANGES                        = 'The settings have changed.'+NL+
                                             'Do you want to save the changes?';
  TEXT_SETTINGS                            = 'Settings';

  TEXT_SETTINGS_FORM_COPYRIGHT             = 'Copyright (c) 2010 by Brian Damgaard';


function  IsWindowsDefaultColorBtnFace(Color:TColor):Boolean;
begin
  Result:=(ColorToRgb(Color)=ColorToRGB(DEFAULT_WINDOWS_98_COLOR_BTN_FACE)) or
          (ColorToRgb(Color)=ColorToRGB(DEFAULT_WINDOWS_VISTA_COLOR_BTN_FACE)) or
          (ColorToRgb(Color)=ColorToRGB(DEFAULT_WINDOWS_XP_COLOR_BTN_FACE));
end;

function  ApplicationHiglightedTextColor:TColor;
begin // returns a color for highlighted text on normal background (clBtnFace)
  if   IsWindowsDefaultColorBtnFace(clBtnFace) then
       Result:=clBlue
  else Result:=clBtnText; // "clHighlight": current background color of selected text ("clHighlight" isn't guaranteed to have enough contrast to the default background color, hence, settle for the safe choice, the normal "clBtnText")
end;

function  CalculateVicinityBoxCount(const VicinitySettings__:TVicinitySettings):Integer;
var i:Integer;
begin // returns the number of consequtive non-zero entries in the vicinity settings (starting from the top because the items are organized in reverse order with the element 0 as a 0-value sentinel)
  Result:=0;
  for i:=High(VicinitySettings__) downto Succ(Low(VicinitySettings__)) do
      if   VicinitySettings__[i]<>0 then Inc(Result)
      else exit;
end;

function  GetFolderPath(Folder__:Integer):String;
const
  SHGFP_TYPE_CURRENT = 0;
  SHGFP_TYPE_DEFAULT = 1;
var
  DLLHandle          :THandle;
  PathCharBuffer     :array[0..MAX_PATH] of Char;
  SHGetFolderPath    :function(hwndOwner:HWND; nFolder:Integer; hToken:THANDLE; Flags:Cardinal; pszPath:PChar):HResult; stdcall;
begin
  Result:='';
  try    DLLHandle:=LoadLibrary('shell32.dll');
         if DLLHandle<>0 then
            try     SHGetFolderPath:=GetProcAddress(DLLHandle,'SHGetFolderPathA');
                    if Assigned(SHGetFolderPath) then begin
                       PathCharBuffer[Low(PathCharBuffer)]:=NULL_CHAR;
                       SHGetFolderPath(0,
                                       Folder__,
                                       0,
                                       SHGFP_TYPE_CURRENT,
                                       PathCharBuffer);
                       if StrLen(PathCharBuffer)<>0 then begin // 'True': the buffer is non-empty if 'SHGetFolderPath' succeeds
                          Result:=PathCharBuffer;
                          end;
                       end;
            finally FreeLibrary(DLLHandle);
            end;
  except on E:Exception do Result:='';
  end;
end;

function  IndexOfOptimizationMethod(Method__:TOptimizationMethod; const MethodOrder__:TOptimizationMethodOrder):Integer;
begin
  for Result:=Low(MethodOrder__) to High(MethodOrder__) do
      if Method__=MethodOrder__[Result] then exit; // 'exit': quick-and-dirty exit when a matching method has been found in the table
  Result:=-1; // '-1': the method was not found in the table
end;

function MessageBox(WindowHandle__:HWND; const Text__,Caption__:String; Flags__:Integer):Integer;
begin
  if Flags__=0 then Flags__:=MB_OK+MB_ICONINFORMATION;
  Result:=Windows.MessageBox(WindowHandle__,PChar(Text__),PChar(Caption__),Flags__);
end;

procedure ShowAbout(WindowHandle__:HWND; ShowSettingsCopyright__,ShowSolverHint__:Boolean; const Caption__:String);
var s,Caption,Text:String;
begin
  if   ShowSettingsCopyright__ then
       s:='"'+YASS.TEXT_APPLICATION_TITLE+' - Settings" programming and design:'+NL+
          TEXT_SETTINGS_FORM_COPYRIGHT+
          NL+NL
  else s:='';

  Text :=  YASS.TEXT_APPLICATION_TITLE+NL+
           YASS.TEXT_VERSION+SPACE+YASS.TEXT_APPLICATION_VERSION_NUMBER+NL+
           YASS.TEXT_APPLICATION_COPYRIGHT+
           NL+NL+
           s+
           'Limits:'+NL+
           '  Board width: '+IntToStr(MAX_BOARD_WIDTH)+NL+
           '  Board height: '+IntToStr(MAX_BOARD_HEIGHT)+NL+
           '  Number of boxes: '+IntToStr(MAX_BOX_COUNT)+NL+
           '  Number of moves: '+IntToStr(High(POptimizerPosition(Positions.BestPosition)^.MoveCount))+NL+
           '  Solver search depth: '+IntToStr(MAX_HISTORY_BOX_MOVES)+' pushes';
  if ShowSolverHint__ then
     Text:=Text+
           NL+NL+
           'Solver:'+NL+
           'Packing order search must be enabled in order to solve medium-sized '+
           'goal-room themed levels. However, it may be necessary '+
           'to disable it for solving small goal-room themed levels. Additionally, '+
           'disabling the packing order search increases the solver''s chances of '+
           'finding push-optimal solutions for small levels.'+NL+NL+
           'Optimizer method settings and vicinity search settings:'+NL+
           'Customized settings for these options expire after 24 hours. The '+
           'rationale is that these settings are typically only good for the '+
           'level they were designed for, and unsuitable settings can '+
           'significantly impair the quality of the optimizer.';

  if   Caption__<>'' then Caption:=Caption__
  else Caption:=YASS.TEXT_APPLICATION_TITLE_LONG;

  MessageBox(WindowHandle__,Text,Caption,MB_OK+MB_ICONINFORMATION);
end;

function  StrEqual(const S1__,S2__:String):Boolean;
begin // Compares the 2 strings without case sensitivity
  Result:=AnsiCompareText(S1__,S2__)=0;
end;

function  StrWithoutTrailingPathDelimiter(const Str__:String):String; {throws EOutOfMemory}
begin
  Result:=Str__;
  if (Result<>'') and (Result[Length(Result)]=FILE_NAME_PATH_DELIMITER) then
     Delete(Result,Length(Result),1);
end;

function  TYASSdllSettingsForm.GetOptimization:TOptimization;
begin
  if                  RadioButtonOptimizeMoves        .Checked then Result:=opMovesPushes
  else if             RadioButtonOptimizePushes       .Checked then Result:=opPushesMoves
       else if        RadioButtonOptimizePushesOnly   .Checked then Result:=opPushesOnly
            else if   RadioButtonOptimizeBoxLinesMoves.Checked then Result:=opBoxLinesMoves
                 else Result:=opBoxLinesPushes;
end;

function  TYASSdllSettingsForm.GetSearchMethod:TSearchMethod;
begin
  if        RadioButtonBackwardSearch.Checked then Result:=smBackward
  else if   RadioButtonForwardSearch .Checked then Result:=smForward
       else Result:=smPerimeter;
end;

procedure TYASSdllSettingsForm.SetOptimization(Optimization__:TOptimization);
begin
  if        Optimization__=opMovesPushes then
            RadioButtonOptimizeMovesOrPushesClick(RadioButtonOptimizeMoves)
  else if   Optimization__=opPushesMoves then
            RadioButtonOptimizeMovesOrPushesClick(RadioButtonOptimizePushes)
  else if   Optimization__=opPushesOnly then
            RadioButtonOptimizeMovesOrPushesClick(RadioButtonOptimizePushesOnly)
  else if   Optimization__=opBoxLinesMoves then
            RadioButtonOptimizeMovesOrPushesClick(RadioButtonOptimizeBoxLinesMoves)
       else RadioButtonOptimizeMovesOrPushesClick(RadioButtonOptimizeBoxLinesPushes);
end;

procedure TYASSdllSettingsForm.SetSearchMethod(SearchMethod__:TSearchMethod);
begin
  if        SearchMethod__=smBackward then RadioButtonSearchStrategyClick(RadioButtonBackwardSearch)
  else if   SearchMethod__=smForward  then RadioButtonSearchStrategyClick(RadioButtonForwardSearch)
       else RadioButtonSearchStrategyClick(RadioButtonPerimeterSearch);
end;

function TYASSdllSettingsForm.IniFileName:String;
var Length:Integer; ModuleFileName:String;
begin
  // store settings in the user's "My Documents" virtual folder
  Result:=StrWithoutTrailingPathDelimiter(GetFolderPath(CSIDL_PERSONAL));
  if (Result<>'') and DirectoryExists(Result) then begin
     Result:=Result+FILE_NAME_PATH_DELIMITER+APPLICATION_DATA_SUB_FOLDER;
     if not DirectoryExists(Result) then begin
        ForceDirectories(Result);
        if not DirectoryExists(Result) then Result:='';
        end;
     if Result<>'' then Result:=Result+FILE_NAME_PATH_DELIMITER+ChangeFileExt(TEXT_APPLICATION_TITLE,INI_FILE_EXT);
     end
  else Result:='';
  if Result='' then
     // store settings in the program's installation directory
     begin
     ModuleFileName:='';
     Length:=Min(Max(MAX_PATH,32767),65535)+SizeOf(Char); // 'Windows.MAX_PATH' seems rather small; better try to be prepared for longer paths or unlimited paths in future Windows versions
     SetLength(ModuleFileName,Length);
     Length:=GetModuleFileName(HInstance, Addr(ModuleFileName[1]), Length-SizeOf(Char));
     if   Length<>0 then
          Result:=ExtractFilePath(Copy(ModuleFileName,1,Length))+ChangeFileExt(TEXT_APPLICATION_TITLE,INI_FILE_EXT)
     else Result:=ExtractFilePath(ParamStr(0)                  )+ChangeFileExt(TEXT_APPLICATION_TITLE,INI_FILE_EXT);
     end;
end;

function TYASSdllSettingsForm.LoadSettingsFromIniFile:Boolean;
var op:TOptimization; sm:TSearchMethod; OptimizerMethodSettingsExpired:Boolean;
    s,Section:String; DateTime1,DateTime2:TDateTime; IniFile:TIniFile;

  procedure LoadCheckBoxValue(const Section__,Key__:String; CheckBox__:TCheckBox);
  var b:Boolean; s:String;
  begin
    s:=IniFile.ReadString (Section__,Key__,TEXT_NO_YES[CheckBox__.Checked]);
    for b:=Low(TEXT_NO_YES) to High(TEXT_NO_YES) do
        if StrEqual(s,TEXT_NO_YES[b]) then CheckBox__.Checked:=b;
  end;

  procedure LoadSpinEditValue(const Section__,Key__:String; SpinEdit__:TSpinEdit);
  var Value:Integer;
  begin
    Value:=IniFile.ReadInteger(Section__,Key__,SpinEdit__.Value);
    if   (Value>=SpinEdit__.MinValue) and (Value<=SpinEdit__.MaxValue) then
         SpinEdit__.Value:=Value
    else LoadSettingsFromIniFileResult:=prInvalidSettings;
  end;

  procedure LoadRadioButtonValue(const Section__,Key__:String; RadioButton__:TRadioButton);
  var b:Boolean; s:String;
  begin
    s:=IniFile.ReadString (Section__,Key__,TEXT_NO_YES[RadioButton__.Checked]);
    for b:=Low(TEXT_NO_YES) to High(TEXT_NO_YES) do
        if StrEqual(s,TEXT_NO_YES[b]) then RadioButton__.Checked:=b;
  end;

begin // LoadSettingsFromIniFile
  LoadSettingsFromIniFileResult:=prFailed;
  OptimizerMethodSettingsExpired:=False;
  try
          IniFile:=TIniFile.Create(IniFileName);
          try     LoadSettingsFromIniFileResult:=prOK;

                  s:=IniFile.ReadString(TEXT_APPLICATION_TITLE,KEY_TAB_SHEET,'');
                  if s<>'' then begin
                     if StrEqual(s,Trim(TabSheetOptimizer.Caption)) then
                        PageControl1.ActivePage:=TabSheetOptimizer;
                     //IniFile.WriteString(TEXT_APPLICATION_TITLE,KEY_TAB_SHEET,'');
                     //IniFile.DeleteKey(TEXT_APPLICATION_TITLE,KEY_TAB_SHEET);
                     end;

                  Section:=Trim(TabSheetSolver.Caption);
                  if   not IniFile.SectionExists(Section) then
                       // early versions of the program - with only a solver and
                       // no optimizer - stored the solver settings in a section
                       // named TEXT_APPLICATION_TITLE
                       Section:=TEXT_APPLICATION_TITLE;

                  s:=IniFile.ReadString (Section,KEY_SEARCH_METHOD,TEXT_SEARCH_METHOD[SearchMethod]);
                  for sm:=Low(TEXT_SEARCH_METHOD) to High(TEXT_SEARCH_METHOD) do
                      if StrEqual(s,TEXT_SEARCH_METHOD[sm]) then SearchMethod:=sm;
                  LoadCheckBoxValue     (Section,KEY_PACKING_ORDER,CheckBoxPackingOrderSearch); //
                  LoadSpinEditValue     (Section,KEY_PACKING_ORDER_THRESHOLD_BOXES,SpinEditPackingOrderSearchThreshold);
                  LoadSpinEditValue     (Section,KEY_TRANSPOSITION_TABLE_SIZE_MIB,SpinEditSolverTranspositionTableSize);
                  LoadCheckBoxValue     (Section,KEY_SEARCH_TIME_LIMIT,CheckBoxSolverSearchTime);
                  LoadSpinEditValue     (Section,KEY_SEARCH_TIME_LIMIT_SECONDS,SpinEditSolverMaxTime);
                  LoadSpinEditValue     (Section,KEY_PRECALCULATED_DEADLOCKS_COMPLEXITY,SpinEditSolverPrecalculatedDeadlocksComplexity);
                  LoadCheckBoxValue     (Section,KEY_SMALL_OPTIMIZATIONS,CheckBoxSmallOptimizations);
                  LoadCheckBoxValue     (Section,KEY_STOP_WHEN_SOLVED,CheckBoxStopWhenSolved);
                  LoadCheckBoxValue     (Section,KEY_LOG,CheckBoxSolverLog);

                  Section:=Trim(TabSheetOptimizer.Caption);
                  LoadRadioButtonValue  (Section,KEY_OPTIMIZE_MOVES,RadioButtonOptimizeMoves); // backward compatibilty only; YASS 2.49 replaced 'Optimize moves yes/no' with the ternary 'Optimization'
                  s:=IniFile.ReadString (Section,KEY_OPTIMIZATION,TEXT_OPTIMIZATION_TYPES[Optimization]);
                  for op:=Low(TEXT_OPTIMIZATION_TYPES) to High(TEXT_OPTIMIZATION_TYPES) do
                      if StrEqual(s,TEXT_OPTIMIZATION_TYPES[op]) then Optimization:=op;
                  LoadSpinEditValue     (Section,KEY_TRANSPOSITION_TABLE_SIZE_MIB,SpinEditOptimizerTranspositionTableSize);
                  LoadCheckBoxValue     (Section,KEY_SEARCH_TIME_LIMIT,CheckBoxOptimizerSearchTime);
                  LoadSpinEditValue     (Section,KEY_SEARCH_TIME_LIMIT_SECONDS,SpinEditOptimizerMaxTime);
                  LoadSpinEditValue     (Section,KEY_PRECALCULATED_DEADLOCKS_COMPLEXITY,SpinEditOptimizerPrecalculatedDeadlocksComplexity);
                  LoadCheckBoxValue     (Section,KEY_LOG,CheckBoxOptimizerLog);
                  LoadCheckBoxValue     (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_REARRANGEMENT,KEY_ENABLED),CheckBoxRearrangementOptimization);
                  LoadCheckBoxValue     (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_GLOBAL,KEY_ENABLED),CheckBoxGlobalOptimization);
                  LoadCheckBoxValue     (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_BOX_PERMUTATIONS,KEY_ENABLED),CheckBoxBoxPermutationsOptimization);
                  LoadCheckBoxValue     (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_VICINITY,KEY_ENABLED),CheckBoxVicinityOptimization);
                  LoadCheckBoxValue     (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_FALLBACK_STRATEGY,KEY_ENABLED),CheckBoxFallbackStrategyOptimization);
                  LoadSpinEditValue     (Section,KEY_OPTIMIZATION_METHOD_REARRANGEMENT,SpinEditRearrangementOptimization);
                  LoadSpinEditValue     (Section,KEY_OPTIMIZATION_METHOD_GLOBAL,SpinEditGlobalOptimization);
                  LoadSpinEditValue     (Section,KEY_OPTIMIZATION_METHOD_BOX_PERMUTATIONS,SpinEditBoxPermutationsOptimization);
                  LoadSpinEditValue     (Section,KEY_OPTIMIZATION_METHOD_VICINITY,SpinEditVicinityOptimization);
                  LoadSpinEditValue     (Section,KEY_VICINITY_BOX+' 1',SpinEditVicinityBox1); {the order is important for box 1..4 settings and quick-search: first spinedits 1..4, then checkboxes 1..4, and finally the checkbox for quick-search}
                  LoadSpinEditValue     (Section,KEY_VICINITY_BOX+' 2',SpinEditVicinityBox2);
                  LoadSpinEditValue     (Section,KEY_VICINITY_BOX+' 3',SpinEditVicinityBox3);
                  LoadSpinEditValue     (Section,KEY_VICINITY_BOX+' 4',SpinEditVicinityBox4);
                  LoadCheckBoxValue     (Section,KEY_VICINITY_BOX_ENABLED+' 1',CheckBoxVicinityBox1);
                  LoadCheckBoxValue     (Section,KEY_VICINITY_BOX_ENABLED+' 2',CheckBoxVicinityBox2);
                  LoadCheckBoxValue     (Section,KEY_VICINITY_BOX_ENABLED+' 3',CheckBoxVicinityBox3);
                  LoadCheckBoxValue     (Section,KEY_VICINITY_BOX_ENABLED+' 4',CheckBoxVicinityBox4);
                  LoadCheckBoxValue     (Section,KEY_QUICK_VICINITY_SEARCH,CheckBoxQuickVicinitySearch);

                  // backdoor to set the time limit for the optimizer's
                  // search-by-slice function via the ini-file;
                  // this is for experimental purposes only, and
                  // the option is not a part of the graphical user interface
                  Optimizer.BoxPermutationsSearchTimeLimitMS:=Max(0,1000*IniFile.ReadInteger(Section,KEY_OPTIMIZER_BOX_PERMUTATIONS_SEARCH_TIME_LIMIT_SECONDS,DEFAULT_OPTIMIZER_BOX_PERMUTATIONS_SEARCH_TIME_LIMIT_MS div 1000));

                  if not StrEqual(IniFile.ReadString(Section,KEY_OPTIMIZATION_METHOD_SETTINGS_EXPIRE_AFTER_24_HOURS,''),TEXT_NO_YES[False]) then // 'True': reset the optimizer method settings after 24 hours; the key is deliberately undocumented, so the normal user cannot prevent this from happening
                     // optimizer method settings expire after 24 hours;
                     // the rationale is that these settings are so crucial for
                     // the efficiency of the optimizer that the user cannot be
                     // trusted to maintain them;
                     // customized settings which worked well for one level
                     // are rarely applicable to other levels, so if the
                     // user forgets to reset the settings, the outcome will
                     // typically be inferior to what the optimizer could have
                     // produced with default settings;
                     try    Section:=TEXT_SETTINGS;
                            DateTime1:=Now;
                            s:=IniFile.ReadString(Section,KEY_DATE,DateToStr(DateTime1));
                            DateTime2:=StrToDate(s);
                            s:=IniFile.ReadString(Section,KEY_TIME,TimeToStr(DateTime1));
                            DateTime2:=DateTime2+StrToTime(s);
                            OptimizerMethodSettingsExpired:=DateTime1-DateTime2>=1.0;    // '>=1.0': 1 day
                     except on E:EConvertError do // ignore any date/time conversion errors from 'StrToDate' and 'StrToTime'
                               OptimizerMethodSettingsExpired:=True; // reset to default settings in case a conversion error occured; this is safer than letting an unwary user continue with settings that were designed for one particular level only
                     end;
                  Result:=True;
          finally IniFile.Free;
                  EnableDisableOptimizationFallbackStrategy;
                  CheckBoxSolverSearchTimeExit(nil);
                  CheckBoxOptimizerSearchTimeExit(nil);
          end;
  except  on E:Exception do begin
             MessageBox(Handle,E.Message,Caption,MB_OK+MB_ICONERROR);
             Result:=False;
             LoadSettingsFromIniFileResult:=prFailed;
             end;
  end;
  if OptimizerMethodSettingsExpired then SetDefaultOptimizerMethodSettings;
  SpinEditVicinityBoxChange(nil); CheckBoxVicinityBoxClick(nil); // ensure that zero-value vicinity-search permutations aren't selected
  if Result then Modified:=False;
  if LoadSettingsFromIniFileResult<>prOK then begin
     SetDefaultValuesForSolver; SetDefaultValuesForOptimizer; Modified:=True;
     end;
  ShowStatus;
end;

function TYASSdllSettingsForm.KeyWithSuffix(const Key__,Suffix__:String):String;
begin
  Result:=Key__+' - '+Suffix__;
end;

function TYASSdllSettingsForm.SaveSettingsToIniFile(ReallySaveSettings__:Boolean):Boolean;
var Section:String; IniFile:TIniFile; DateTime:TDateTime;
begin
  SpinEditVicinityBoxChange(nil); CheckBoxVicinityBoxClick(nil); // ensure that zero-value vicinity-search permutations aren't selected

  try     IniFile:=TIniFile.Create(IniFileName);
          try     //if IniFile.SectionExists(TEXT_APPLICATION_TITLE) then
                       // early versions of the program - with only a solver and
                       // no optimizer - had the solver settings in the section
                       // 'TEXT_APPLICATION_TITLE'
                  //   IniFile.EraseSection(TEXT_APPLICATION_TITLE);

                  IniFile.WriteString   (TEXT_APPLICATION_TITLE,KEY_TAB_SHEET,Trim(PageControl1.ActivePage.Caption)); // remember the most recently selected tabsheet

                  if ReallySaveSettings__ then begin
                     Section:=Trim(TabSheetSolver.Caption);
                     IniFile.WriteString   (Section,KEY_SEARCH_METHOD,TEXT_SEARCH_METHOD[SearchMethod]);
                     IniFile.WriteString   (Section,KEY_PACKING_ORDER,TEXT_NO_YES[CheckBoxPackingOrderSearch.Checked]);
                     IniFile.WriteInteger  (Section,KEY_PACKING_ORDER_THRESHOLD_BOXES,SpinEditPackingOrderSearchThreshold.Value);
                     IniFile.WriteInteger  (Section,KEY_TRANSPOSITION_TABLE_SIZE_MIB,SpinEditSolverTranspositionTableSize.Value);
                     IniFile.WriteString   (Section,KEY_SEARCH_TIME_LIMIT,TEXT_NO_YES[CheckBoxSolverSearchTime.Checked]);
                     IniFile.WriteInteger  (Section,KEY_SEARCH_TIME_LIMIT_SECONDS,SpinEditSolverMaxTime.Value);
                     IniFile.WriteInteger  (Section,KEY_PRECALCULATED_DEADLOCKS_COMPLEXITY,SpinEditSolverPrecalculatedDeadlocksComplexity.Value);
                     IniFile.WriteString   (Section,KEY_SMALL_OPTIMIZATIONS,TEXT_NO_YES[CheckBoxSmallOptimizations.Checked]);
                     IniFile.WriteString   (Section,KEY_STOP_WHEN_SOLVED,TEXT_NO_YES[CheckBoxStopWhenSolved.Checked]);
                     IniFile.WriteString   (Section,KEY_LOG,TEXT_NO_YES[CheckBoxSolverLog.Checked]);

                     Section:=Trim(TabSheetOptimizer.Caption);
                     // YASS 2.49 replaced 'Optimize moves yes/no' with the ternary 'Optimization', therefore the old key/value pair is deleted here
                     //IniFile.WriteString   (Section,KEY_OPTIMIZE_MOVES,TEXT_NO_YES[OptimizeMoves]);
                     IniFile.DeleteKey     (Section,KEY_OPTIMIZE_MOVES);
                     IniFile.WriteString   (Section,KEY_OPTIMIZATION,TEXT_OPTIMIZATION_TYPES[Optimization]);
                     IniFile.WriteInteger  (Section,KEY_TRANSPOSITION_TABLE_SIZE_MIB,SpinEditOptimizerTranspositionTableSize.Value);
                     IniFile.WriteString   (Section,KEY_SEARCH_TIME_LIMIT,TEXT_NO_YES[CheckBoxOptimizerSearchTime.Checked]);
                     IniFile.WriteInteger  (Section,KEY_SEARCH_TIME_LIMIT_SECONDS,SpinEditOptimizerMaxTime.Value);
                     IniFile.WriteInteger  (Section,KEY_PRECALCULATED_DEADLOCKS_COMPLEXITY,SpinEditOptimizerPrecalculatedDeadlocksComplexity.Value);
                     IniFile.WriteString   (Section,KEY_LOG,TEXT_NO_YES[CheckBoxOptimizerLog.Checked]);
                     IniFile.WriteString   (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_REARRANGEMENT,KEY_ENABLED),TEXT_NO_YES[CheckBoxRearrangementOptimization.Checked]);
                     IniFile.WriteString   (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_GLOBAL,KEY_ENABLED),TEXT_NO_YES[CheckBoxGlobalOptimization.Checked]);
                     IniFile.WriteString   (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_BOX_PERMUTATIONS,KEY_ENABLED),TEXT_NO_YES[CheckBoxBoxPermutationsOptimization.Checked]);
                     IniFile.WriteString   (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_VICINITY,KEY_ENABLED),TEXT_NO_YES[CheckBoxVicinityOptimization.Checked]);
                     IniFile.WriteString   (Section,KeyWithSuffix(KEY_OPTIMIZATION_METHOD_FALLBACK_STRATEGY,KEY_ENABLED),TEXT_NO_YES[CheckBoxFallbackStrategyOptimization.Checked]);
                     IniFile.WriteInteger  (Section,KEY_OPTIMIZATION_METHOD_REARRANGEMENT,SpinEditRearrangementOptimization.Value);
                     IniFile.WriteInteger  (Section,KEY_OPTIMIZATION_METHOD_GLOBAL,SpinEditGlobalOptimization.Value);
                     IniFile.WriteInteger  (Section,KEY_OPTIMIZATION_METHOD_BOX_PERMUTATIONS,SpinEditBoxPermutationsOptimization.Value);
                     IniFile.WriteInteger  (Section,KEY_OPTIMIZATION_METHOD_VICINITY,SpinEditVicinityOptimization.Value);
                     IniFile.WriteString   (Section,KEY_VICINITY_BOX_ENABLED+' 1',TEXT_NO_YES[CheckBoxVicinityBox1.Checked]);
                     IniFile.WriteString   (Section,KEY_VICINITY_BOX_ENABLED+' 2',TEXT_NO_YES[CheckBoxVicinityBox2.Checked]);
                     IniFile.WriteString   (Section,KEY_VICINITY_BOX_ENABLED+' 3',TEXT_NO_YES[CheckBoxVicinityBox3.Checked]);
                     IniFile.WriteString   (Section,KEY_VICINITY_BOX_ENABLED+' 4',TEXT_NO_YES[CheckBoxVicinityBox4.Checked]);
                     IniFile.WriteInteger  (Section,KEY_VICINITY_BOX+' 1',SpinEditVicinityBox1.Value);
                     IniFile.WriteInteger  (Section,KEY_VICINITY_BOX+' 2',SpinEditVicinityBox2.Value);
                     IniFile.WriteInteger  (Section,KEY_VICINITY_BOX+' 3',SpinEditVicinityBox3.Value);
                     IniFile.WriteInteger  (Section,KEY_VICINITY_BOX+' 4',SpinEditVicinityBox4.Value);
                     IniFile.WriteString   (Section,KEY_QUICK_VICINITY_SEARCH,TEXT_NO_YES[CheckBoxQuickVicinitySearch.Checked]);

                     if Optimizer.BoxPermutationsSearchTimeLimitMS<>DEFAULT_OPTIMIZER_BOX_PERMUTATIONS_SEARCH_TIME_LIMIT_MS then
                        IniFile.WriteInteger(Section,KEY_OPTIMIZER_BOX_PERMUTATIONS_SEARCH_TIME_LIMIT_SECONDS,Optimizer.BoxPermutationsSearchTimeLimitMS div 1000);

                     Section:=TEXT_SETTINGS;
                     DateTime:=Now;
                     IniFile.WriteString   (Section,KEY_DATE,DateToStr(DateTime));
                     IniFile.WriteString   (Section,KEY_TIME,TimeToStr(DateTime));
                     end;

                  Result:=True;
          finally IniFile.Free;
          end;
  except  on E:Exception do begin
             MessageBox(Handle,E.Message,Caption,MB_OK+MB_ICONERROR);
             Result:=False;
             end;
  end;
  if Result then Modified:=False;
  ShowStatus;
end;

procedure TYASSdllSettingsForm.CheckBoxClick(Sender: TObject);
begin
  if Sender is TCheckBox then with Sender as TCheckBox do begin
     Self.Modified:=Self.Modified or (Ord(Checked)<>Tag);
     Tag:=Ord(Checked);
     ShowStatus;
     end
  else
     ShowStatus;
end;

procedure TYASSdllSettingsForm.SpinEditChange(Sender: TObject);
begin
  if Sender is TSpinEdit then with Sender as TSpinEdit do begin
     try    if  Trim(Text)='' then begin // the text is allowed to the blank; when the spinedit control looses focus, a blank text is changed to '0'
                if Text<>'' then Text:='';
                end
            else begin
               if (StrToInt(Text)=0) then begin // 'StrToInt': check if it's a legal integer value; if not, then 'StrToInt' raises an 'EConvertError' exception
                  end;
               if MinValue<MaxValue then
                  if      Value<MinValue then Value:=MinValue
                  else if Value>MaxValue then Value:=MaxValue;
               if Value<>0 then
                  while (Text<>'') and (Text[1]='0') do Text:=Copy(Text,2,MaxInt); // trim unnecessary leading zero characters
               end;
     except on E:EConvertError do Text:=IntToStr(Tag); // restore the last valid value
     end;
     if Trim(Text)<>'' then begin
        Self.Modified:=Self.Modified or (Value<>Tag);
        Tag:=Value;
        end
     else begin
        Self.Modified:=Self.Modified or (Tag<>MinValue);
        Tag:=MinValue;
        end;
     ShowStatus;
     end
  else
     ShowStatus;
end;

procedure TYASSdllSettingsForm.SpinEditExit(Sender: TObject);
var s:String;
begin
  if Sender is TSpinEdit then with Sender as TSpinEdit do
     try    s:=IntToStr(Value);      // trim unnecessary leading zero characters, if any, and interpret a blank text as the minimum value; 'IntToStr' raises an 'EConvertError' if the characters in the spinedit text field isn't a valid integer
            if s<>Text then Text:=s; // update the text, if necessary
     except on E:EConvertError do Text:=IntToStr(MinValue);
     end;
end;

procedure TYASSdllSettingsForm.RadioButtonSearchStrategyClick(Sender: TObject);
begin
  if Sender is TRadioButton then with Sender as TRadioButton do begin
     Self.Modified:=Self.Modified or (Ord(Checked)<>Tag);
     if not Checked then Checked:=True;
     Tag:=Ord(Checked);
     ShowStatus;
     end
  else
     ShowStatus;
end;

procedure TYASSdllSettingsForm.RadioButtonOptimizeMovesOrPushesClick(Sender: TObject);
begin
  if Sender is TRadioButton then with Sender as TRadioButton do begin
     Self.Modified:=Self.Modified or (Ord(Checked)<>Tag);
     if not Checked then Checked:=True;
     Tag:=Ord(Checked);
     ShowStatus;
     end
  else
     ShowStatus;
end;

procedure TYASSdllSettingsForm.ShowStatus;
begin
  if StatusBar1.Panels[0].Text<>TEXT_OK_OR_MODIFIED[Modified] then
     StatusBar1.Panels[0].Text:=TEXT_OK_OR_MODIFIED[Modified];
end;

procedure TYASSdllSettingsForm.CheckBoxPackingOrderSearchEnabledClick(Sender: TObject);
begin
  CheckBoxClick(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditPackingOrderSearchThresholdChange(
  Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditSolverTranspositionTableSizeChange(Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditSolverMaxTimeChange(Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditSolverMaxPushesChange(Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditSolverMaxDepthChange(Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.SpinEditSolverPrecalculatedDeadlocksComplexityChange(
  Sender: TObject);
begin
  SpinEditChange(Sender);
end;

procedure TYASSdllSettingsForm.CheckBoxEnableDisableOptimizationMethodClick(
  Sender: TObject);
begin
  CheckBoxClick(Sender);
  if      Sender=CheckBoxRearrangementOptimization           then
          SpinEditRearrangementOptimization     .Enabled :=CheckBoxRearrangementOptimization     .Checked
  else if Sender=CheckBoxGlobalOptimization           then
          SpinEditGlobalOptimization         .Enabled :=CheckBoxGlobalOptimization         .Checked
  else if Sender=CheckBoxBoxPermutationsOptimization  then
          SpinEditBoxPermutationsOptimization.Enabled :=CheckBoxBoxPermutationsOptimization.Checked
  else if Sender=CheckBoxVicinityOptimization         then
          SpinEditVicinityOptimization       .Enabled :=CheckBoxVicinityOptimization       .Checked
  else if Sender=CheckBoxFallbackStrategyOptimization then begin
          end;

  EnableDisableOptimizationFallbackStrategy;
end;

procedure TYASSdllSettingsForm.EnableDisableOptimizationFallbackStrategy;
begin
  if CheckBoxRearrangementOptimization.Checked or
     CheckBoxGlobalOptimization.Checked or
     CheckBoxBoxPermutationsOptimization.Checked or
     CheckBoxVicinityOptimization.Checked then begin
     if not CheckBoxFallbackStrategyOptimization.Enabled then
        CheckBoxFallbackStrategyOptimization.Enabled:=True;
     end
  else begin
     if CheckBoxFallbackStrategyOptimization.Enabled then
        CheckBoxFallbackStrategyOptimization.Enabled:=False;
     if not CheckBoxFallbackStrategyOptimization.Checked then
        CheckBoxFallbackStrategyOptimization.Checked:=True;
     end;
end;

procedure TYASSdllSettingsForm.SpinEditOptimizationMethodOrderChange(
  Sender: TObject);
var i,OldValue,NewValue:Integer; Method,m:TOptimizationMethod;
begin
  with Sender as TSpinEdit do begin
    SpinEditChange(Sender);

    for Method:=Low(Method) to High(Method) do
        if Sender=OptimizationMethodSpinEdits[Method] then begin
           OldValue:=IndexOfOptimizationMethod(Method,OptimizationMethodOrder);
           NewValue:=Value;
           if (OldValue<>NewValue) and (OldValue<>-1) then begin
              if   OldValue<NewValue then
                   for i:=OldValue to     Pred(NewValue) do OptimizationMethodOrder[i]:=OptimizationMethodOrder[Succ(i)]
              else for i:=OldValue downto Succ(NewValue) do OptimizationMethodOrder[i]:=OptimizationMethodOrder[Pred(i)];
              OptimizationMethodOrder[NewValue]:=Method;

              for m:=Low(m) to High(m) do
                  if Assigned(OptimizationMethodSpinEdits[m]) then with OptimizationMethodSpinEdits[m] do begin
                     NewValue:=IndexOfOptimizationMethod(m,OptimizationMethodOrder);
                     if (Value<>NewValue) and (NewValue<>-1) then Value:=NewValue;
                     end;
              end;
           end;
    end;
end;

procedure TYASSdllSettingsForm.SpinEditVicinityBoxChange(Sender: TObject);

  procedure Check(SpinEdit__:TSpinEdit; CheckBox__:TCheckBox);
  begin
    if Trim(SpinEdit__.Text)<>'' then
       if      SpinEdit__.Value=0 then
               if   ActiveControl<>CheckBox__ then CheckBox__.Checked:=False
               else begin end
       else if (Sender=SpinEdit__) and (Sender=ActiveControl) then begin
               CheckBox__.Checked:=True;
               if OldQuickVicinitySearchChecked and
                  (not CheckBoxQuickVicinitySearch.Checked) then
                  // if the value for the spin-edit control changed from non-zero to zero
                  // and back to non-zero without leaving the control, then quick vicinity
                  // search became unchecked when the spin-edit value was zero;
                  // resurrect the checkmark here
                  CheckBoxQuickVicinitySearch.Checked:=True;
               end;
  end;

begin
  Check(SpinEditVicinityBox1,CheckBoxVicinityBox1);
  Check(SpinEditVicinityBox2,CheckBoxVicinityBox2);
  Check(SpinEditVicinityBox3,CheckBoxVicinityBox3);
  Check(SpinEditVicinityBox4,CheckBoxVicinityBox4);
  CheckBoxVicinityBoxClick(nil);
  SpinEditChange(Sender);
  if Assigned(Sender) and (Sender<>ActiveControl) then SpinEditExit(Sender);
end;

procedure TYASSdllSettingsForm.ButtonDefaultSettingsClick(Sender: TObject);
begin
  if      PageControl1.ActivePage=TabSheetSolver    then SetDefaultValuesForSolver
  else if PageControl1.ActivePage=TabSheetOptimizer then SetDefaultValuesForOptimizer;
  ShowStatus;
end;

procedure TYASSdllSettingsForm.ButtonDefaultSettingsMouseUp(
  Sender: TObject; Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  if   (Button=mbRight) and (Shift=[]) and (PageControl1.ActivePage=TabSheetOptimizer) then begin
       SetDefaultOptimizerMethodSettings;
       ShowStatus;
       end
  else FormMouseUp(Sender,Button,Shift,X,Y);
end;

procedure TYASSdllSettingsForm.SetDefaultOptimizerMethodSettings;
var i,VicinityBoxCount:Integer;

  procedure SetDefaultValueForVicinityBox(BoxNo__,VicinityBoxCount__:Integer; SpinEdit__:TSpinEdit; CheckBox__:TCheckBox);
  var DefaultValue:Integer;
  begin
    with SpinEdit__ do begin
       Enabled:=BoxNo__<=VicinityBoxCount__; CheckBox__.Checked:=Enabled;
       BoxNo__:=High(DEFAULT_VICINITY_SETTINGS)+1-BoxNo__;
       MinValue:=0;
       MaxValue:=MAX_VICINITY_SQUARES;
       DefaultValue:=Max(MinValue,DEFAULT_VICINITY_SETTINGS[BoxNo__]);
       Self.Modified:=Self.Modified or (Value<>DefaultValue);
       Value:=DefaultValue;
       end;
  end;

begin // SetDefaultOptimizerMethodSettings
  with CheckBoxRearrangementOptimization do begin
       Self.Modified:=Self.Modified or (not Checked);
       Checked:=True;
       end;

  with CheckBoxGlobalOptimization do begin
       Self.Modified:=Self.Modified or (not Checked);
       Checked:=True;
       end;

  with CheckBoxBoxPermutationsOptimization do begin
       Self.Modified:=Self.Modified or (not Checked);
       Checked:=True;
       end;

  with CheckBoxVicinityOptimization do begin
       Self.Modified:=Self.Modified or (not Checked);
       Checked:=True;
       end;

  with CheckBoxFallbackStrategyOptimization do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_OPTIMIZER_FALLBACK_STRATEGY_ENABLED);
       Checked:=DEFAULT_OPTIMIZER_FALLBACK_STRATEGY_ENABLED;
       end;

  EnableDisableOptimizationFallbackStrategy;

  for  i:=Low(OptimizationMethodOrder) to High(OptimizationMethodOrder) do
       OptimizationMethodOrder[i]:=Low(TOptimizationMethod); // 'Low': the fallback strategy for the optimization must be the first one; see 'TOptimizationMethod'

  with SpinEditGlobalOptimization do begin
       MinValue:=1; MaxValue:=4;
       Self.Modified:=Self.Modified or (Value<>Ord(omGlobalSearch));
       Value:=Ord(omGlobalSearch);
       OptimizationMethodOrder[Value]:=omGlobalSearch;
       end;

  with SpinEditBoxPermutationsOptimization do begin
       MinValue:=1; MaxValue:=SpinEditGlobalOptimization.MaxValue;
       Self.Modified:=Self.Modified or (Value<>Ord(omBoxPermutationsWithTimeLimit));
       Value:=Ord(omBoxPermutationsWithTimeLimit);
       OptimizationMethodOrder[Value]:=omBoxPermutationsWithTimeLimit;
       end;

  with SpinEditRearrangementOptimization do begin
       MinValue:=1; MaxValue:=SpinEditGlobalOptimization.MaxValue;
       Self.Modified:=Self.Modified or (Value<>Ord(omRearrangement));
       Value:=Ord(omRearrangement);
       OptimizationMethodOrder[Value]:=omRearrangement;
       end;

  with SpinEditVicinityOptimization do begin
       MinValue:=1; MaxValue:=SpinEditGlobalOptimization.MaxValue;
       Self.Modified:=Self.Modified or (Value<>Ord(omVicinitySearch));
       Value:=Ord(omVicinitySearch);
       OptimizationMethodOrder[Value]:=omVicinitySearch;
       end;

  VicinityBoxCount:=CalculateVicinityBoxCount(DEFAULT_VICINITY_SETTINGS);
  SetDefaultValueForVicinityBox(1,VicinityBoxCount,SpinEditVicinityBox1,CheckBoxVicinityBox1);
  SetDefaultValueForVicinityBox(2,VicinityBoxCount,SpinEditVicinityBox2,CheckBoxVicinityBox2);
  SetDefaultValueForVicinityBox(3,VicinityBoxCount,SpinEditVicinityBox3,CheckBoxVicinityBox3);
  SetDefaultValueForVicinityBox(4,VicinityBoxCount,SpinEditVicinityBox4,CheckBoxVicinityBox4);

  with CheckBoxQuickVicinitySearch do begin
       Self.Modified:=Self.Modified or (not Checked);
       Enabled:=VicinityBoxCount>1; Checked:=VicinityBoxCount<>1;
       OldQuickVicinitySearchChecked:=Checked;
       end;
end;

procedure TYASSdllSettingsForm.SetDefaultValuesForSolver;
var NewValue:Integer; PhysicalMemoryByteSize,PhysicalMemoryMiB:Cardinal;
begin //
  Self.Modified:=Self.Modified or (SearchMethod<>DEFAULT_SEARCH_METHOD);
  SearchMethod:=DEFAULT_SEARCH_METHOD;

  with CheckBoxPackingOrderSearch do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_PACKING_ORDER_ENABLED);
       Checked:=DEFAULT_PACKING_ORDER_ENABLED;
       end;
  with SpinEditPackingOrderSearchThreshold  do begin
       MinValue:=0; MaxValue:=MAX_BOX_COUNT;
       Self.Modified:=Self.Modified or (Value<>DEFAULT_PACKING_ORDER_BOX_COUNT_THRESHOLD);
       Value:=DEFAULT_PACKING_ORDER_BOX_COUNT_THRESHOLD;
       end;

  with SpinEditSolverTranspositionTableSize do begin
       MinValue              :=0;
       PhysicalMemoryByteSize:=GetPhysicalMemoryByteSize;
       PhysicalMemoryMiB     :=PhysicalMemoryByteSize div ONE_MEBI;
       if   PhysicalMemoryMiB<>0 then
            MaxValue         :=Min((PhysicalMemoryMiB div 4)*3                ,MAX_TRANSPOSITION_TABLE_BYTE_SIZE div ONE_MEBI)
       else MaxValue         :=Min(CalculateDefaultMemoryByteSize div ONE_MEBI,MAX_TRANSPOSITION_TABLE_BYTE_SIZE div ONE_MEBI);
       NewValue              :=CalculateDefaultMemoryByteSize div ONE_MEBI;
       Self.Modified:=Self.Modified or (Value<>NewValue);
       Value:=NewValue;
       end;

  Self.Modified:=Self.Modified or (CheckBoxSolverSearchTime.Checked<>True);
  CheckBoxSolverSearchTime.Checked:=True;
  with SpinEditSolverMaxTime do begin
       MinValue:=0; MaxValue:=MAX_SEARCH_TIME_LIMIT_MS div ONE_THOUSAND;
       NewValue:=DEFAULT_SEARCH_TIME_LIMIT_MS div ONE_THOUSAND;
       Self.Modified:=Self.Modified or (Value<>NewValue);
       Value:=NewValue;
       Enabled:=CheckBoxSolverSearchTime.Checked;
       end;

  with SpinEditSolverPrecalculatedDeadlocksComplexity do begin
       MinValue:=1; MaxValue:=3;
       Self.Modified:=Self.Modified or (Value<>DEFAULT_DEADLOCK_SETS_ADJACENT_OPEN_SQUARES_LIMIT);
       Value:=DEFAULT_DEADLOCK_SETS_ADJACENT_OPEN_SQUARES_LIMIT;
       end;

  with CheckBoxSmallOptimizations do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_OPTIMIZER_ENABLED);
       Checked:=DEFAULT_OPTIMIZER_ENABLED;
       end;

  with CheckBoxStopWhenSolved do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_STOP_WHEN_SOLVED);
       Checked:=DEFAULT_STOP_WHEN_SOLVED;
       end;

  with CheckBoxSolverLog do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_LOG_FILE_ENABLED);
       Checked:=DEFAULT_LOG_FILE_ENABLED;
       end;
end;

procedure TYASSdllSettingsForm.SetDefaultValuesForOptimizer;
var NewValue:Integer;
begin // precondition: 'SetDefaultValuesForSolver' has been called before this procedure
  Self.Modified:=Self.Modified or (Optimization<>DEFAULT_OPTIMIZATION);
  Optimization:=DEFAULT_OPTIMIZATION;

  with SpinEditOptimizerTranspositionTableSize do begin
       MinValue:=0;
       MaxValue:=SpinEditSolverTranspositionTableSize.MaxValue;
       NewValue:=CalculateDefaultMemoryByteSize div ONE_MEBI;
       Self.Modified:=Self.Modified or (Value<>NewValue);
       Value:=NewValue;
       end;

  Self.Modified:=Self.Modified or (CheckBoxOptimizerSearchTime.Checked<>DEFAULT_OPTIMIZER_TIME_LIMIT_ENABLED);
  CheckBoxOptimizerSearchTime.Checked:=DEFAULT_OPTIMIZER_TIME_LIMIT_ENABLED;
  with SpinEditOptimizerMaxTime do begin
       MinValue:=0; MaxValue:=MAX_SEARCH_TIME_LIMIT_MS div ONE_THOUSAND;
       NewValue:=DEFAULT_SEARCH_TIME_LIMIT_MS div ONE_THOUSAND;
       Self.Modified:=Self.Modified or (Value<>NewValue);
       Value:=NewValue;
       Enabled:=CheckBoxOptimizerSearchTime.Checked;
       end;

  with SpinEditOptimizerPrecalculatedDeadlocksComplexity do begin
       MinValue:=1; MaxValue:=3;
       Self.Modified:=Self.Modified or (Value<>DEFAULT_DEADLOCK_SETS_ADJACENT_OPEN_SQUARES_LIMIT);
       Value:=DEFAULT_DEADLOCK_SETS_ADJACENT_OPEN_SQUARES_LIMIT;
       end;

  with CheckBoxOptimizerLog do begin
       Self.Modified:=Self.Modified or (Checked<>DEFAULT_LOG_FILE_ENABLED);
       Checked:=DEFAULT_LOG_FILE_ENABLED;
       end;

  SetDefaultOptimizerMethodSettings;
end;

procedure TYASSdllSettingsForm.ButtonAboutClick(Sender: TObject);
begin
  ShowAbout(Handle,True,{PageControl1.ActivePage=TabSheetSolver} True,Caption);
end;

procedure TYASSdllSettingsForm.ButtonOKClick(Sender: TObject);
begin
 SaveSettingsToIniFile(True);
 if not Modified then begin
    ModalResult:=mrOk; Close; ModalResult:=mrOk;
    end;
end;

procedure TYASSdllSettingsForm.ButtonCancelClick(Sender: TObject);
begin
  Modified:=False; ShowStatus;
  if PageControl1.ActivePage<>FirstTabSheet then SaveSettingsToIniFile(False);
  ModalResult:=mrCancel; Close; ModalResult:=mrCancel;
end;

procedure TYASSdllSettingsForm.FormCreate(Sender: TObject);
var i:Integer; s:String;
begin
  // use a fixed date and time format; it helps to avoid problems with locale settings
  DateSeparator  :='-';
  TimeSeparator  :=':';
  LongTimeFormat :='hh'  +TimeSeparator+'mm'+TimeSeparator+'ss';
  ShortDateFormat:='yyyy'+DateSeparator+'MM'+DateSeparator+'dd';

  StatusBar1.Font.Assign(Self.Font);
  Modified:=False; HintControl:=nil; LoadSettingsFromIniFileResult:=prFailed;
  Caption:=YASS.TEXT_APPLICATION_TITLE_LONG;

  SetLength(s,10); for i:=1 to Length(s) do s[i]:=SPACE;
  for i:=0 to Pred(PageControl1.PageCount) do with PageControl1.Pages[i] do
      Caption:=s+Caption+s; // increase the tabsheet captions so the user gets a larger clickable area

  for i:=0 to Pred(ComponentCount) do
      if      Components[i] is TPanel       then with Components[i] as TPanel       do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TLabel       then with Components[i] as TLabel       do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TButton      then with Components[i] as TButton      do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TRadioButton then with Components[i] as TRadioButton do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TCheckBox    then with Components[i] as TCheckBox    do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TComboBox    then with Components[i] as TComboBox    do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TGroupBox    then with Components[i] as TGroupBox    do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TListBox     then with Components[i] as TListBox     do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TMemo        then with Components[i] as TMemo        do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TImage       then with Components[i] as TImage       do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TSpinEdit    then with Components[i] as TSpinEdit    do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove;
                                                                                             if MaxLength=0 then MaxLength:=MAX_SPIN_EDIT_TEXT_LENGTH;
                                                                                       end
      else if Components[i] is TStatusBar   then with Components[i] as TStatusBar   do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end
      else if Components[i] is TTabSheet    then with Components[i] as TTabSheet    do begin OnMouseUp:=FormMouseUp; if not Assigned(OnMouseMove) then OnMouseMove:=ControlMouseMove; end;
  ButtonDefaultSettings.OnMouseUp:=ButtonDefaultSettingsMouseUp;

  SpinEditRearrangementOptimization.Hint:=CheckBoxRearrangementOptimization.Hint;
  SpinEditGlobalOptimization.Hint:=CheckBoxGlobalOptimization.Hint;
  SpinEditBoxPermutationsOptimization.Hint:=CheckBoxBoxPermutationsOptimization.Hint;
  SpinEditVicinityOptimization.Hint:=CheckBoxVicinityOptimization.Hint;
  SpinEditVicinityBox2.Hint:=SpinEditVicinityBox1.Hint;
  SpinEditVicinityBox3.Hint:=SpinEditVicinityBox1.Hint;
  SpinEditVicinityBox4.Hint:=SpinEditVicinityBox1.Hint;
  CheckBoxVicinityBox2.Hint:=CheckBoxVicinityBox1.Hint;
  CheckBoxVicinityBox3.Hint:=CheckBoxVicinityBox1.Hint;
  CheckBoxVicinityBox4.Hint:=CheckBoxVicinityBox1.Hint;

  OptimizationMethodSpinEdits[omBoxPermutations             ]:=nil;
  OptimizationMethodSpinEdits[omRearrangement               ]:=SpinEditRearrangementOptimization;
  OptimizationMethodSpinEdits[omGlobalSearch                ]:=SpinEditGlobalOptimization;
  OptimizationMethodSpinEdits[omBoxPermutationsWithTimeLimit]:=SpinEditBoxPermutationsOptimization;
  OptimizationMethodSpinEdits[omVicinitySearch              ]:=SpinEditVicinityOptimization;

  PageControl1.ActivePage:=TabSheetSolver;
//PageControl1.ActivePage:=TabSheetOptimizer;
  SetDefaultValuesForSolver; SetDefaultValuesForOptimizer; Modified:=False;
  LoadSettingsFromIniFile;
  PageControl1Change(PageControl1.ActivePage);
  FirstTabSheet:=PageControl1.ActivePage;
  ActiveControl:=ButtonOK;
  ShowStatus;
end;

procedure TYASSdllSettingsForm.ControlMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
begin
  DisplayHint(Sender);
end;

procedure TYASSdllSettingsForm.DisplayHint(Sender: TObject);
begin
  if (Sender<>HintControl) then begin
     if   (Sender is TControl) then
          StatusBar1.Panels[1].Text := GetLongHint(TControl(Sender).Hint)
     else StatusBar1.Panels[1].Text := '';
     HintControl:=Sender;
     end;
end;

procedure TYASSdllSettingsForm.FormActivate(Sender: TObject);
begin
  PanelTop.Font.Color:=ApplicationHiglightedTextColor;
end;

procedure TYASSdllSettingsForm.FormCloseQuery(Sender: TObject;
  var CanClose: Boolean);
begin
  if Modified then begin
     if   ActiveControl is TSpinEdit then ActiveControl:=ButtonOK; // force updating of the spinedit component
     case MessageBox(Handle,TEXT_SAVE_CHANGES,Caption,MB_YESNOCANCEL+MB_ICONQUESTION) of
          IDYES    : SaveSettingsToIniFile(True);
          IDNO     : begin Modified:=False; ShowStatus;
                           if PageControl1.ActivePage<>FirstTabSheet then SaveSettingsToIniFile(False);
                     end;
          IDCANCEL :;
     end; // case
     end;
  CanClose:=not Modified;
end;

procedure TYASSdllSettingsForm.FormKeyUp(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if (Key=VK_ESCAPE) and (not Modified) then Close;
end;

procedure TYASSdllSettingsForm.FormMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if (Button=mbRight) and (not Modified) then Close;
end;

procedure TYASSdllSettingsForm.PageControl1Change(Sender: TObject);
begin
  with PageControl1 do begin
    PanelTop.Caption:=YASS.TEXT_APPLICATION_TITLE+' - '+Trim(ActivePage.Caption)+SPACE+TEXT_SETTINGS+SPACE;
    StatusBar1.Panels[1].Text:='';
    OldQuickVicinitySearchChecked:=CheckBoxQuickVicinitySearch.Checked;
    if   ActivePage=TabSheetOptimizer then begin
         ButtonDefaultSettings.Hint:=TEXT_HINT_DEFAULT_SETTINGS_OPTIMIZER;
         SpinEditVicinityBoxChange(nil); CheckBoxVicinityBoxClick(nil);
         end
    else ButtonDefaultSettings.Hint:=TEXT_HINT_DEFAULT_SETTINGS;
    end;
end;

procedure TYASSdllSettingsForm.CheckBoxVicinityBoxClick(Sender: TObject);
var Count:Integer;
begin
  SpinEditVicinityBox1.Enabled:=CheckBoxVicinityBox1.Checked or (ActiveControl=SpinEditVicinityBox1);
  SpinEditVicinityBox2.Enabled:=CheckBoxVicinityBox2.Checked or (ActiveControl=SpinEditVicinityBox2);
  SpinEditVicinityBox3.Enabled:=CheckBoxVicinityBox3.Checked or (ActiveControl=SpinEditVicinityBox3);
  SpinEditVicinityBox4.Enabled:=CheckBoxVicinityBox4.Checked or (ActiveControl=SpinEditVicinityBox4);

  CheckBoxClick(Sender);

  Count:=0;
  if CheckBoxVicinityBox1.Checked and ((Trim(SpinEditVicinityBox1.Text)='') or (SpinEditVicinityBox1.Value>0)) then Inc(Count);
  if CheckBoxVicinityBox2.Checked and ((Trim(SpinEditVicinityBox2.Text)='') or (SpinEditVicinityBox2.Value>0)) then Inc(Count);
  if CheckBoxVicinityBox3.Checked and ((Trim(SpinEditVicinityBox3.Text)='') or (SpinEditVicinityBox3.Value>0)) then Inc(Count);
  if CheckBoxVicinityBox4.Checked and ((Trim(SpinEditVicinityBox4.Text)='') or (SpinEditVicinityBox4.Value>0)) then Inc(Count);
  with CheckBoxQuickVicinitySearch do begin
    if Count=0 then Checked:=True;
    if Count=1 then Checked:=False;
    Enabled:=Count>1;
    end;
end;

procedure TYASSdllSettingsForm.CheckBoxVicinityBoxExit(Sender: TObject);
begin
  if Trim(SpinEditVicinityBox1.Text)='' then begin SpinEditVicinityBox1.Text:='0'; SpinEditVicinityBox1.Value:=0; SpinEditVicinityBox1.Tag:=0; end;
  if Trim(SpinEditVicinityBox2.Text)='' then begin SpinEditVicinityBox2.Text:='0'; SpinEditVicinityBox2.Value:=0; SpinEditVicinityBox2.Tag:=0; end;
  if Trim(SpinEditVicinityBox3.Text)='' then begin SpinEditVicinityBox3.Text:='0'; SpinEditVicinityBox3.Value:=0; SpinEditVicinityBox3.Tag:=0; end;
  if Trim(SpinEditVicinityBox4.Text)='' then begin SpinEditVicinityBox4.Text:='0'; SpinEditVicinityBox4.Value:=0; SpinEditVicinityBox3.Tag:=0; end;
  if CheckBoxVicinityBox1.Checked and (SpinEditVicinityBox1.Tag=0) and (ActiveControl<>SpinEditVicinityBox1) then CheckBoxVicinityBox1.Checked:=False;
  if CheckBoxVicinityBox2.Checked and (SpinEditVicinityBox2.Tag=0) and (ActiveControl<>SpinEditVicinityBox2) then CheckBoxVicinityBox2.Checked:=False;
  if CheckBoxVicinityBox3.Checked and (SpinEditVicinityBox3.Tag=0) and (ActiveControl<>SpinEditVicinityBox3) then CheckBoxVicinityBox3.Checked:=False;
  if CheckBoxVicinityBox4.Checked and (SpinEditVicinityBox4.Tag=0) and (ActiveControl<>SpinEditVicinityBox4) then CheckBoxVicinityBox4.Checked:=False;
  CheckBoxVicinityBoxClick(Sender);
end;

procedure TYASSdllSettingsForm.ControlEnter(Sender: TObject);
begin
  OldQuickVicinitySearchChecked:=CheckBoxQuickVicinitySearch.Checked;
  CheckBoxVicinityBoxExit(Sender);
end;

procedure TYASSdllSettingsForm.CheckBoxSolverSearchTimeClick(Sender: TObject);
begin
  CheckBoxClick(Sender);
  if      CheckBoxSolverSearchTime.Checked then
          SpinEditSolverMaxTime.Enabled:=True
  else if ActiveControl<>SpinEditSolverMaxTime then
          SpinEditSolverMaxTime.Enabled:=False;
end;

procedure TYASSdllSettingsForm.CheckBoxSolverSearchTimeExit(Sender: TObject);
begin
  SpinEditExit(Sender);
  if (SpinEditSolverMaxTime.Value=0) and
     (ActiveControl<>CheckBoxSolverSearchTime) and
     (ActiveControl<>SpinEditSolverMaxTime) then
     CheckBoxSolverSearchTime.Checked:=False;
  CheckBoxSolverSearchTimeClick(nil);
end;

procedure TYASSdllSettingsForm.CheckBoxOptimizerSearchTimeClick(Sender: TObject);
begin
  CheckBoxClick(Sender);
  if      CheckBoxOptimizerSearchTime.Checked then
          SpinEditOptimizerMaxTime.Enabled:=True
  else if ActiveControl<>SpinEditOptimizerMaxTime then
          SpinEditOptimizerMaxTime.Enabled:=False;
end;

procedure TYASSdllSettingsForm.CheckBoxOptimizerSearchTimeExit(Sender: TObject);
begin
  SpinEditExit(Sender);
  if (SpinEditOptimizerMaxTime.Value=0) and
     (ActiveControl<>CheckBoxOptimizerSearchTime) and
     (ActiveControl<>SpinEditOptimizerMaxTime) then
     CheckBoxOptimizerSearchTime.Checked:=False;
  CheckBoxOptimizerSearchTimeClick(nil);
end;

end.


