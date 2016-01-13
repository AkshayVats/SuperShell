﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;
using SuperShell.Bridge.Core;

namespace ICSharpCode.CodeCompletion
{
    public class CodeTextEditor : AvalonEdit.TextEditor, SuperShell.Bridge.Ui.IShellInputControl
    {
        protected CompletionWindow completionWindow;
        protected OverloadInsightWindow insightWindow;

        
        public CodeTextEditor()
        {
            FontFamily = new System.Windows.Media.FontFamily("Consolas");
            SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
            HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Disabled;
            PreviewKeyDown += CodeTextEditor_PreviewKeyDown;

            TextArea.TextEntering += OnTextEntering;
            TextArea.TextEntered += OnTextEntered;

            var ctrlSpace = new RoutedCommand();
            ctrlSpace.InputGestures.Add(new KeyGesture(Key.Space, ModifierKeys.Control));
            var cb = new CommandBinding(ctrlSpace, OnCtrlSpaceCommand);

            this.CommandBindings.Add(cb);
        }

        private void CodeTextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (completionWindow == null)
            {
                if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
                {
                    CommandEntered?.Invoke(this, Text);
                    e.Handled = true;
                    _lastCommands.Add(Text);
                }
                else if(e.Key == Key.Up)
                {
                    Clear();
                    AppendText(_lastCommands.Previous());
                }
                else if(e.Key == Key.Down)
                {
                    Clear();
                    AppendText(_lastCommands.Next());
                }
                else
                {
                    _lastCommands.Reset();
                }
            }
        }

        public CSharpCompletion Completion { get; set; }

        #region Open & Save File
        public string FileName { get; set; }

        
        public void OpenFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            if (completionWindow != null)
                completionWindow.Close();
            if (insightWindow != null)
                insightWindow.Close();

            FileName = fileName;
            Load(fileName);
            Document.FileName = FileName;

            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
        }

        public bool SaveFile()
        {
            if (String.IsNullOrEmpty(FileName))
                return false;

            Save(FileName);
            return true;
        }
        #endregion


        #region Code Completion
        private void OnTextEntered(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            ShowCompletion(textCompositionEventArgs.Text, false);
        }

        private void OnCtrlSpaceCommand(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            ShowCompletion(null, true);
        }

        private void ShowCompletion(string enteredText, bool controlSpace)
        {
            if (!controlSpace)
                Debug.WriteLine("Code Completion: TextEntered: " + enteredText);
            else
                Debug.WriteLine("Code Completion: Ctrl+Space");

            //only process csharp files and if there is a code completion engine available
            if (String.IsNullOrEmpty(FileName))
            {
                Debug.WriteLine("No document file name, cannot run code completion");
                return;
            }


            if (Completion == null)
            {
                Debug.WriteLine("Code completion is null, cannot run code completion");
                return;
            }

            var fileExtension = Path.GetExtension(FileName);
            fileExtension = fileExtension != null ? fileExtension.ToLower() : null;
            //check file extension to be a c# file (.cs, .csx, etc.)
            if (fileExtension == null || (!fileExtension.StartsWith(".cs")))
            {
                Debug.WriteLine("Wrong file extension, cannot run code completion");
                return;
            }

            if (completionWindow == null)
            {
                CodeCompletionResult results = null;
                try
                {
                    var offset = 0;
                    var doc = GetCompletionDocument(out offset);
                    results = Completion.GetCompletions(doc, offset, controlSpace);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Error in getting completion: " + exception);
                }
                if (results == null)
                    return;

                if (insightWindow == null && results.OverloadProvider != null)
                {
                    insightWindow = new OverloadInsightWindow(TextArea);
                    insightWindow.Provider = results.OverloadProvider;
                    insightWindow.Show();
                    insightWindow.Closed += (o, args) => insightWindow = null;
                    return;
                }

                if (completionWindow == null && results != null && results.CompletionData.Any())
                {
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(TextArea);
                    completionWindow.CloseWhenCaretAtBeginning = controlSpace;
                    completionWindow.StartOffset -= results.TriggerWordLength;
                    //completionWindow.EndOffset -= results.TriggerWordLength;

                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (var completion in results.CompletionData.OrderBy(item => item.Text))
                    {
                        data.Add(completion);
                    }
                    if (results.TriggerWordLength > 0)
                    {
                        //completionWindow.CompletionList.IsFiltering = false;
                        completionWindow.CompletionList.SelectItem(results.TriggerWord);
                    }
                    completionWindow.Show();
                    completionWindow.Closed += (o, args) => completionWindow = null;
                }
            }//end if

            //update the insight window
            if (!string.IsNullOrEmpty(enteredText) && insightWindow != null)
            {
                //whenver text is entered update the provider
                var provider = insightWindow.Provider as CSharpOverloadProvider;
                if (provider != null)
                {
                    //since the text has not been added yet we need to tread it as if the char has already been inserted
                    var offset = 0;
                    var doc = GetCompletionDocument(out offset);
                    provider.Update(doc, offset);
                    //if the windows is requested to be closed we do it here
                    if (provider.RequestClose)
                    {
                        insightWindow.Close();
                        insightWindow = null;
                    }
                }
            }
        }//end method

        private void OnTextEntering(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            Debug.WriteLine("TextEntering: " + textCompositionEventArgs.Text);
            if (textCompositionEventArgs.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(textCompositionEventArgs.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(textCompositionEventArgs);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        /// <summary>
        /// Gets the document used for code completion, can be overridden to provide a custom document
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>The document of this text editor.</returns>
        protected virtual IDocument GetCompletionDocument(out int offset)
        {
            offset = CaretOffset;
            return new ReadOnlyDocument(new StringTextSource(Text/*.Substring(0, offset)*/), FileName);
        }




        #endregion

        #region IShellInputControl

        private ICommandHistoryManager _lastCommands;
        public event EventHandler<string> CommandEntered;
        private bool? _evaluating;
        public bool? Evaluating
        {
            get
            {
                return _evaluating;
            }

            set
            {
                IsReadOnly = false;
                _evaluating = value;
                if (_evaluating == null)
                    ;// Style = FindResource("shell_active") as Style;
                else if (_evaluating == true)
                {
                    IsReadOnly = true;
                    //Style = FindResource("shell_evaluating") as Style;
                }
                else;// Style = FindResource("shell_inactive") as Style;
            }
        }

        public Control Control
        {
            get
            {
                return this;
            }
        }
        public void SetCommandHistoryManager(ICommandHistoryManager manager)
        {
            _lastCommands = manager;
        }

        #endregion
    }
}
