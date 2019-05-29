using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Prism.Commands;
using Prism.Mvvm;

namespace Biavler
{
    

    class MainWindowViewModel: BindableBase
    {
        private ObservableCollection<VarroEntry> varroEntries;
        private string filter;
        private string filename = "";

        public MainWindowViewModel()
        {


            varroEntries = new ObservableCollection<VarroEntry>()
            {
                #if DEBUG
                new VarroEntry("1", DateTime.Now, 100, "Ingen Problemer"),
                new VarroEntry("2", DateTime.Now, 200, "Ingen Problemer") 
                #endif
            };
            CurrentEntry = null;
            
            EntryIds = new ObservableCollection<string>
            {
                "1",
                "2",
                "3",
                "4",
                "5"
            };

            filterEntryIds.Add("All");
            filterEntryIds.AddRange(EntryIds);
        }

        #region Properties

        public ObservableCollection<VarroEntry> VarroEntries
        {
            get { return varroEntries; }
            set { SetProperty(ref varroEntries, value); }
        }



        VarroEntry currentEntry = null;

        public VarroEntry CurrentEntry
        {
            get { return currentEntry; }
            set
            {
                SetProperty(ref currentEntry, value);
            }
        }

        int currentIndex = -1;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                SetProperty(ref currentIndex, value);
            }
        }

        ObservableCollection<string> entryIds;
        public ObservableCollection<string> EntryIds
        {
            get { return entryIds; }
            set
            {
                SetProperty(ref entryIds, value);
            }
        }


        private ObservableCollection<string> filterEntryIds = new ObservableCollection<string>();
        

        public ObservableCollection<string> FilterEntryIds
        {
            get
            {
                return filterEntryIds;
            }
            set { SetProperty(ref filterEntryIds, value); }
        }

        int currentEntryIdIndex = 0;

        public int CurrentEntryIdIndex
        {
            get { return currentEntryIdIndex; }
            set
            {
                if (currentEntryIdIndex != value)
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(VarroEntries);
                    currentEntryIdIndex = value;
                    if (currentEntryIdIndex == 0)
                        cv.Filter = null; // Index 0 is no filter (show all)
                    else
                    {
                        filter = EntryIds[(currentEntryIdIndex - 1)];
                        cv.Filter = CollectionViewSource_Filter;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        private bool CollectionViewSource_Filter(object en)
        {
            VarroEntry entry = en as VarroEntry;
            return (entry.BistadeId == filter);
        }


        #endregion


        #region Commands

        ICommand _newCommand;
        public ICommand AddNewEntryCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new DelegateCommand(() =>
                {
                    var newEntry = new VarroEntry();
                    VarroEntries.Add(newEntry);
                    CurrentEntry = newEntry;
                }));
            }
        }

        ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand =
                                                  new DelegateCommand(DeleteEntry, DeleteEntry_CanExecute)
                                                      .ObservesProperty(() => CurrentIndex));

        private void DeleteEntry()
        {
            VarroEntries.Remove(CurrentEntry);
            // RaisePropertyChanged("Count");
        }

        private bool DeleteEntry_CanExecute()
        {
            if (VarroEntries.Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }
        ICommand _newBistadCommand;

        public ICommand AddNewBistadCommand
        {
            get { return _newBistadCommand ?? (_newBistadCommand = new DelegateCommand(() =>
            {
                string newId = (EntryIds.Count+1).ToString();
                EntryIds.Add((newId));
                FilterEntryIds.Add(newId);
            })); }
        }


        ICommand _closeAppCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                return _closeAppCommand ?? (_closeAppCommand = new DelegateCommand(() =>
                {
                    App.Current.MainWindow.Close();
                }));
            }
        }

        ICommand _SaveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _SaveAsCommand ?? (_SaveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
        }

        private void SaveAsCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {
                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                filename = argFilename;
                SaveFileCommand_Execute();
            }
        }

        ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                  .ObservesProperty(() => VarroEntries.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            SaveData saveData = new SaveData();
            saveData.Entries = VarroEntries;
            saveData.EntryIds = EntryIds;
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            TextWriter writer = new StreamWriter(filename);
            // Serialize the savedata.
            serializer.Serialize(writer, saveData);
            writer.Close();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (VarroEntries.Count > 0);
        }

        ICommand _NewFileCommand;
        public ICommand NewFileCommand
        {
            get { return _NewFileCommand ?? (_NewFileCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                VarroEntries.Clear();
                EntryIds = new ObservableCollection<string>
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5"
                };
                filterEntryIds.Clear();
                filterEntryIds.Add("All");
                filterEntryIds.AddRange(EntryIds);
                filename = "";
            }
        }


        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {

                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                filename = argFilename;
                var tempSaveData = new SaveData();
 


                // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

                try
                {
                    TextReader reader = new StreamReader(filename);
                    // Deserialize all the saveData
                    tempSaveData = (SaveData)serializer.Deserialize(reader);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                VarroEntries = tempSaveData.Entries;
                EntryIds = tempSaveData.EntryIds;
                FilterEntryIds.Clear();
                FilterEntryIds.Add("All");
                FilterEntryIds.AddRange(EntryIds);
            }
        }

        #endregion
    }
}
