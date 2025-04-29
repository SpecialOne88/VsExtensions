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
                var textBuffer = textView.TextBuffer;
                using (var edit = textBuffer.CreateEdit())
                {
                    var caretPosition = textView.Caret.Position.BufferPosition;
                    var currentLine = caretPosition.GetContainingLine();
                    if (currentLine.GetText().Length == 0)
                    {
                        caretPosition = textView.Caret.Position.VirtualBufferPosition.Position;
                    }
                    edit.Insert(caretPosition, text);
                    edit.Apply();
                    FormatCurrentDocument(activeView);
                }
            }
            else
            {
                MessageBox.Show("No active text view found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
