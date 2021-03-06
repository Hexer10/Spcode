﻿using System.Collections.Generic;
using SourcepawnCondenser.SourcemodDefinition;
using SourcepawnCondenser.Tokenizer;

namespace SourcepawnCondenser
{
    public partial class Condenser
    {
        private int ConsumeSMEnum()
        {
            var startIndex = t[position].Index;
            if ((position + 1) < length)
            {
                var iteratePosition = position;
                var enumName = string.Empty;
                while ((iteratePosition + 1) < length && t[iteratePosition].Kind != TokenKind.BraceOpen)
                {
                    if (t[iteratePosition].Kind == TokenKind.Identifier)
                    {
                        enumName = t[iteratePosition].Value;
                    }
                    ++iteratePosition;
                }
                var braceState = 0;
                var inIgnoreMode = false;
                var endTokenIndex = -1;
                var entries = new List<string>();
                for (; iteratePosition < length; ++iteratePosition)
                {
                    if (t[iteratePosition].Kind == TokenKind.BraceOpen)
                    {
                        ++braceState;
                        continue;
                    }
                    if (t[iteratePosition].Kind == TokenKind.BraceClose)
                    {
                        --braceState;
                        if (braceState == 0)
                        {
                            endTokenIndex = iteratePosition;
                            break;
                        }
                        continue;
                    }
                    if (inIgnoreMode)
                    {
                        if (t[iteratePosition].Kind == TokenKind.Comma)
                        {
                            inIgnoreMode = false;
                        }
                        continue;
                    }
                    if (t[iteratePosition].Kind == TokenKind.Identifier)
                    {
                        entries.Add(t[iteratePosition].Value);
                        inIgnoreMode = true;
                    }
                }
                if (endTokenIndex == -1)
                {
                    return -1;
                }
                def.Enums.Add(new SMEnum()
                {
                    Index = startIndex,
                    Length = t[endTokenIndex].Index - startIndex + 1,
                    File = FileName,
                    Entries = entries.ToArray(),
                    Name = enumName
                });
                return endTokenIndex;
            }
            return -1;
        }
    }
}
