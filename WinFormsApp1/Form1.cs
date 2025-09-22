namespace WinFormsApp1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Button b1 = new();
			b1.BackColor = Color.Black;
			b1.ForeColor = Color.Green;

			b1.Location = new Point(234, 123);

			Controls.Add(b1);


		}

		private void button1_Click(object sender, EventArgs e)
		{
			Console.WriteLine("полет нормальный");
		}

		private void button1ToolStripMenuItem_Click(object sender, EventArgs e)
		{


			Console.WriteLine("полет нормальный");
		}
	}
}
