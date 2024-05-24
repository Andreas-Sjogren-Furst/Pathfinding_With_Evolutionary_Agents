
public class PDFView : IPrintView
{
    PrintPresenter PrintPresenter;
    public void WriteDataToFile(string fileName)
    {
        throw new System.NotImplementedException();
    }

    public PDFView(PrintPresenter printPresenter){
        this.PrintPresenter = printPresenter;
    }

}
