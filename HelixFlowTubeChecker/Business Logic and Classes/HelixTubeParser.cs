using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.ComponentModel;
using System.IO;


public static class HelixTubeParser
{

    public static DataInterface DataInterface { get; set; }

    public static BindingList<FlowTube> Parse(string filename)
    {

        int failureCount = 0;
        int sleepTime = 2000;
        bool success = false;
        Exception lastException = null;
        XDocument x = null;
        do
        {
            try
            {
                x = XDocument.Load(filename);
                success = true;
            }
            catch (Exception ex)
            {
                lastException = ex;
                System.Threading.Thread.Sleep(sleepTime);
                failureCount++;
            }
        }
        while (!success && failureCount < 10);

 
        if (!success) throw lastException;
    
        XNamespace ns = x.Root.GetDefaultNamespace();
        var persons = from data in x.Descendants(ns + "Person")
                      select new
                      {
                          PersonID = (string)data.Element(ns + "PersonId"),
                          MRN = (string)data.Element(ns + "MRN"),
                          Name = (string)data.Element(ns + "NameFullFormatted"),
                      };

        foreach (var p in persons)
            System.Diagnostics.Debug.WriteLine(p.ToString());

        var Persons = persons.ToDictionary(o => o.PersonID, o => o);

        var containers = from data in x.Descendants(ns + "Container")
                         select new
                         {
                             ContainerId = (string)data.Element(ns + "ContainerId"),
                             ContainerSuffix = (string)data.Element(ns + "ContainerSuffix"),
                             ContainerAccessionNumberFormatted = (string)data.Element(ns + "ContainerAccessionNumber").Element(ns + "Formatted"),
                             ContainerAccessionNumberUnformatted = (string)data.Element(ns + "ContainerAccessionNumber").Element(ns + "Unformatted"),
                             TubeLabel = (string)data.Element(ns + "SpecimenType").Element(ns + "Display"),
                         };

        var Containers = containers.ToDictionary(o => o.ContainerId, o => o);

        foreach (var c in containers)
            System.Diagnostics.Debug.WriteLine(c.ToString());

        var tubes = from data in x.Descendants(ns + "Cell")
                    select new
                    {
                        SeqNbr = (int)data.Element(ns + "SeqNbr"),
                        BatchItemId = (string)data.Element(ns + "BatchItemId"),
                    };

        foreach (var t in tubes)
            System.Diagnostics.Debug.WriteLine(t.ToString());

        var Tubes = tubes.ToDictionary(o => o.SeqNbr, o => o);

        var batches = from data in x.Descendants(ns + "BatchItem")
                      select new
                      {
                          BatchItemId = (string)data.Element(ns + "BatchItemId"),
                          ContainerId = (string)data.Element(ns + "ContainerId"),
                          OrderId = (string)data.Element(ns + "OrderId"),
                      };

        foreach (var b in batches)
            System.Diagnostics.Debug.WriteLine(b.ToString());

        var Batches = batches.ToDictionary(o => o.BatchItemId, o => o);

        var orders = from data in x.Descendants(ns + "Order")
                     select new
                     {
                         OrderId = (string)data.Element(ns + "OrderId"),
                         PersonId = (string)data.Element(ns + "PersonId"),
                     };

        foreach (var o in orders)
            System.Diagnostics.Debug.WriteLine(o.ToString());

        var Orders = orders.ToDictionary(o => o.OrderId, o => o);

        List<int> sequenceNumbers = Tubes.Keys.ToList();
        sequenceNumbers.Sort();
        BindingList<FlowTube> items = new BindingList<FlowTube>();


        foreach (int sequenceNumber in sequenceNumbers)
        {

            FlowTube item = new FlowTube();
            item.SeqNbr = sequenceNumber;
            item.MRN = Persons[Orders[Batches[Tubes[sequenceNumber].BatchItemId].OrderId].PersonId].MRN;
            item.Name = Persons[Orders[Batches[Tubes[sequenceNumber].BatchItemId].OrderId].PersonId].Name;
            item.AccessionNumber = Containers[Batches[Tubes[sequenceNumber].BatchItemId].ContainerId].ContainerAccessionNumberFormatted +
                Containers[Batches[Tubes[sequenceNumber].BatchItemId].ContainerId].ContainerSuffix;
            item.TubeLabel = Containers[Batches[Tubes[sequenceNumber].BatchItemId].ContainerId].TubeLabel;
            items.Add(item);
        }

        return items;

    }

    public static void WriteFileToDatabase(string filename)
    {


        FileInfo fi = new FileInfo(filename);
        BindingList<FlowTube> items = Parse(filename);



    }





}

