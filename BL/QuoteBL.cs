namespace FinalProject.BL
{
    public class QuoteBL
    {
        private readonly DBservices _db;

        public QuoteBL()
        {
            _db = new DBservices();
        }

        /// <summary>
        /// יצירת הצעת מחיר חדשה
        /// </summary>
        public QuoteCreationResult CreateQuote(CreateQuoteRequest request)
        {
            try
            {
                Console.WriteLine($"🔄 יוצר הצעת מחיר חדשה עבור RequestID: {request.RequestID}");

                // שלב 1: חישוב הצעת המחיר
                var calculatedQuote = CalculateQuote(request);
                if (calculatedQuote == null)
                {
                    return new QuoteCreationResult 
                    { 
                        Success = false, 
                        Message = "שגיאה בחישוב הצעת המחיר" 
                    };
                }

                // שלב 2: שמירת ההצעה הראשית
                var quote = new Quote
                {
                    RequestID = request.RequestID,
                    TotalPrice = calculatedQuote.FinalTotal,
                    DiscountAmount = calculatedQuote.DiscountAmount,
                    DiscountPercent = calculatedQuote.DiscountPercent
                };

                int quoteID = _db.InsertQuote(quote);
                if (quoteID <= 0)
                {
                    return new QuoteCreationResult 
                    { 
                        Success = false, 
                        Message = "שגיאה בשמירת הצעת המחיר" 
                    };
                }

                Console.WriteLine($"✅ הצעת מחיר נשמרה בהצלחה, QuoteID: {quoteID}");

                // שלב 3: שמירת פריטי ההצעה
                int savedItems = 0;
                foreach (var calcItem in calculatedQuote.Items)
                {
                    var quoteItem = new QuoteItem
                    {
                        QuoteID = quoteID,
                        CalculatorItemID = calcItem.CalculatorItemID,
                        CustomItemName = calcItem.ItemName,
                        PriceForItem = calcItem.UnitPrice,
                        Quantity = calcItem.Quantity,
                        FinalPrice = calcItem.TotalPrice,
                        Notes = calcItem.Notes
                    };

                    int itemResult = _db.AddQuoteItem(quoteItem);
                    if (itemResult > 0) savedItems++;
                }

                Console.WriteLine($"✅ נשמרו {savedItems} פריטים מתוך {calculatedQuote.Items.Count}");

                return new QuoteCreationResult
                {
                    Success = true,
                    Message = $"הצעת מחיר נשמרה בהצלחה. QuoteID: {quoteID}",
                    QuoteID = quoteID,
                    Quote = calculatedQuote
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה ביצירת הצעת מחיר: {ex.Message}");
                return new QuoteCreationResult 
                { 
                    Success = false, 
                    Message = $"שגיאה: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// חישוב הצעת מחיר
        /// </summary>
        public CalculatedQuote CalculateQuote(CreateQuoteRequest request)
        {
            try
            {
                Console.WriteLine($"🧮 מחשב הצעת מחיר עבור RequestID: {request.RequestID}");

                var result = new CalculatedQuote();
                decimal subtotal = 0;

                // שלב 1: חישוב מחיר פרקט בסיסי (אם נדרש)
                if (request.IncludeBaseFloor && request.TotalArea > 0 && request.FloorPricePerMeter > 0)
                {
                    decimal floorTotal = request.TotalArea * request.FloorPricePerMeter;
                    subtotal += floorTotal;
                    
                    result.Items.Add(new CalculatedQuoteItem
                    {
                        ItemName = $"התקנת פרקט {request.FloorType}",
                        UnitPrice = request.FloorPricePerMeter,
                        Quantity = 1,
                        TotalPrice = floorTotal,
                        Notes = $"{request.TotalArea} מ\"ר × {request.FloorPricePerMeter} ש\"ח"
                    });
                }

                // שלב 2: הוספת פריטים נוספים
                if (request.Items != null && request.Items.Any())
                {
                    foreach (var item in request.Items.Where(i => i.Quantity > 0))
                    {
                        decimal itemTotal = item.UnitPrice * item.Quantity;
                        subtotal += itemTotal;

                        result.Items.Add(new CalculatedQuoteItem
                        {
                            CalculatorItemID = item.CalculatorItemID,
                            ItemName = item.ItemName,
                            UnitPrice = item.UnitPrice,
                            Quantity = item.Quantity,
                            TotalPrice = itemTotal,
                            Notes = item.Notes
                        });
                    }
                }

                // שלב 3: חישוב הנחה
                result.Subtotal = subtotal;
                result.DiscountPercent = request.DiscountPercent ?? 0;
                result.DiscountAmount = subtotal * (result.DiscountPercent / 100);
                result.AfterDiscount = subtotal - result.DiscountAmount;

                            // שלב 4: חישוב מע"מ
            result.VATPercent = 18; // מע"מ נוכחי בישראל
            result.VATAmount = result.AfterDiscount * (result.VATPercent / 100);
                result.FinalTotal = result.AfterDiscount + result.VATAmount;

                Console.WriteLine($"✅ הצעת מחיר חושבה: סה\"כ {result.FinalTotal:N2} ש\"ח");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בחישוב הצעת מחיר: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// קבלת הצעות מחיר של לקוח
        /// </summary>
        public List<QuoteSummary> GetCustomerQuotes(int customerID)
        {
            try
            {
                var quotes = _db.GetQuoteItemExtendedByCustomerID(customerID);
                if (quotes == null || !quotes.Any()) return new List<QuoteSummary>();

                var groupedQuotes = quotes.GroupBy(q => q.QuoteID)
                    .Select(g => new QuoteSummary
                    {
                        QuoteID = g.Key,
                        RequestID = g.First().RequestID,
                        TotalPrice = g.First().TotalPrice,
                        DiscountPercent = g.First().DiscountPercent ?? 0,
                        CreatedAt = g.First().CreatedAt,
                        Items = g.Select(item => new QuoteItemSummary
                        {
                            CalculatorItemID = item.CalculatorItemID,
                            ItemName = item.CustomItemName,
                            Quantity = item.Quantity,
                            UnitPrice = item.PriceForItem,
                            TotalPrice = item.FinalPrice,
                            Notes = item.Notes
                        }).ToList()
                    })
                    .OrderByDescending(q => q.CreatedAt)
                    .ToList();

                return groupedQuotes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer quotes: {ex.Message}");
                return new List<QuoteSummary>();
            }
        }

        /// <summary>
        /// קבלת פריטי מחשבון זמינים
        /// </summary>
        public List<CalculatorItem> GetCalculatorItems()
        {
            try
            {
                var items = _db.GetCalculatorItems();
                return items.Select(i => new CalculatorItem
                {
                    CalculatorItemID = i.CalculatorItemID,
                    ItemName = i.ItemName,
                    Description = i.Description,
                    Price = i.Price,
                    IsActive = i.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting calculator items: {ex.Message}");
                return new List<CalculatorItem>();
            }
        }

        /// <summary>
        /// מחיקת הצעת מחיר
        /// </summary>
        public QuoteDeleteResult DeleteQuote(int quoteID)
        {
            try
            {
                Console.WriteLine($"🗑️ מוחק הצעת מחיר QuoteID: {quoteID}");

                // שלב 1: מחיקת כל פריטי ההצעה (בגלל Foreign Key)
                int deletedItems = _db.DeleteQuoteItems(quoteID);
                Console.WriteLine($"🗑️ נמחקו {deletedItems} פריטי הצעה");

                // שלב 2: מחיקת ההצעה עצמה
                bool quoteDeleted = _db.DeleteQuote(quoteID);
                
                if (quoteDeleted)
                {
                    Console.WriteLine($"✅ הצעת מחיר {quoteID} נמחקה בהצלחה");
                    return new QuoteDeleteResult
                    {
                        Success = true,
                        Message = "הצעת מחיר נמחקה בהצלחה",
                        DeletedQuoteID = quoteID,
                        DeletedItemsCount = deletedItems
                    };
                }
                else
                {
                    Console.WriteLine($"❌ שגיאה במחיקת הצעת מחיר {quoteID}");
                    return new QuoteDeleteResult
                    {
                        Success = false,
                        Message = "שגיאה במחיקת הצעת המחיר"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה במחיקת הצעת מחיר: {ex.Message}");
                return new QuoteDeleteResult
                {
                    Success = false,
                    Message = $"שגיאה: {ex.Message}"
                };
            }
        }
    }

    // מחלקות עזר
    public class CreateQuoteRequest
    {
        public int RequestID { get; set; }
        public bool IncludeBaseFloor { get; set; }
        public decimal TotalArea { get; set; }
        public string FloorType { get; set; }
        public decimal FloorPricePerMeter { get; set; }
        public decimal? DiscountPercent { get; set; }
        public List<QuoteItemRequest> Items { get; set; } = new List<QuoteItemRequest>();
    }

    public class QuoteItemRequest
    {
        public int? CalculatorItemID { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }

    public class CalculatedQuote
    {
        public List<CalculatedQuoteItem> Items { get; set; } = new List<CalculatedQuoteItem>();
        public decimal Subtotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AfterDiscount { get; set; }
        public decimal VATPercent { get; set; }
        public decimal VATAmount { get; set; }
        public decimal FinalTotal { get; set; }
    }

    public class CalculatedQuoteItem
    {
        public int? CalculatorItemID { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }

    public class QuoteCreationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int QuoteID { get; set; }
        public CalculatedQuote Quote { get; set; }
    }

    public class CalculatorItem
    {
        public int CalculatorItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuoteSummary
    {
        public int QuoteID { get; set; }
        public int RequestID { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QuoteItemSummary> Items { get; set; }
    }

    public class QuoteItemSummary
    {
        public int? CalculatorItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }

    public class QuoteDeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int DeletedQuoteID { get; set; }
        public int DeletedItemsCount { get; set; }
    }
} 