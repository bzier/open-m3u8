using System;
using System.IO;
using System.Text;

namespace M3U8Parser
{
    public class M3uParser : BaseM3uParser
    {
        public M3uParser(Stream inputStream, Encoding encoding) : base(inputStream, encoding) { }

        public override Playlist parse()
        {
            validateAvailable();

            ParseState state = new ParseState(mEncoding);
            TrackLineParser trackLineParser = new TrackLineParser();

            try
            {
                state.setMedia();

                while (mScanner.hasNext())
                {
                    String line = mScanner.next();
                    validateLine(line);

                    if (line.Length == 0 || isComment(line))
                    {
                        continue;
                    }
                    else
                    {
                        trackLineParser.parse(line, state);
                    }
                }

                Playlist playlist = new Playlist.Builder()
                        .withMediaPlaylist(new MediaPlaylist.Builder()
                                .withTracks(state.getMedia().tracks)
                                .build())
                        .build();

                PlaylistValidation validation = PlaylistValidation.from(playlist);

                if (validation.isValid())
                {
                    return playlist;
                }
                else
                {
                    throw new PlaylistException(mScanner.getInput(), validation.getErrors());
                }
            }
            catch (ParseException exception)
            {
                exception.setInput(mScanner.getInput());
                throw exception;
            }
        }

        private void validateLine(String line)
        {
            if (!isComment(line))
            {
                if (line.Length != line.Trim().Length)
                {
                    throw ParseException.create(ParseExceptionType.WHITESPACE_IN_TRACK, line, "" + line.Length);
                }
            }
        }

        private bool isComment(String line)
        {
            return line.IndexOf(Constants.COMMENT_PREFIX) == 0;
        }
    }
}
