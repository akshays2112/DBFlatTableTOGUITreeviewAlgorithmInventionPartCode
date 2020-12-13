using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Node
        {
            public string Key { get; set; }
            public TreeViewItem ParentTvi { get; set; }

            public Node(string key, TreeViewItem parentTvi)
            {
                Key = key;
                ParentTvi = parentTvi;
            }
        }

        TreeViewItem parentTvi = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            WpfApplication1.TreeAlgorithmDataSet treeAlgorithmDataSet = ((WpfApplication1.TreeAlgorithmDataSet)(this.FindResource("treeAlgorithmDataSet")));
            // Load data into the table TreeData. You can modify this code as needed.
            WpfApplication1.TreeAlgorithmDataSetTableAdapters.TreeDataTableAdapter treeAlgorithmDataSetTreeDataTableAdapter = new WpfApplication1.TreeAlgorithmDataSetTableAdapters.TreeDataTableAdapter();
            treeAlgorithmDataSetTreeDataTableAdapter.Fill(treeAlgorithmDataSet.TreeData);
            System.Windows.Data.CollectionViewSource treeDataViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("treeDataViewSource")));
            treeDataViewSource.View.MoveCurrentToFirst();
            DataTable dt = treeAlgorithmDataSet.Tables["TreeData"];

            //Insert Code Begins
            var insertquery = from keydata in dt.AsEnumerable()
                              where keydata.Field<string>("Key").StartsWith("A.")
                              orderby
                              keydata.Field<string>("Key").IndexOf("A.") > -1 ?
                              keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).IndexOf(".") > 0 ?
                              keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).Substring(0,
                              keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).IndexOf(".") - 1) :
                              keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length) : null
                              select new
                              {
                                  FullKey = keydata.Field<string>("Key"),
                                  Key = keydata.Field<string>("Key").IndexOf("A.") > -1 ?
                                        keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).IndexOf(".") > 0 ?
                                        keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).Substring(0,
                                        keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length).IndexOf(".") - 1) :
                                        keydata.Field<string>("Key").Substring("A.".Length, keydata.Field<string>("Key").Length - "A.".Length) : null
                              };
            string newinsertkey = insertquery.Count() == 0 ? "A.A" : (insertquery.Last().Key.Last<char>() == 'Z' ? insertquery.Last().FullKey + "A" :
                insertquery.Last().FullKey.Substring(0, insertquery.Last().FullKey.Length - 1) + ((char)((int)insertquery.Last().Key.Last<char>() + 1)).ToString());
            DataRow dr = dt.NewRow();
            dr.SetField<string>(dt.Columns["Key"], newinsertkey);
            dr.SetField<string>(dt.Columns["Data"], "Fritjof Capra");
            dt.Rows.Add(dr);
            dt.AcceptChanges();
            //Insert code ends

            //Select or Read code begins
            EnumerableRowCollection<DataRow> query = from keydata in dt.AsEnumerable()
                                                     orderby keydata.Field<string>("Key")
                                                     select keydata;
            foreach (DataRow tdr in query)
            {
                string key = tdr.Field<string>("Key").ToString();
                TreeViewItem tvi = new TreeViewItem();
                Label lbl = new Label();
                lbl.Content = key + "    " + tdr.Field<string>("Data").ToString();
                tvi.Items.Add(lbl);
                tvi.Tag = new Node(key, parentTvi);
                if (parentTvi == null || key.IndexOf('.') == 0)
                {
                    parentTvi = tvi;
                    tvi.Tag = new Node(key, null);
                    treeView.Items.Add(tvi);
                }
                else
                {
                    bool found = false;
                    while (parentTvi != null && key.Contains(".") && !found)
                    {
                        found = Loop(parentTvi, tvi, key);
                        if (found)
                        {
                            parentTvi = tvi;
                        }
                    }
                    if (!found)
                    {
                        parentTvi = tvi;
                        tvi.Tag = new Node(key, null);
                        treeView.Items.Add(tvi);
                    }
                }
            }
        }

        bool Loop(TreeViewItem parentTvi2, TreeViewItem tvi, string key)
        {
            bool found = false;
            string parentTagKey = ((Node)parentTvi2.Tag).Key;
            if (key.Contains(parentTagKey + "."))
            {
                tvi.Tag = new Node(key, parentTvi2);
                parentTvi2.Items.Add(tvi);
                parentTvi2 = tvi;
                found = true;
            }
            else
            {
                parentTvi = ((Node)parentTvi2.Tag).ParentTvi;
            }
            return found;
        }
        //Select or Read code ends
    }
}

