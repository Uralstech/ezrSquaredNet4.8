﻿using ezrSquared.General;
using static ezrSquared.Constants.constants;
using System;

namespace ezrSquared.Errors
{
    public abstract class error
    {
        internal string name;
        internal string details;

        public error(string name, string details)
        {
            this.name = name;
            this.details = details;
        }

        public virtual string asString() { return $"(error) {name}: {details}"; }
    }

    public abstract class positionBasedError : error
    {
        internal position startPos;
        internal position endPos;

        public positionBasedError(string name, string details, position startPos, position endPos) : base(name, details)
        {
            this.startPos = startPos;
            this.endPos = endPos;
        }

        public override string asString() { return $"{base.asString()} -> File '{startPos.file}', line {startPos.line + 1}\n{stringWithUnderline(startPos.text, startPos, endPos)}"; }

        internal string stringWithUnderline(string text, position startPos, position endPos)
        {
            int start = Math.Max(text.Substring(0, ((startPos.index <= text.Length) ? startPos.index : text.Length)).LastIndexOf('\n'), 0);
            int end = text.IndexOf('\n', start + 1);
            if (end == -1) end = text.Length;

            string result = text.Substring(start, end - start) + '\n';
            for (int i = 0; i < startPos.column; i++)
                result += ' ';
            for (int i = 0; i < endPos.column - startPos.column; i++)
                result += '~';
            return result.Replace('\t', ' ');
        }
    }

    public class unknownCharacterError : positionBasedError
    {
        public unknownCharacterError(string details, position startPos, position endPos) : base("Unknown character", details, startPos, endPos) { }
    }

    public class invalidGrammarError : positionBasedError
    {
        public invalidGrammarError(string details, position startPos, position endPos) : base("Invalid grammar", details, startPos, endPos) { }
    }

    public class overflowError : positionBasedError
    {
        public overflowError(string details, position startPos, position endPos) : base("Overflow", details, startPos, endPos) { }
    }

    public class runtimeError : positionBasedError
    {
        private context context;
        public runtimeError(position startPos, position endPos, string tag, string details, context context) : base(tag, details, startPos, endPos) { this.context = context; }

        public override string asString() { return $"{generateTraceback()}(runtime error) : {details} -> tag '{name}'\n\n{stringWithUnderline(startPos.text, startPos, endPos)}"; }

        internal string generateTraceback()
        {
            string result = "";
            position? pos = startPos;
            context? context = this.context;

            while (context != null)
            {
                result = $"\t File '{pos.file}', line {pos.line + 1} - In '{context.name}'\n{result}";
                pos = context.parentEntryPos;
                context = context.parent;
            }

            return $"Traceback - most recent call last:\n{result}";
        }
    }

    public class runtimeRunError : runtimeError
    {
        private string runError;
        public runtimeRunError(position startPos, position endPos, string details, string runError, context context) : base(startPos, endPos, RT_RUN, details, context) { this.runError = runError; }

        public override string asString() { return $"{generateTraceback()}(runtime error) : {details} -> tag '{name}'\n\n{runError}\n\n{stringWithUnderline(startPos.text, startPos, endPos)}"; }
    }

    public class interruptError : error
    {
        public interruptError() : base("Interrupt error", "Execution interrupted") { }
    }
}