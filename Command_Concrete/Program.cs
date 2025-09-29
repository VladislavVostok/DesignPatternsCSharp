namespace Command_Concrete
{
	// ==================== INTERFACES ====================
	public interface ICommand
	{
		string Name { get; }
		string Description { get; }
		Task<bool> ExecuteAsync();
		Task<bool> UndoAsync();
		bool CanExecute();
		bool CanUndo();
	}

	public interface ICommandHistory
	{
		void Push(ICommand command);
		ICommand Pop();
		ICommand Peek();
		void Clear();
		int Count { get; }
		IEnumerable<ICommand> GetHistory();
		IEnumerable<ICommand> GetUndoableCommands();
	}

	public interface ITextEditor
	{
		string Content { get; }
		string SelectedText { get; }
		int SelectionStart { get; }
		int SelectionLength { get; }

		event EventHandler<TextChangedEventArgs> TextChanged;

		void Select(int start, int length);
		void InsertText(string text, int position);
		void DeleteText(int start, int length);
		void ReplaceText(string text, int start, int length);
	}

	// ==================== EVENTS ====================
	public class TextChangedEventArgs : EventArgs
	{
		public string OldText { get; }
		public string NewText { get; }
		public string ChangeType { get; }

		public TextChangedEventArgs(string oldText, string newText, string changeType)
		{
			OldText = oldText;
			NewText = newText;
			ChangeType = changeType;
		}
	}

	public class CommandExecutedEventArgs : EventArgs
	{
		public ICommand Command { get; }
		public bool Success { get; }
		public TimeSpan ExecutionTime { get; }

		public CommandExecutedEventArgs(ICommand command, bool success, TimeSpan executionTime)
		{
			Command = command;
			Success = success;
			ExecutionTime = executionTime;
		}
	}

	// ==================== CORE IMPLEMENTATIONS ====================
	public class AdvancedTextEditor : ITextEditor
	{
		private string _content = string.Empty;
		private int _selectionStart;
		private int _selectionLength;

		public string Content
		{
			get => _content;
			private set
			{
				if (_content != value)
				{
					var oldText = _content;
					_content = value;
					TextChanged?.Invoke(this, new TextChangedEventArgs(oldText, value, "ContentChanged"));
				}
			}
		}

		public string SelectedText =>
			_selectionLength > 0 ? Content.Substring(_selectionStart, _selectionLength) : string.Empty;

		public int SelectionStart => _selectionStart;
		public int SelectionLength => _selectionLength;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		public void Select(int start, int length)
		{
			_selectionStart = Math.Max(0, Math.Min(start, Content.Length));
			_selectionLength = Math.Max(0, Math.Min(length, Content.Length - _selectionStart));
		}

		public void InsertText(string text, int position)
		{
			if (string.IsNullOrEmpty(text)) return;

			position = Math.Max(0, Math.Min(position, Content.Length));
			var oldContent = Content;
			Content = Content.Insert(position, text);

			TextChanged?.Invoke(this, new TextChangedEventArgs(oldContent, Content, "Insert"));
		}

		public void DeleteText(int start, int length)
		{
			if (length <= 0) return;

			start = Math.Max(0, Math.Min(start, Content.Length));
			length = Math.Min(length, Content.Length - start);

			var oldContent = Content;
			Content = Content.Remove(start, length);

			TextChanged?.Invoke(this, new TextChangedEventArgs(oldContent, Content, "Delete"));
		}

		public void ReplaceText(string text, int start, int length)
		{
			start = Math.Max(0, Math.Min(start, Content.Length));
			length = Math.Min(length, Content.Length - start);

			var oldContent = Content;
			Content = Content.Remove(start, length).Insert(start, text);

			TextChanged?.Invoke(this, new TextChangedEventArgs(oldContent, Content, "Replace"));
		}
	}

	public class CommandHistory : ICommandHistory
	{
		private readonly Stack<ICommand> _history = new Stack<ICommand>();
		private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();
		private readonly int _maxSize;

		public CommandHistory(int maxSize = 100)
		{
			_maxSize = maxSize;
		}

		public int Count => _history.Count;

		public void Push(ICommand command)
		{
			if (_history.Count >= _maxSize)
			{
				// Remove oldest command when max size reached
				var temp = new Stack<ICommand>();
				while (_history.Count > 1) // Keep at least one
					temp.Push(_history.Pop());

				_history.Clear();
				while (temp.Count > 0)
					_history.Push(temp.Pop());
			}

			_history.Push(command);
			_redoStack.Clear(); // Clear redo stack when new command is executed
		}

		public ICommand Pop()
		{
			return _history.Count > 0 ? _history.Pop() : null;
		}

		public ICommand Peek()
		{
			return _history.Count > 0 ? _history.Peek() : null;
		}

		public void Clear()
		{
			_history.Clear();
			_redoStack.Clear();
		}

		public IEnumerable<ICommand> GetHistory()
		{
			return _history.ToArray().Reverse(); // Return in chronological order
		}

		public IEnumerable<ICommand> GetUndoableCommands()
		{
			return GetHistory().Where(cmd => cmd.CanUndo());
		}
	}



	public abstract class EditorCommand : ICommand
	{
		protected readonly ITextEditor Editor;
		protected string BackupContent;
		protected int BackupSelectionStart;
		protected int BackupSelectionLength;

		public abstract string Name { get; }
		public abstract string Description { get; }

		protected EditorCommand(ITextEditor editor)
		{
			Editor = editor ?? throw new ArgumentNullException(nameof(editor));
		}

		protected virtual void SaveBackup()
		{
			BackupContent = Editor.Content;
			BackupSelectionStart = Editor.SelectionStart;
			BackupSelectionLength = Editor.SelectionLength;
		}

		protected virtual void RestoreBackup()
		{
			// This would need to be implemented with proper editor state restoration
		}

		public abstract Task<bool> ExecuteAsync();
		public abstract Task<bool> UndoAsync();

		public virtual bool CanExecute() => true;
		public virtual bool CanUndo() => !string.IsNullOrEmpty(BackupContent);
	}

	public class CopyCommand : EditorCommand
	{
		private readonly IClipboardService _clipboard;

		public override string Name => "Copy";
		public override string Description => "Copy selected text to clipboard";

		public CopyCommand(ITextEditor editor, IClipboardService clipboard) : base(editor)
		{
			_clipboard = clipboard;
		}

		public override async Task<bool> ExecuteAsync()
		{
			if (string.IsNullOrEmpty(Editor.SelectedText))
				return false;

			await _clipboard.SetTextAsync(Editor.SelectedText);
			return true;
		}

		public override Task<bool> UndoAsync() => Task.FromResult(true); // Copy cannot be undone

		public override bool CanExecute() => !string.IsNullOrEmpty(Editor.SelectedText);
		public override bool CanUndo() => false;
	}

	public class CutCommand : EditorCommand
	{
		private readonly IClipboardService _clipboard;

		public override string Name => "Cut";
		public override string Description => "Cut selected text to clipboard";

		public CutCommand(ITextEditor editor, IClipboardService clipboard) : base(editor)
		{
			_clipboard = clipboard;
		}

		public override async Task<bool> ExecuteAsync()
		{
			if (string.IsNullOrEmpty(Editor.SelectedText))
				return false;

			SaveBackup();
			await _clipboard.SetTextAsync(Editor.SelectedText);
			Editor.DeleteText(Editor.SelectionStart, Editor.SelectionLength);
			return true;
		}

		public override Task<bool> UndoAsync()
		{
			Editor.ReplaceText(BackupContent, 0, Editor.Content.Length);
			Editor.Select(BackupSelectionStart, BackupSelectionLength);
			return Task.FromResult(true);
		}
	}

	public class PasteCommand : EditorCommand
	{
		private readonly IClipboardService _clipboard;

		public override string Name => "Paste";
		public override string Description => "Paste text from clipboard";

		public PasteCommand(ITextEditor editor, IClipboardService clipboard) : base(editor)
		{
			_clipboard = clipboard;
		}

		public override async Task<bool> ExecuteAsync()
		{
			var clipboardText = await _clipboard.GetTextAsync();
			if (string.IsNullOrEmpty(clipboardText))
				return false;

			SaveBackup();

			if (Editor.SelectionLength > 0)
			{
				Editor.ReplaceText(clipboardText, Editor.SelectionStart, Editor.SelectionLength);
			}
			else
			{
				Editor.InsertText(clipboardText, Editor.SelectionStart);
			}

			return true;
		}

		public override Task<bool> UndoAsync()
		{
			Editor.ReplaceText(BackupContent, 0, Editor.Content.Length);
			Editor.Select(BackupSelectionStart, BackupSelectionLength);
			return Task.FromResult(true);
		}
	}

	public class UndoCommand : EditorCommand
	{
		private readonly ICommandHistory _history;

		public override string Name => "Undo";
		public override string Description => "Undo last command";

		public UndoCommand(ITextEditor editor, ICommandHistory history) : base(editor)
		{
			_history = history;
		}

		public override async Task<bool> ExecuteAsync()
		{
			var lastCommand = _history.Pop();
			if (lastCommand != null && lastCommand.CanUndo())
			{
				return await lastCommand.UndoAsync();
			}
			return false;
		}

		public override Task<bool> UndoAsync() => Task.FromResult(false);

		public override bool CanExecute() => _history.Count > 0;
		public override bool CanUndo() => false;
	}

	public class MacroCommand : EditorCommand
	{
		private readonly List<ICommand> _commands = new List<ICommand>();

		public override string Name => "Macro";
		public override string Description => $"Macro containing {_commands.Count} commands";

		public MacroCommand(ITextEditor editor) : base(editor) { }

		public void AddCommand(ICommand command)
		{
			_commands.Add(command);
		}

		public override async Task<bool> ExecuteAsync()
		{
			SaveBackup();
			bool success = true;

			foreach (var command in _commands)
			{
				if (!await command.ExecuteAsync())
				{
					success = false;
					break;
				}
			}

			return success;
		}

		public override async Task<bool> UndoAsync()
		{
			bool success = true;

			foreach (var command in _commands.Reverse<ICommand>())
			{
				if (command.CanUndo() && !await command.UndoAsync())
				{
					success = false;
					break;
				}
			}

			return success;
		}

		public override bool CanExecute() => _commands.All(cmd => cmd.CanExecute());
		public override bool CanUndo() => _commands.All(cmd => cmd.CanUndo());
	}
	public interface IClipboardService
	{
		Task<string> GetTextAsync();
		Task SetTextAsync(string text);
	}

	public class ClipboardService : IClipboardService
	{
		private string _clipboardContent = string.Empty;

		public Task<string> GetTextAsync() => Task.FromResult(_clipboardContent);
		public Task SetTextAsync(string text)
		{
			_clipboardContent = text ?? string.Empty;
			return Task.CompletedTask;
		}
	}


	public class AdvancedTextEditorApplication
	{
		private readonly ICommandHistory _history;
		private readonly ITextEditor _editor;
		private readonly IClipboardService _clipboard;
		private readonly Dictionary<string, ICommand> _commands;

		public event EventHandler<CommandExecutedEventArgs> CommandExecuted;

		public AdvancedTextEditorApplication()
		{
			_editor = new AdvancedTextEditor();
			_clipboard = new ClipboardService();
			_history = new CommandHistory();

			_commands = new Dictionary<string, ICommand>
			{
				["copy"] = new CopyCommand(_editor, _clipboard),
				["cut"] = new CutCommand(_editor, _clipboard),
				["paste"] = new PasteCommand(_editor, _clipboard),
				["undo"] = new UndoCommand(_editor, _history)
			};

			_editor.TextChanged += (s, e) => Console.WriteLine($"Text changed: {e.ChangeType}");
		}

		public async Task<bool> ExecuteCommandAsync(string commandName)
		{
			if (!_commands.ContainsKey(commandName))
				return false;

			var command = _commands[commandName];
			if (!command.CanExecute())
				return false;

			var startTime = DateTime.Now;
			var success = await command.ExecuteAsync();
			var executionTime = DateTime.Now - startTime;

			if (success && commandName != "undo" && commandName != "copy")
			{
				_history.Push(command);
			}

			CommandExecuted?.Invoke(this, new CommandExecutedEventArgs(command, success, executionTime));
			return success;
		}

		public void RegisterCommand(string name, ICommand command)
		{
			_commands[name] = command;
		}

		public ITextEditor GetEditor() => _editor;
		public ICommandHistory GetHistory() => _history;
	}


	class Program
	{
		static async Task Main(string[] args)
		{

			var app = new AdvancedTextEditorApplication();
			var editor = app.GetEditor();

			// Setup initial text and selection
			editor.InsertText("Hello, World!", 0);
			editor.Select(7, 5); // Select "World"

			Console.WriteLine($"Initial text: {editor.Content}");

			// Execute commands
			await app.ExecuteCommandAsync("copy");
			Console.WriteLine($"Final text: {editor.Content}");
			await app.ExecuteCommandAsync("cut");
			Console.WriteLine($"Final text: {editor.Content}");
			await app.ExecuteCommandAsync("paste");
			Console.WriteLine($"Final text: {editor.Content}");

			// Show command history
			var history = app.GetHistory();
			Console.WriteLine($"Command history: {history.Count} commands");

			foreach (var cmd in history.GetHistory())
			{
				Console.WriteLine($"- {cmd.Name}: {cmd.Description}");
			}
		}
	}
}