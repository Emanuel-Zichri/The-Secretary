namespace FinalProject.BL
{
    public class PriceCalculatorItem
    {
        public int CalculatorItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public static int AddCalcItem(PriceCalculatorItem newItem)
        {
            DBservices dBservices = new DBservices();
            int newItemId = dBservices.AddCalcItem(newItem);
            return newItemId;
        }
        public static int DeleteCalcItem(PriceCalculatorItem itemID)
        {
            DBservices dBservices = new DBservices();
            int result = dBservices.DeleteCalcItem(itemID);
            return result;
        }
        public static int UpdateCalcItem(PriceCalculatorItem item)
        {
            DBservices dBservices = new DBservices();
            int result = dBservices.UpdateCalcItem(item);
            return result;
        }
    }

}
