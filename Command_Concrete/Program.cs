namespace Command_Concrete
{
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
		IEnumerable<ICommand> GetHestory();
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
		//void InsertText(string text, int position);
		//void DeleteText(int start, int length);
		//void ReplaceText(string text, int start, int length);
	}


		public class TextChangedEventArgs : EventArgs
		{
			public string OldText { get; }
			public string NewText { get; }

			public string ChageType { get; }
		
			public TextChangedEventArgs(string oldText, string newText, string changeType)
			{
				OldText = oldText;
				NewText = newText;
				ChageType = changeType;
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

		public string SelectedText => _selectionLength > 0 ? Content.Substring(_selectionStart, _selectionLength) : string.Empty;

		public int SelectionStart => _selectionStart;

		public int SelectionLength => _selectionLength;
		public event EventHandler<TextChangedEventArgs> TextChanged;

		public void Select(int start, int length)
		{
			_selectionStart = Math.Max(0, Math.Min(start, Content.Length));
			_selectionLength = Math.Max(0, Math.Min(length, Content.Length - _selectionStart));
		}

		//public void DeleteText(int start, int length)
		//{
		//	throw new NotImplementedException();
		//}

		//public void InsertText(string text, int position)
		//{
		//	throw new NotImplementedException();
		//}

		//public void ReplaceText(string text, int start, int length)
		//{
		//	throw new NotImplementedException();
		//}


	}


	public abstract class EditorCommang : ICommand
	{
		protected readonly ITextEditor Editor;
		protected string BackupContent;
		protected int BackupSelectionStart;
		protected int BackupSelectionLength;



		public abstract string Name { get; }

		public string Description => throw new NotImplementedException();

		public bool CanExecute()
		{
			throw new NotImplementedException();
		}

		public bool CanUndo()
		{
			throw new NotImplementedException();
		}

		public Task<bool> ExecuteAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> UndoAsync()
		{
			throw new NotImplementedException();
		}
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
		}
	}
}
