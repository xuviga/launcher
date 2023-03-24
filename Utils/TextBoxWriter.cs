using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace iMine.Launcher.Utils
{
    public class TextBoxWriter : TextWriter
    {
        public override Encoding Encoding { get; } = Encoding.UTF8;

        private readonly TextBox textBox;

        public TextBoxWriter(TextBox textBox)
        {
            this.textBox = textBox;
        }


        public override void Write(string value)
        {
            textBox.Dispatcher.Invoke(new Action(delegate
            {
                textBox.AppendText(value);
                if (Math.Abs(textBox.VerticalOffset + textBox.ViewportHeight - textBox.ExtentHeight) < 4)
                    textBox.ScrollToEnd();
            }));
        }

        public override void WriteLine(string value)
        {
            textBox.Dispatcher.Invoke(new Action(delegate
            {
                textBox.AppendText(value + "\r\n");
                if (Math.Abs(textBox.VerticalOffset + textBox.ViewportHeight - textBox.ExtentHeight) < 4)
                    textBox.ScrollToEnd();
            }));
        }

        public override void Flush()
        {
        }

        public override void Close()
        {
        }
    }
}