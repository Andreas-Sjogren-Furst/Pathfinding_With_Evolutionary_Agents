
public class PDFView : IPrintView
{
    PrintPresenter printPresenter;
    public void WriteDataToFile(string fileName)
    {
        throw new System.NotImplementedException();
    }

    public PDFView(PrintPresenter printPresenter){
        this.printPresenter = printPresenter;
    }

}
