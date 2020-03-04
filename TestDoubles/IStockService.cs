namespace TestDoubles
{
    public interface IStockService
    {
        int GetStock(string productId, int quantityId);
        int GetStock(string productId);
    }
}
