using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Collections.Generic;


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

            string extension = Path.GetExtension(path);
            if (extension.ToUpper() == ".XLSX")
            {
                _FlowTubes = new BindingList<FlowTube>();

                try
                {
                    DataTable dt = OncoSeek.Core.Shared.ImportExceltoDatatable(path);
                    int sequenceNumber = 1;
                    Dictionary<int, List<FlowTube>> tubes = new Dictionary<int, List<FlowTube>>();
                    foreach (DataRow row in dt.Rows)
                    {
                        int batchNumber = Convert.ToInt32(row["Batch Position"]);
                        if (!tubes.ContainsKey(batchNumber))
                            tubes[batchNumber] = new List<FlowTube>();
                        FlowTube item = new FlowTube();
                        item.MRN = row["MRN"].ToString();
                        item.Name = row["Patient Name"].ToString();
                        item.AccessionNumber = row["Container"].ToString();
                        item.TubeLabel = row["Task"].ToString();
                        tubes[batchNumber].Add(item);
                    }

                    List<int> batchNumbers = new List<int>(tubes.Keys);
                    batchNumbers.Sort();
                    foreach (int batchNumber in batchNumbers)
                    {
                        foreach (var tube in tubes[batchNumber])
                        {
                            tube.SeqNbr = sequenceNumber;
                            _FlowTubes.Add(tube);
                            sequenceNumber++;
                        } 
                    }

                    is_valid = (_FlowTubes.Count != 0);
                }
                catch (Exception ex)
                {
                    _FlowTubes = new BindingList<FlowTube>();
                    exception = ex;
                    is_valid = false;

                }


            }

            else

            {
                try
                {
                    _FlowTubes = HelixTubeParser.Parse(path);
                    is_valid = (_FlowTubes.Count != 0);
                }
                catch (Exception ex)
                {
                    _FlowTubes = new BindingList<FlowTube>();
                    exception = ex;
                    is_valid = false;
                }
            }
        }
        this.DataInterface = di;
    }

    public DataInterface DataInterface { get; private set; }
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




