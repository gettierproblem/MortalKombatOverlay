using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MortalKombatOCRWrapper;
using Newtonsoft.Json;
using Point = System.Drawing.Point;

namespace MortalKombatOverlay;

public class GameWatcherViewModel : INotifyPropertyChanged
{
    private DispatcherTimer? _gameWatcherTimer;
    private IntPtr _hwnd = IntPtr.Zero;

    private string _player1 = string.Empty;
    private MoveListState _player1MoveListState = MoveListState.SpecialMovesOnly;
    private readonly ObservableCollection<KeyValuePair<string, List<MovePart>>> _player1Moves = new();

    private string _player2 = string.Empty;
    private MoveListState _player2MoveListState = MoveListState.SpecialMovesOnly;
    private readonly ObservableCollection<KeyValuePair<string, List<MovePart>>> _player2Moves = new();

    private bool _wasMortalKombatMinimized;
    private IntPtr previousHwnd = IntPtr.Zero;


    public GameWatcherViewModel()
    {
        ChangePlayer1MoveListStateCommand = new RelayCommand(_ => ChangePlayer1MoveListState());
        ChangePlayer2MoveListStateCommand = new RelayCommand(_ => ChangePlayer2MoveListState());
        LaunchMortalKombatCommand = new RelayCommand(_ => LaunchMortalKombat());
        CloseCommand = new RelayCommand(_ => CloseWindow());
    }


    public ICommand ChangePlayer1MoveListStateCommand { get; }
    public ICommand ChangePlayer2MoveListStateCommand { get; }


    // JSON data structure for characters
    public Dictionary<string, CharacterData> CharacterDataMap { get; set; } = new();

    public ICommand CloseCommand { get; }

    public bool IsMortalKombatRunning => _hwnd != IntPtr.Zero && IsWindow(_hwnd) && !IsIconic(_hwnd);

    public ICommand LaunchMortalKombatCommand { get; }

