using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Windows;

namespace PowerMacros.Utils
{
    public static class TextEditor
    {
        public static void InsertTextInCurrentView(string text)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var (textView, activeView) = GetActiveTextView();

            if (textView != null)
            {
                var caretPosition = textView.Caret.Position.BufferPosition;
                var caretLine = caretPosition.GetContainingLine();
                var isLineEmpty = caretLine.GetText().Length == 0;
                var lineNumber = caretLine.LineNumber;

                // When the current line is empty, the virtual buffer position works better
                if (isLineEmpty)
                {
                    caretPosition = textView.Caret.Position.VirtualBufferPosition.Position;
                    lineNumber = caretPosition.GetContainingLine().LineNumber;
                }

                double originalPositionX = isLineEmpty ? 0d : textView.Caret.Left;

                int xOffset = 0;
                int yOffset = 0;
                bool moveCaret = false;
                int numberOfLines = 1;

                // Check for $end$ marker in the text and calculate caret offsets
                if (text.IndexOf("$end$") >= 0)
                {
                    moveCaret = true;
                    var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    numberOfLines = lines.Length;
                    foreach (var line in lines)
                    {
                        if (line.Contains("$end$"))
                        {
                            xOffset = line.IndexOf("$end$");
                            break;
                        }
                        yOffset++;
                    }
                    text = text.Replace("$end$", "");
                }

                var textBuffer = textView.TextBuffer;
                using (var edit = textBuffer.CreateEdit())
                {
                    edit.Insert(caretPosition, text);
                    edit.Apply();
                }

                // Move the caret to the specified position if $end$ marker was found
                if (moveCaret)
                {
                    if (numberOfLines > 1)
                    {
                        MoveCaretToLine(lineNumber + yOffset, textView);
                        // When pasting multiple lines, only use the original position if the marker is in the first line
                        MoveCaretToCharacterPosition(xOffset, textView, yOffset > 0 ? 0d : originalPositionX);
                    }
                    else
                    {
                        MoveCaretToCharacterPosition(xOffset, textView, originalPositionX);
                    }                        
                }

                textView.Caret.EnsureVisible();
                // Auto format the document like a real paste operation
                FormatCurrentDocument(activeView);
            }
            else
            {
                MessageBox.Show("No active text view found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void MoveCaretToCharacterPosition(int characterPosition, Microsoft.VisualStudio.Text.Editor.ITextView textView, double originalPositionX = 0d)
        {
            if (textView != null)
            {
                var currentLine = textView.Caret.ContainingTextViewLine;

                if (currentLine != null)
                {
                    characterPosition = Math.Min(characterPosition, currentLine.Length);
                    var bufferPosition = currentLine.Start + characterPosition;
                    var bounds = currentLine.GetCharacterBounds(bufferPosition);

                    textView.Caret.MoveTo(textView.Caret.ContainingTextViewLine, originalPositionX + bounds.Left);
                }
            }
        }

        private static void MoveCaretToLine(int line, Microsoft.VisualStudio.Text.Editor.ITextView textView)
        {
            if (textView != null)
            {
                var currentSnapshot = textView.TextSnapshot;

                int targetLineNumber = line;
                targetLineNumber = Math.Max(0, Math.Min(targetLineNumber, currentSnapshot.LineCount - 1));

                var targetLine = currentSnapshot.GetLineFromLineNumber(targetLineNumber);
                int targetPosition = targetLine.Start;

                var targetSnapshotPoint = new Microsoft.VisualStudio.Text.SnapshotPoint(currentSnapshot, targetPosition);
                textView.Caret.MoveTo(targetSnapshotPoint);
            }
        }

        public static (Microsoft.VisualStudio.Text.Editor.ITextView, IVsTextView) GetActiveTextView()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            var textManager = (IVsTextManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsTextManager));

            // Activate the current document in the IDE
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            dte2.ActiveDocument?.Activate();

            if (componentModel != null && textManager != null)
            {
                textManager.GetActiveView(1, null, out IVsTextView activeView);

                if (activeView != null)
                {
                    var adapterService = componentModel.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();
                    var textView = adapterService.GetWpfTextView(activeView);
                    if (textView != null && textView.TextBuffer != null)
                    {
                        return (textView, activeView);
                    }
                }
            }

            return (null, null);
        }

        private static void FormatCurrentDocument(IVsTextView activeView)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            // Get the command target for the active view
            if (activeView != null && activeView is Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget commandTarget)
            {
                // Define the "Format Document" command
                var cmdGroup = Microsoft.VisualStudio.VSConstants.VSStd2K;
                var cmdID = (uint)Microsoft.VisualStudio.VSConstants.VSStd2KCmdID.FORMATDOCUMENT;

                // Execute the command
                var cmdExecOpt = Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT;
                commandTarget.Exec(ref cmdGroup, cmdID, (uint)cmdExecOpt, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
