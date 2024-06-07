using System.Collections.Generic;
using System.IO;
// using Unity.Plastic.Newtonsoft.Json;
// Written by: Andreas Sjögren Fürst (s201189)

public class PDFView : IPrintView
{
    PrintPresenter printPresenter;
    List<PrintViewModel> data;
    public void WriteDataToFile(string fileName)
    {
        // string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        // File.WriteAllText(fileName, jsonData);
    }

    public PDFView(PrintPresenter printPresenter)
    {
        data = new();
        this.printPresenter = printPresenter;
    }

    public void StoreCurrentData()
    {
        PrintViewModel printViewModel = printPresenter.PackageData();
        data.Add(printViewModel);
    }



}
