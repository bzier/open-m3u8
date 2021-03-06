using System;
using System.IO;
using System.Text;

namespace M3U8Parser
{
    public abstract class Writer
    {
        protected readonly TagWriter tagWriter;

        public Writer(Stream outputStream, Encoding encoding)
        {
            try
            {
                tagWriter = new TagWriter(new StreamWriter(outputStream, encoding.getValue()));
            }
            catch (Exception e) // TODO: Was UnsupportedEncodingException. c# equivalent?
            {
                throw new ArgumentException(e.Message, e);
            }
        }

        public void writeTagLine(String tag)
        {
            writeLine(Constants.COMMENT_PREFIX + tag);
        }

        public void writeTagLine(String tag, Object value)
        {
            writeLine(Constants.COMMENT_PREFIX + tag + Constants.EXT_TAG_END + value);
        }

        public void writeLine(String line)
        {
            tagWriter.write(line);
            tagWriter.write("\n");
        }

        public void write(Playlist playlist)
        {
            doWrite(playlist);

            tagWriter.flush();
        }

        public abstract void doWrite(Playlist playlist);
    }
}
