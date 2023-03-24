using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace iMine.Launcher.Utils
{
    public class SuperTextWriter : TextWriter
    {
        public override Encoding Encoding { get; } = Encoding.UTF8;
        private readonly List<TextWriter> writers = new List<TextWriter>();
        private StringBuilder buffer = new StringBuilder();
        private StringBuilder bufferSwap = new StringBuilder();
        private readonly Thread timer;

        public SuperTextWriter(IEnumerable<TextWriter> writerz, Encoding encoding = null)
        {
            if (encoding != null)
                Encoding = encoding;

            foreach (var writer in writerz)
            {
                if (writer != null)
                    writers.Add(writer);
            }

            timer = new Thread(Process);
            timer.Start();
        }


        public override void Write(string value)
        {
            buffer.Append(value);
        }

        public override void WriteLine(string value)
        {
            buffer.Append(value).Append('\n');
        }

        /*public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }*/

        public override void Close()
        {
            foreach (var writer in writers)
                writer.Close();
        }

        public new void Dispose()
        {
            foreach (var writer in writers)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            timer.Interrupt();
            base.Dispose();
        }

        private void Process()
        {
            try
            {
                for (;;)
                {
                    Thread.Sleep(100);
                    if (buffer.Length<=0)
                        continue;
                    var tmp = buffer;
                    buffer = bufferSwap;
                    bufferSwap = tmp;
                    var data = tmp.ToString();
                    tmp.Clear();
                    foreach (var writer in writers)
                        writer.Write(data);
                }
            }
            catch
            {
            }
        }
    }
}