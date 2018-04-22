namespace PGA.Model.BLL
{
    public interface IModel
    {
        // LayerStatesBLL GetPathDataFromRegistry();

        void WriteDataToRegistry(string Key, string Val);

        LayerStatesBLL GetDataFromRegistry();
    }
}