    public string Player1
    {
        get => _player1;
        set
        {
            _player1 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Player1)));
            UpdatePlayer1Moves();
        }
    }

    public string Player1MoveListStateText => GetStateText(_player1MoveListState, "P1");

    public ObservableCollection<KeyValuePair<string, List<MovePart>>> Player1Moves
    {
        get => _player1Moves;
        set
        {
            if (value != null)
            {
                _player1Moves.Clear();
                foreach (var movePartList in value) _player1Moves.Add(movePartList);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Player1Moves)));
        }
    }

    public string Player2
    {
        get => _player2;
        set
        {
            _player2 = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Player2)));
            UpdatePlayer2Moves();
        }
    }

    public string Player2MoveListStateText => GetStateText(_player2MoveListState, "P2");

    public ObservableCollection<KeyValuePair<string, List<MovePart>>> Player2Moves
    {
        get => _player2Moves;
        set
        {
            if (value != null)
            {
                _player2Moves.Clear();
                foreach (var movePartList in value) _player2Moves.Add(movePartList);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Player2Moves)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    [DllImport("user32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);

    public void ProcessBitmap(Bitmap screenshot)
    {
        var names = OcrHelper.ExtractCharacterNames(screenshot);

        // characters can jump in front of their name overlay or discord notifications can cover it up
        // so don't change the state of the move list if we only see one name

        // check if both names are either empty/whitespace or neither are empty/whitespace.
        var bothEmptyOrWhitespace = string.IsNullOrWhiteSpace(names[0]) && string.IsNullOrWhiteSpace(names[1]);
        var neitherEmptyNorWhitespace =
            !string.IsNullOrWhiteSpace(names[0]) && !string.IsNullOrWhiteSpace(names[1]);

        if (names.Count == 2 && (bothEmptyOrWhitespace || neitherEmptyNorWhitespace))
        {
            Player1 = names[0];
            Player2 = names[1];
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);


    public void SetGameForeground()
    {
        SetForegroundWindow(_hwnd);
    }

    public void StartWatching(string title)
    {
        LoadCharacterData(); // Load JSON data during initialization
        _gameWatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3) // 3 seconds interval
        };

        _gameWatcherTimer.Tick += (sender, e) => WatchGameWindow(title);
        _gameWatcherTimer.Start();
    }

    public void StopWatching()
    {
        _gameWatcherTimer?.Stop();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        // dispose managed resources
        _gameWatcherTimer?.Stop();
        _gameWatcherTimer = null;
    }

    private void CaptureAndProcessScreenshot(IntPtr windowHandle, string savePath = null)
    {
        var bounds = GetClientBounds(windowHandle);
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
        }

        ProcessBitmap(bitmap);

        if (!string.IsNullOrEmpty(savePath))
            bitmap.Save(savePath,
                ImageFormat.Png); // Save as PNG format. You can change this to another format if needed.
    }

    private void ChangePlayer1MoveListState()
    {
        ChangePlayerMoveListState(ref _player1MoveListState, Player1Moves, "Player1");
    }

    private void ChangePlayer2MoveListState()
    {
        ChangePlayerMoveListState(ref _player2MoveListState, Player2Moves, "Player2");
    }

    private void ChangePlayerMoveListState(ref MoveListState playerMoveListState,
        ObservableCollection<KeyValuePair<string, List<MovePart>>> movesCollection, string propertyName)
    {
        playerMoveListState = (MoveListState)(((int)playerMoveListState + 1) % 4);
        UpdateMoves(movesCollection, playerMoveListState, propertyName);
        SetForegroundWindow(_hwnd);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName + "MoveListStateText"));
    }

    private void ClearPlayerInfo()
    {
        Player1 = string.Empty;
        Player2 = string.Empty;
    }

    private void CloseWindow()
    {
        Application.Current.MainWindow?.Close();
    }

    // todo: move this to a helper class
    private Rectangle GetClientBounds(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero) return new Rectangle();

        GetClientRect(hwnd, out var clientRect);

        var topLeft = new Point(clientRect.Left, clientRect.Top);
        var bottomRight = new Point(clientRect.Right, clientRect.Bottom);

        ClientToScreen(hwnd, ref topLeft);
        ClientToScreen(hwnd, ref bottomRight);


        topLeft.X = topLeft.X;
        topLeft.Y = topLeft.Y;
        bottomRight.X = bottomRight.X;
        bottomRight.Y = bottomRight.Y;

        return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }

    private static string GetStateText(MoveListState state, string playerPrefix)
    {
        return state switch
        {
            MoveListState.AllVisible => $"{playerPrefix}: Show Special Moves Only",
            MoveListState.SpecialMovesOnly => $"{playerPrefix}: Show Fatalities Only",
            MoveListState.FatalitiesOnly => $"{playerPrefix}: Hide Moves",
            MoveListState.Hidden => $"{playerPrefix}: Show All Moves",
            _ => "Unknown"
        };
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(IntPtr hWnd);


    private void LaunchMortalKombat()
    {
        try
        {
            Process.Start(new ProcessStartInfo("steam://rungameid/1971870") { UseShellExecute = true });
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMortalKombatRunning)));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message); // Or handle this exception as needed.
        }
    }

    // Load JSON data into CharacterDataMap
    private void LoadCharacterData()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileName = "mortal_kombat_move_list_v2.json";

            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonText = reader.ReadToEnd();

                // Deserialize the JSON data into CharacterDataMap
                CharacterDataMap = JsonConvert.DeserializeObject<Dictionary<string, CharacterData>>(jsonText);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to load move list data. Error: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void RefreshGameWindowHandle(string title)
    {
        if (_hwnd == IntPtr.Zero || !IsWindow(_hwnd)) _hwnd = FindWindow("UnrealWindow", title);
    }

    private void UpdateMoves(
        ObservableCollection<KeyValuePair<string, List<MovePart>>> movesCollection,
        MoveListState moveState,
        string playerName)
    {
        movesCollection.Clear();

        if (moveState == MoveListState.Hidden || !CharacterDataMap.ContainsKey(playerName)) return;

        var characterData = CharacterDataMap[playerName];

        if (moveState == MoveListState.AllVisible || moveState == MoveListState.SpecialMovesOnly)
            foreach (var move in characterData.SpecialMoves)
                movesCollection.Add(new KeyValuePair<string, List<MovePart>>(move.Key, move.Value));

        if (moveState == MoveListState.AllVisible || moveState == MoveListState.FatalitiesOnly)
            foreach (var move in characterData.Finishers)
                movesCollection.Add(new KeyValuePair<string, List<MovePart>>(move.Key, move.Value));
    }

    private void UpdatePlayer1Moves()
    {
        UpdateMoves(Player1Moves, _player1MoveListState, Player1);
    }

    private void UpdatePlayer2Moves()
    {
        UpdateMoves(Player2Moves, _player2MoveListState, Player2);
    }

    private void WatchGameWindow(string title)
    {
        RefreshGameWindowHandle(title);

        var isGameRunning = _hwnd != IntPtr.Zero;
        var isGameMinimized = isGameRunning && IsIconic(_hwnd);

        if (_hwnd != previousHwnd || isGameMinimized != _wasMortalKombatMinimized)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMortalKombatRunning)));
            previousHwnd = _hwnd;
            _wasMortalKombatMinimized = isGameMinimized;
        }

        if (isGameRunning)
        {
            if (isGameMinimized)
            {
                Debug.WriteLine("Game is minimized.");
                ClearPlayerInfo();
            }
            else
            {
                CaptureAndProcessScreenshot(_hwnd);
            }
        }
        else
        {
            Debug.WriteLine("Game not running.");
            ClearPlayerInfo();
        }
    }
}