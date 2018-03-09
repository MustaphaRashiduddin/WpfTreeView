using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WpfTreeView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default constructor, where treeview is first initialized
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            var t = new TreeViewItem();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            //GetAnimationBaseValue every logical drive on the machine
            foreach (var drive in Directory.GetLogicalDrives()) {
                //BitmapCreateOptions a new item for it
                var item = new TreeViewItem();
                item.Header = drive;
                item.Tag = drive;

                //AddChild a dummy item
                item.Items.Add(null);

                //Listen out for item being expanded
                item.Expanded += Folder_Expanded;

                //AddChild it to the main tree-view
                FolderView.Items.Add(item);
            }
        }

        /// <summary>
        /// When a folder is expanded, find the sub folders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Folder_Expanded(object sender, RoutedEventArgs e) {
            #region Initial Checks
            var item = (TreeViewItem)sender;

            // If the item doesnt contain dummy data
            if (item.Items[0] != null || item.Items.Count != 1)
                return;

            // Clear dummy data
            item.Items.Clear();

            // Get full path
            var fullPath = (string)item.Tag;
            #endregion

            #region Get Folders
            // Create a blank list for directories
            var directories = new List<string>();

            // Try and get directories from the folder
            // ignoring any issues doing so
            try {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            } catch { }

            directories.ForEach(directoryPath => {

                var subItem = new TreeViewItem();
                subItem.Header = GetFileFolderName(directoryPath);
                subItem.Tag = directoryPath;

                // Add dummy item so we can expand folder
                subItem.Items.Add(null);

                // Handle expanding
                subItem.Expanded += Folder_Expanded;

                // Add this item to the parent
                item.Items.Add(subItem);
            });
            #endregion
          
            #region Get Files
            // Create a blank list for files
            var files = new List<string>();

            // Try and get files from the folder
            // ignoring any issues doing so
            try {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            } catch { }

            //for each file
            files.ForEach(filePath => {

                // Create file item
                var subItem = new TreeViewItem();
                // set header as file name
                subItem.Header = GetFileFolderName(filePath);
                //Add tag as full path
                subItem.Tag = filePath;

                // Add this item to the parent
                item.Items.Add(subItem);
            });
            #endregion
        }

        /// <summary>
        /// give full path get name of folder or file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path) {

            // If we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            //make all slashes back slashed
            var normalizedPath = path.Replace('/', '\\');

            // Foind the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don't find a backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last backslash
            return path.Substring(lastIndex + 1);
        }

    }
}
