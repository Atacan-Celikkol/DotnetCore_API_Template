using Data.Models.Enums;

namespace Presentation.Api
{
    /// <summary>
    /// </summary>
    public class SearchQueryParams
    {
        /// <summary>
        /// </summary>
        public SearchQueryParams()
        {
            q = null;
            page = 0;
            size = 20;
            sort_by = null;
            order_by = OrderType.Default;
        }

        /// <summary>
        /// </summary>
        public string q { get; set; }

        /// <summary>
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// </summary>
        public int size { get; set; }

        /// <summary>
        /// </summary>
        public string sort_by { get; set; }

        /// <summary>
        /// </summary>
        public OrderType order_by { get; set; }
    }
}