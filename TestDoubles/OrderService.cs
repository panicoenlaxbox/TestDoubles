namespace TestDoubles
{
    public class OrderService
    {
        private readonly IStockService _stockService;

        public OrderService(IStockService stockService)
        {
            this._stockService = stockService;
        }

        public void Place(string productId, int quantityId)
        {
            _stockService.GetStock(productId, quantityId);
            //_stockService.GetStock(productId);
        }
    }
}
