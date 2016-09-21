using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

 
 
   public class FlowTubeList
        {
            public FlowTubeList(string path, bool FileSystemBound, DataInterface di)
            {
                this.path = path;
                this.FileSystemBound = FileSystemBound;
                if (FileSystemBound)
                {
                    this.file_data = File.ReadAllText(path);
                    this.filename = Path.GetFileNameWithoutExtension(path);
                    FileInfo fi = new FileInfo(path);
                    this.directory = fi.Directory.FullName;
                    this.containing_directory = fi.Directory.Name.ToString();
                    this.date_created = fi.CreationTime;
                    try
                    {
                        _FlowTubes = HelixTubeParser.Parse(path);
                        is_valid = (_FlowTubes.Count != 0);
                    }
                    catch (Exception ex)
                    {
                        _FlowTubes = new BindingList<FlowTube>();
                        exception = ex;
                        is_valid = false; }
                }
                this.DataInterface = di;
            }

            public DataInterface DataInterface {get;private set;}
            private BindingList<FlowTube> _FlowTubes;
            public int flow_tube_list_id { get; set; }
            public string filename { get; set; }
            public string file_data { get; set; }
            public bool FileSystemBound { get; private set; }
            public bool is_valid { get; set; }
            public string file_hash { get { return OncoSeek.Core.Shared.GetSHAHash(file_data); } }
            public bool is_newly_uploaded { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_parsed { get; set; }
            public DateTime? date_verified { get; set; }
            public string verified_by { get; set; }
            public string first_tube_barcode { get; set; }
            public string path { get; set; }
            public string directory { get; set; }
            public string containing_directory { get; set; }
            public Exception exception { get; set; }

           [Browsable(false)]
            public BindingList<FlowTube> FlowTubes
            {

                get
                {
                    if (_FlowTubes == null && !FileSystemBound) _FlowTubes = this.DataInterface.GetFlowTubes(this.flow_tube_list_id);
                    return _FlowTubes;
                }
            }


        }




